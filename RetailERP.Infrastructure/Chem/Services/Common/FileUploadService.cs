using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;
using RetailERP.Core.Common;
using System.Data;
using OfficeOpenXml;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Microsoft.Data.SqlClient;
using RetailERP.Infrastructure.Chem.Interfaces.Common;

namespace RetailERP.Infrastructure.Chem.Services.Common
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly SqlConnection _connection;
        private readonly DapperContext<FileUploadModel> _context;
        public FileUploadService(IHostingEnvironment appEnvironment, DapperContext<FileUploadModel> context)
        {
            _appEnvironment = appEnvironment;
            _context = context;
            _connection = _context.Connection;
        }

        //PDF File reader
        public string PDLUpload()
        {
            string pdfFilePath = "D:/PDLFile.pdf"; // Provide the path to your PDF file

            //if (File.Exists(pdfFilePath))
            //{
            //    ReadPdf(pdfFilePath);
            //}
            //else
            //{
            //    Console.WriteLine("The specified PDF file does not exist.");
            //}

            using (PdfReader pdfReader = new PdfReader(pdfFilePath))
            {
                using (PdfDocument pdfDocument = new PdfDocument(pdfReader))
                {
                    int numberOfPages = pdfDocument.GetNumberOfPages();

                    Console.WriteLine($"Number of Pages: {numberOfPages}");

                    for (int pageNumber = 1; pageNumber <= numberOfPages; pageNumber++)
                    {
                        var text = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(pageNumber));
                        Console.WriteLine($"Page {pageNumber} Text:\n{text}");
                    }
                }
            }
            Console.ReadLine();
            return pdfFilePath;
        }

        //Index = Index is loop int value like for(int i=0; i<length.count; i++) index use for multiple value upload at a time
        public async Task<FileUploadModel> FileUpload(FileUploadModel fileUpload, string Index)
        {
            try
            {
                ///PDLUpload();

                if (fileUpload.Files != null && fileUpload.Files.Length > 0 && fileUpload.Is64Byte == false)
                {
                    string extn = "";

                    string webRoot = _appEnvironment.WebRootPath;
                    if (string.IsNullOrWhiteSpace(_appEnvironment.WebRootPath))
                    {
                        webRoot = _appEnvironment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    }

                    string path = webRoot + fileUpload.FilePath;
                    string prevPath = webRoot + fileUpload.PreviousFilePath;

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    if (File.Exists(prevPath) && !string.IsNullOrWhiteSpace(fileUpload.PreviousFilePath))
                    {
                        File.Delete(@"" + prevPath + "");
                    }

                    string fileName = string.Empty;

                    fileName = ContentDispositionHeaderValue.Parse(fileUpload.Files.ContentDisposition).FileName.Trim('"');
                    if (!fileUpload.IsName)
                    {
                        FileInfo fi = new FileInfo(fileName);
                        extn = fi.Extension;
                        fileName = string.Concat(fileUpload.PrimaryKeyID, "_", fileUpload.FileFormatName, Index, fileUpload.FileType, extn);
                    }

                    string fullPath = Path.Combine(path, fileName);

                    using (Stream stream = new FileStream(fullPath, FileMode.Create))
                    {
                        string filesToDelete = string.Concat(fileUpload.PrimaryKeyID, "_", fileUpload.FileFormatName, Index, fileUpload.FileType, extn);
                        string[] fileList = Directory.GetFiles(path, filesToDelete);

                        string fileNameExist = null;

                        foreach (string sfile in fileList)
                        {
                            if (fullPath == sfile)
                            {
                                fileNameExist = sfile;
                            }
                            else
                            {
                                File.Delete(sfile);
                            }
                        }
                        await fileUpload.Files.CopyToAsync(stream);
                    }

                    fileUpload.FilePath += fileName;

                    //File Name save in the database
                    if (!string.IsNullOrWhiteSpace(fileUpload.Usql))
                    {
                        //File Name save in the database
                        if (!fileUpload.IsMultiple)
                        { 
                            FileSave(fileUpload);
                        }
                        else
                        {
                            FileSaveMultiple(fileUpload);
                        } 
                    } 
                    return fileUpload;
                }
                else
                {
                    //Image save Byte format in the database table
                    FileUploadModel fileUploadToByte = new FileUploadModel();
                    fileUploadToByte = await ImageConvertToByte(fileUpload.Files);
                    fileUploadToByte.Usql = fileUpload.Usql;
                    fileUploadToByte.Condition = fileUpload.Condition;
                    FileSave_ByImage(fileUploadToByte);
                }
                return fileUpload;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<FileUploadModel> ImageConvertToByte(IFormFile file)
        {
            FileUploadModel fileUpload = new FileUploadModel();

            if (file != null || file.Length > 0)
            {
                byte[] ImageByteData;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    ImageByteData = memoryStream.ToArray();
                }

                fileUpload = new FileUploadModel
                {
                    ImageByteData = ImageByteData
                };
            }

            return fileUpload;
        }

        // This [ ImageConvertBase64String ] function use for Image show in the view Page
        public string ImageConvertBase64String(string ImagePath) //Like : /Files/employee/EmployeeInfo/Image.png
        {
            try
            {
                string base64String = "";
                string path = _appEnvironment.WebRootPath + ImagePath;
                byte[] b = File.ReadAllBytes(path);
                return base64String = "data:image/png;base64," + Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public MemoryStream DownloadFile(string fileUrl, string fileName)
        {
            fileUrl = @"wwwroot\" + fileUrl;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileUrl, fileName);
            var memory = new MemoryStream();
            if (File.Exists(filePath))
            {
                var webClient = new System.Net.WebClient();
                var data = webClient.DownloadData(filePath);
                memory = new MemoryStream(data);

            }
            memory.Position = 0;
            return memory;
        }

        //File Name save in the database
        public FileUploadModel FileSave(FileUploadModel fileUpload)
        {
            fileUpload.SqlQuery = fileUpload.Usql + "'" + fileUpload.FilePath + "'" + fileUpload.Condition;

            string strQuery = $@"{fileUpload.SqlQuery} "; 
            SqlCommand cmd = new SqlCommand(strQuery, _connection);
            _connection.Open();
            int i = cmd.ExecuteNonQuery();
            _connection.Close();
            return fileUpload;
        }
        public FileUploadModel FileSaveMultiple(FileUploadModel fileUpload)
        {  
            fileUpload.SqlQuery = fileUpload.Usql + " Values('"+ fileUpload.FilePath +"', " + fileUpload.PrimaryKeyID + ", 0)";  
            string strQuery = $@"{fileUpload.SqlQuery} ";
            SqlCommand cmd = new SqlCommand(strQuery, _connection);
            _connection.Open();
            int i = cmd.ExecuteNonQuery();
            _connection.Close();
            return fileUpload;
        }
        //File save in the database image format wise
        public FileUploadModel FileSave_ByImage(FileUploadModel fileUpload)
        {
            fileUpload.SqlQuery = fileUpload.Usql + " @ImageByteData" + fileUpload.Condition; 

            SqlCommand cmd = new SqlCommand(fileUpload.SqlQuery, _connection);
            cmd.Parameters.AddWithValue("@ImageByteData", fileUpload.ImageByteData);
            _connection.Open();
            cmd.ExecuteNonQuery();
            _connection.Close();
            return fileUpload;
        } 
        public int FileDelete(string Dsql)
        { 
            string strQuery = $@"{Dsql} ";
            SqlCommand cmd = new SqlCommand(strQuery, _connection);
            _connection.Open();
            int i = cmd.ExecuteNonQuery();
            _connection.Close();
            return i;
        }  
        public DataTable ExcelFileReadData(ExcelWorksheet worksheets)
        {
            var dtDataSource = new DataTable();
            ExcelWorksheet worksheet = worksheets;

            if (worksheet != null)
            {
                if (worksheet.Dimension != null)
                {
                    foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                    {
                        dtDataSource.Columns.Add(firstRowCell.Text);
                    }
                    for (var rowNumber = 2; rowNumber <= worksheet.Dimension.End.Row; rowNumber++)
                    {
                        var row = worksheet.Cells[rowNumber, 1, rowNumber, worksheet.Dimension.End.Column];
                        var newRow = dtDataSource.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        dtDataSource.Rows.Add(newRow);
                    }
                }
            }
            return dtDataSource;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

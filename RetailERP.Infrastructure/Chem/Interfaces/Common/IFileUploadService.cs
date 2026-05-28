using RetailERP.Core.Common;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Common
{
    public interface IFileUploadService:IDisposable
    { 
        Task<FileUploadModel> FileUpload(FileUploadModel fileUpload, string Index);
        MemoryStream DownloadFile(string fileUrl, string fileName);
        int FileDelete(string Dsql);
        DataTable ExcelFileReadData(ExcelWorksheet worksheets);

       

    }
}

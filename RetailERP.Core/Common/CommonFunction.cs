using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Common
{
    public class CommonFunction
    {
        public string Reverse_String(string Password)
        {
            char[] charArray = Password.ToCharArray();
            Array.Reverse(charArray);
            string strReversePassword = new string(charArray);
            return strReversePassword;
        }
        public string PasswordEncrypt(string Password)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(Reverse_String(Password));
            string EncryptedPassword = Convert.ToBase64String(plainTextBytes);
            return EncryptedPassword;
        }
        public string PasswordDecrypt(string Password)
        {
            var base64EncodedBytes = Convert.FromBase64String(Password);
            string DecryptedPassword = Reverse_String(Encoding.UTF8.GetString(base64EncodedBytes));
            return DecryptedPassword;
        }
        //Get IP Address
        public string GetIPAddress()
        {
            string IPAddress = "";
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IPAddress = ip.ToString();
                }
            }
            return IPAddress;
        }
        //public static IEnumerable FillModelStateError(ModelStateDictionary modelState)
        //{
        //    if (!modelState.IsValid)
        //    {
        //        return modelState.ToDictionary(kvp => kvp.Key,
        //            kvp => kvp.Value.Errors
        //                            .Select(e => e.ErrorMessage).ToArray())
        //                            .Where(m => m.Value.Count() > 0);
        //    }
        //    return null;
        //}
        //Convert List To DataTable
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            try
            {
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                return dataTable;
            }
            catch
            {
                return dataTable = null;
            }
            //put a breakpoint here and check datatable 
        }

        /// <summary>
        /// Return Data Set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns>DataSet</returns>
        public static DataSet ToDataSet<T>(List<T> items)
        {
            DataTable dataTable = new DataTable("tbl_" + typeof(T).Name);
            DataSet dataSet = new DataSet("ds_" + typeof(T).Name);
            try
            {
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }

                dataSet.Tables.Add(dataTable);
                return dataSet;
            }
            catch
            {
                return dataSet = null;
            }
        }

        public void SaveImage(string imgBase64, string ImgName)
        {
            string path = "../../wwwroot/Files/LCFiles"; //Path

            //Check if directory exist
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path); //Create directory if it doesn't exist
            }

            //string imageName = ImgName + ".jpg";

            //set the image path
            //string imgPath = Path.Combine(path, imageName);

            byte[] imageBytes = Convert.FromBase64String(imgBase64);

            MemoryStream imageStream = new MemoryStream();
            imageStream.Write(imageBytes, 0, imageBytes.Length);

            // Include the file name along with extension (Eg: 'C:\TestImage.JPEG')
            FileStream fs = File.Create("C:\\TestImage.JPEG");
            imageStream.WriteTo(fs);

            fs.Close();
            imageStream.Close();

        }
        public string Where_Clause(string whereCaluse, string parameter)
        {
            string sWhereClause = "";
            if (string.IsNullOrEmpty(whereCaluse) == true)
            {
                sWhereClause = " Where " + parameter;
            }
            else
            {
                sWhereClause = whereCaluse + " And " + parameter;
            }

            return sWhereClause;
        }
        public string AmountInWords(string numb)
        {
            string val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            string endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents    
                        endStr = "Paisa " + endStr;//Cents    
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = string.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }
        private string ConvertDecimals(string number)
        {
            string cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Zero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }
        private string ConvertWholeNumber(string Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = Convert.ToDouble(Number);
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    string place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = numDigits % 3 + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = numDigits % 4 + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = numDigits % 7 + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = numDigits % 10 + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }
        private string ones(string digit)
        {
            int digt = Convert.ToInt32(digit);
            string name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 2:
                    name = "Two";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 3:
                    name = "Three";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 4:
                    name = "Four";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 5:
                    name = "Five";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 6:
                    name = "Six";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 7:
                    name = "Seven";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 8:
                    name = "Eight";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 9:
                    name = "Nine";
                    break; // TODO: might not be correct. Was : Exit Select  

            }
            return name;
        }
        private string tens(string digit)
        {
            int digt = Convert.ToInt32(digit);
            string name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break; // TODO: might not be correct. Was : Exit Select        \  
                case 11:
                    name = "Eleven";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 12:
                    name = "Twelve";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 13:
                    name = "Thirteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 14:
                    name = "Fourteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 15:
                    name = "Fifteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 16:
                    name = "Sixteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 17:
                    name = "Seventeen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 18:
                    name = "Eighteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 19:
                    name = "Nineteen";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 20:
                    name = "Twenty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 30:
                    name = "Thirty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 40:
                    name = "Fourty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 50:
                    name = "Fifty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 60:
                    name = "Sixty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 70:
                    name = "Seventy";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 80:
                    name = "Eighty";
                    break; // TODO: might not be correct. Was : Exit Select  
                case 90:
                    name = "Ninety";
                    break; // TODO: might not be correct. Was : Exit Select  
                default:
                    if (digt > 0)
                    {
                        name = tens(digit.Substring(0, 1) + "0") + " " + ones(digit.Substring(1));
                    }
                    break; // TODO: might not be correct. Was : Exit Select  
            }
            return name;
        }

        public string GetStringToDate(string Date)
        {
            string convertedDate = null;
            if (Date != "")
            {
                DateTime date1 = DateTime.ParseExact(Date, "dd.MM.yy", System.Globalization.CultureInfo.InvariantCulture);
                string date = date1.ToString("yyyy-MM-dd");
                convertedDate = date;
            }
           
            return convertedDate;
        }
    }
}

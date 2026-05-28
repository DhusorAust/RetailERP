using RetailERP.Core.Chem.SmsService; 
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Common
{
    public interface IHelperService
    {
        AuditTrail SaveAuditTrail(AuditTrail entity);

        #region Ingredient
        Task<List<DropDownList>> GetIngredientBySearch(string searchKey, string itemName, int pageIndex, int pageSize);
        #endregion Ingredient


        #region Product
        Task<List<DropDownList>> GetAllProductBySearch(string searchKey, string itemName, int pageIndex, int pageSize);
        #endregion Product 

        #region Others
        Task<SMSConfig> GetSMSConfig();
        Task<string> ExcelFileConvertBase64String(DataTable dataTable);
        #endregion Others  

        #region Employee Information  
        //Task<List<Emp_EmploymentInfo>> GetEmployeeByUnitID(string searchKey, int page, int pageSize, string employeeName, string unitID, string deptIds, string secIds, string empTypeIds);
        #endregion Employee Information 
    }
}

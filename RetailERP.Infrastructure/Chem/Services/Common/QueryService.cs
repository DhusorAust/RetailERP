using RetailERP.Core.Common;

namespace RetailERP.Infrastructure.Chem.Services.Common
{
    public static class QueryService
    {
        public static string GetEmployee()
        {
            string sql = $@"
            SELECT CAST(EmployeeID AS VARCHAR) id, (EmployeeNo+' - '+ EmployeeName) [text] 
            FROM {DBSource.PDLHRM}..emp_EmploymentInfo 
            ORDER BY EmployeeNo, EmployeeName";
            return sql;
        }
    }
}

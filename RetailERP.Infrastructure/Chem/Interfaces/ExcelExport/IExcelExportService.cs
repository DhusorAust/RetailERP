using RetailERP.Core.Chem.ExcelExport;
using System.Data;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.ExcelExport
{
    public interface IExcelExportService
    {
        Task<ExcelExportParam> GetNew(int userId);
        Task<DataTable> GetExportExcelData(ExcelExportParam reportParemaeters);

    }
}

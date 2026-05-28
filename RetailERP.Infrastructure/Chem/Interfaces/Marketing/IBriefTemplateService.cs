using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.Marketing
{
    public interface IBriefTemplateService
    {
        UserList User { get; set; }
        Task<GridEntity<Mkt_BriefTemplateMaster>> GetList(GridOptions options, Status status);
        Task<Mkt_BriefTemplateMaster> GetNew();
        Task<Mkt_BriefTemplateMaster> GetEdit(int id);
        Task<List<DropDownList>> GetEmployeeList(string searchKey, int employeeId);
        Task<Mkt_BriefTemplateMaster> SaveAsync(Mkt_BriefTemplateMaster entity);
    }
}

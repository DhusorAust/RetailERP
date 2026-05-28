using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Interfaces.RnD
{
    public interface IFeedbackService
    {
        UserList User { get; set; }
        Task<GridEntity<RnD_FeedbackMaster>> GetList(GridOptions options, Status status);
        Task<RnD_FeedbackMaster> GetNew(int id);
        Task<RnD_FeedbackMaster> GetEdit(int Id);
        Task<RnD_FeedbackMaster> SaveAsync(RnD_FeedbackMaster entity);
    }
}

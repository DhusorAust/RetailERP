
using System.Threading.Tasks;
using RetailERP.Core.Chem.AuditTrails;

namespace RetailERP.Infrastructure.Chem.Interfaces.AuditTrails
{
    public interface IAuditTrailService
    {
        Task<AuditTrailEntity> SaveAsync(AuditTrailEntity entity);
        Task<TransactionUserInfo> GetTransactionUserInfo(string pageName, int id);
    }
}

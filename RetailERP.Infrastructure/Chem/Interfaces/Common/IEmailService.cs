using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;

namespace RetailERP.Infrastructure.Chem.Interfaces.Common
{
    public interface IEmailService : IDisposable
    {
        bool SendEmail(string to, string subject, string body);

        Task<BaseEntity> SendCommonEmail(string bodyData, string fromEmail, string toEmail, string cCEmailID, string mailSubject, int? referenceID);
    }
}

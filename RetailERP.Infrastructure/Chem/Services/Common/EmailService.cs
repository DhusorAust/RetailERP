using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using RetailERP.Core.Common;
using System.Data;
using Dapper;
using RetailERP.Infrastructure.Chem.Interfaces.Common;

namespace RetailERP.Infrastructure.Chem.Services.Common
{
    public class EmailService : IEmailService
    {
        private readonly DapperContext<BaseEntity> _context;
        public EmailService(DapperContext<BaseEntity> context)
        {
            _context = context;
        } 
        public bool SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"Local email skipped. To={to}, Subject={subject}");
            return true;
        }  
        public async Task<BaseEntity> SendCommonEmail(string bodyData, string fromEmail, string toEmail, string cCEmailID, string mailSubject, int? referenceID)
        {
            var entity = new BaseEntity();
            var parameters = (dynamic)null;
            parameters = new DynamicParameters();
            parameters.Add("@SaveOption", 1);               
            parameters.Add("@ModuleName", "Merchandiser");
            parameters.Add("@ReferenceID", referenceID);
            parameters.Add("@FromMailAddress", fromEmail);              
            parameters.Add("@ToMailAddress", toEmail);              
            parameters.Add("@CCMailAddress", cCEmailID);
            parameters.Add("@MailSubject", mailSubject);            
            parameters.Add("@MailBody", bodyData);
            parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            entity.NoofRows = await _context.ExecuteAsync("spSet_sett_EmailInformation", parameters, 30, CommandType.StoredProcedure);
            entity.ResultId = parameters.Get<int>("@IdentityValue");
            entity.ErrorNo = parameters.Get<int>("@ErrNo");

            if (entity.NoofRows > 0 && entity.ResultId > 0)
            {              
                entity.Message = "Email send successful.";
                entity.ErrorNo = 1;
            }
            else
            {
                entity.Message = "Email send failed.";
                entity.ErrorNo = 0;
            }
            return entity;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

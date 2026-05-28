using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Chem.SmsService;

namespace RetailERP.Infrastructure.Chem.Interfaces.SmsService
{
    public interface ICommercialExportSMSService
    {
        Task<CommercialExportHOSMS> GetCommercialExportHOSMSData(string trnsDate);
    }
}

using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.ExcelExport
{
    public class ExcelExportParam : BaseEntity
    {
        public int? ReportTypeID { get; set; }
        public int? ReportMenuID { get; set; }
        public int UnitID { get; set; }
        public int? BuyerID { get; set; }
        public int? LCID { get; set; }
        public int? InvoiceMasterID { get; set; }
        public int? CertificateTypeID { get; set; }
        public string FileNo { get; set; }
        public int? PIID { get; set; }
        public int? BuyerBankID { get; set; }
        public int? NegBankID { get; set; }
        public int? BankID { get; set; }
        public string ZoneID { get; set; }
        public int? YarnTypeID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        public List<DropDownList> ReportMenuList { get; set; } = new();
        public List<DropDownList> UnitList { get; set; } = new();
        public List<DropDownList> BuyerList { get; set; } = new();
        public List<DropDownList> NegoBankList { get; set; } = new();
        public List<DropDownList2> ZoneList { get; set; } = new();
        public List<DropDownList> CertTypeList { get; set; } = new(); 
        public List<DropDownList2> YarnTypeList { get; set; } = new();
        public List<DropDownList> InvoiceList { get; set; } = new();
        public List<DropDownList> LCList { get; set; } = new();
    }
}

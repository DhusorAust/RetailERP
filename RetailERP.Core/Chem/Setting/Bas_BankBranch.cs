using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Setting
{

    public class Bas_BankBranch : BaseEntity
    {
        public Bas_BankBranch()
        {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            };
        }
        public int BankBranchID { get; set; }
        public int BankID { get; set; }
        public string BranchCode { get; set; } = AppGlobalFields.NEW;
        public string BranchName { get; set; }
        public string BranchShortName { get; set; }
        public string BranchAddress { get; set; }
        public string SwiftCode { get; set; }
        public string RoutingNo { get; set; }
        public int IsBeneficiaryBankBranch { get; set; }

        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = System.DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        #region Additional Columns   
        [Write(false)]
        public string BankName { get; set; }

        [Write(false)]
        public string BankShortName { get; set; }
        
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        [Write(false)]
        public List<DropDownList> BankList { get; set; } = new List<DropDownList>();


        #endregion Additional Columns  

    }

}

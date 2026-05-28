using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Setting
{

    public class Bas_BuyerBankAccount : BaseEntity
    {
        public Bas_BuyerBankAccount()
        {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            };
        }
        public int BuyerBankAccountID { get; set; }
        public int BuyerID { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNo { get; set; }
        public int BankID { get; set; }
        public int? BankBranchID { get; set; }

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
        public string BranchName { get; set; }

        [Write(false)]
        public string BuyerName { get; set; }
        
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }


        [Write(false)]
        public List<DropDownList> BuyerList { get; set; } = new List<DropDownList>();


        [Write(false)]
        public List<DropDownList> BankList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<DropDownList> BankBranchList { get; set; } = new List<DropDownList>();

        #endregion Additional Columns  

    }

}

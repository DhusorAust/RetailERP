using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RetailERP.Core.Chem.Setting
{
    public class Bas_Bank : BaseEntity
    {
        public Bas_Bank() {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        }
        public int BankID { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankShortName { get; set; }
        public string BankAddress { get; set; }
        public string SwiftCode { get; set; }
        public string ADCode { get; set; }
        public int IsBeneficiaryBank { get; set; }
        public int IsAdvisingBank { get; set; }
        public int IsNegoBank { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }= System.DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }





        #region Additional Columns   

        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns  

    }
}

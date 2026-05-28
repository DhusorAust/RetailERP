using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RetailERP.Core.Chem.Setting
{
    public class Bas_Brand : BaseEntity
    {
        public Bas_Brand()
        {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            };
        }
        public int BrandID { get; set; }
        public string BrandName { get; set; }
        public string BrandShortName { get; set; }
        public string Phone { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public int? CountryID { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactNo { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = System.DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }


        #region Additional Columns   
        public string CountryName { get; set; }
        public string IsActiveStatus { get; set; }
        public List<DropDownList> CountryList { get; set; }
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns   
    }
}

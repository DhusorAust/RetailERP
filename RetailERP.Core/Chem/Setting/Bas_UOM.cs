using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RetailERP.Core.Chem.Setting
{
    public class Bas_UOM : BaseEntity
    {
        public Bas_UOM() {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        } 
        public int UOMID { get; set; }
        public string UOM { get; set; }
        public string UOMFullName { get; set; }
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
        public string IsActiveStatus { get; set; }
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns  

    }
}

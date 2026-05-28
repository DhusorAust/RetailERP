using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RetailERP.Core.Chem.Setting
{
    public class Bas_Season : BaseEntity
    {
        public Bas_Season() {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        } 
        public int SeasonID { get; set; }
        public string SeasonName { get; set; }
        public DateTime? StartMonth { get; set; }
        public DateTime? EndMonth { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }= System.DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        [Write(false)]
        public string StartMonthDate { get; set; }
        [Write(false)]
        public string EndMonthDate { get; set; } 

        #region Additional Columns   
        [Write(false)]
        public string IsActiveStatus { get; set; }
        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns  

    }
}

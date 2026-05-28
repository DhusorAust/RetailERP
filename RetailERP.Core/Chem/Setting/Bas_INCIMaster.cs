using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace RetailERP.Core.Chem.Setting
{
    public class Bas_INCIMaster : BaseEntity
    {
        public Bas_INCIMaster() {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        }
        public int INCIID { get; set; }
        public string INCICode { get; set; } = AppGlobalFields.NEW;
        public string INCIName { get; set; }
        public string CASNo { get; set; }
        public int IsAllergen { get; set; } 
        public string Remarks { get; set; } 
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }= System.DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }


        #region Additional Columns 
        public string IsActiveStatus { get; set; } 
        public string AllergenStatus { get; set; }

        [Write(false)]
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns  

    }
}

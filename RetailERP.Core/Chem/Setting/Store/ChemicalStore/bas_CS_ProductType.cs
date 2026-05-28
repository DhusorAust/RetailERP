using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace RetailERP.Core.Chem.Setting.Store.ChemicalStore
{
    public class bas_CS_ProductType : BaseEntity
    {
        public bas_CS_ProductType() {
            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        } 
        public int ProductTypeID { get; set; } 
        public string ProductTypeName { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; }
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

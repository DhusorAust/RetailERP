using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;

namespace RetailERP.Core.Chem.Setting.Store.ChemicalStore
{
    public class Bas_CS_ProductMaster : BaseEntity
    {
        public Bas_CS_ProductMaster() {

            IsActiveList = new List<DropDownList>
            {
                new DropDownList { text = "Active",   id = 1 },
                new DropDownList { text = "Inactive", id = 0 }
            }; 
        } 
        public int ProductID { get; set; }
        public string ProductCode { get; set; } = AppGlobalFields.NEW;
        public string ProductName { get; set; } 
        public int ProductCategoryID { get; set; }
        public int ProductTypeID { get; set; } 
        public int ItemTypeID { get; set; } 
        public string Remarks { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        #region Additional Columns
        public string ProductSize { get; set; }
        public int? CountryID { get; set; }
        public int? UOMID { get; set; } 
        public string ProductCategoryName { get; set; }
        public string ProductTypeName { get; set; }
        public string IsActiveStatus { get; set; }   
        public List<DropDownList> ProductCategoryList { get; set; }= new List<DropDownList>();
        public List<DropDownList> ProductTypeList { get; set; }=new List<DropDownList>();
        public List<DropDownList> ItemTypeList { get; set; } = new List<DropDownList>();
        public List<DropDownList> IsActiveList { get; set; }

        #endregion Additional Columns  

    }
}

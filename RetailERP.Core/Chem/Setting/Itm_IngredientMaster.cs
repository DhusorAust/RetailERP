using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;

namespace RetailERP.Core.Chem.Setting
{
    public class Itm_IngredientMaster : BaseEntity
    {
        public Itm_IngredientMaster()
        {
            IngredientCode = AppGlobalFields.NEW;
            CreatedDate = DateTime.Now;
            IsActive = 1;
            Approved = false;
            IsFragrance = 0;
            IsAllergenApplicable = 0;
        }

        public int IngredientID { get; set; }
        public string IngredientCode { get; set; }
        public string IngredientName { get; set; }
        public string IngredientDescription { get; set; }
        public int? IngredientCategoryID { get; set; }
        public int? IngredientTypeID { get; set; }
        public decimal? NaturalIndexPct { get; set; }
        public decimal? NaturalOriginIndexPct { get; set; }
        public decimal? OrganicIndexPct { get; set; }
        public decimal? OrganicOriginIndexPct { get; set; }
        public decimal? MOQInKg { get; set; }
        public int? LeadTime { get; set; }
        public string LeadTimeUnit { get; set; }
        public string IngredientStatus { get; set; }
        public int? IsFragrance { get; set; }
        public int? IsAllergenApplicable { get; set; }
        public string Remarks { get; set; }
        public int? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Approved { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        #region Additional Columns 


        public List<DropDownList> IngredientStatusList { get; set; } = new List<DropDownList>(); 
        public List<DropDownList> INCIBreakDownList { get; set; } = new List<DropDownList>();

        public List<DropDownList> AllergensBreakDownList { get; set; } = new List<DropDownList>();
        public List<DropDownList> SupplierList { get; set; } = new List<DropDownList>();
        public List<DropDownList> CountryList { get; set; } = new List<DropDownList>();
        public List<DropDownList> LeadTimeUnitList { get; set; } = new List<DropDownList>();
        public List<DropDownList> IngredientManufacturerList { get; set; } = new List<DropDownList>();
        public string IngredientManufacturerIds { get; set; }




        // Childs Tables 
        public List<Itm_INCIBreakdown> INCIBreakdowns { get; set; } = new List<Itm_INCIBreakdown>();

        [Write(false)]
        public List<Itm_AllergensBreakdown> AllergensBreakdowns { get; set; } = new List<Itm_AllergensBreakdown>();
        [Write(false)]
        public List<Itm_IngredientSupplier> IngredientSuppliers { get; set; } = new List<Itm_IngredientSupplier>();
        [Write(false)]
        public List<Itm_IngredientManufacturer> IngredientManufacturers { get; set; } = new List<Itm_IngredientManufacturer>();
        [Write(false)]
        public List<Itm_IngredientRegulatoryRestriction> IngredientRegulatoryRestrictions { get; set; } = new List<Itm_IngredientRegulatoryRestriction>();
        [Write(false)]
        public List<Itm_IngredientDocument> IngredientDocuments { get; set; } = new List<Itm_IngredientDocument>();

        #endregion Additional Columns  
    }
}

using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("rnd_FormulationMaster")]
    public class RnD_FormulationMaster : BaseEntity
    {
        public RnD_FormulationMaster()
        {

        }
        public int FormulationMasterID { get; set; }
        public string FormulationCode { get; set; } = AppGlobalFields.NEW;
        public string FormulationVersion { get; set; } = AppGlobalFields.NEW;
        public int FormulationBy { get; set; }
        public int UnitID { get; set; } 
        public int BrandID { get; set; } 
        public int ProductCategoryID { get; set; }
        public int ProductTypeID { get; set; }
        public int ProductID { get; set; }
        public DateTime FormulationDate { get; set; } = DateTime.Now;
        public decimal BatchQty { get; set; } = 0;
        public int BatchUOMID { get; set; }
        public decimal? TotalPercentage { get; set; }
        public decimal? TotalFormulationPrice { get; set; }
        public decimal? TotalNaturalIndexPct { get; set; }
        public decimal? TotalNaturalOriginIndexPct { get; set; }
        public decimal? TotalOrganicIndexPct { get; set; }
        public decimal? TotalOrganicOriginIndexPct { get; set; }
        public string Remarks { get; set; }
        public int? FormulationStatus { get; set; }
        public string Appearance { get; set; }
        public string Colour { get; set; }
        public string Odour { get; set; }
        public string Viscosity { get; set; }
        public string PH { get; set; }
        public string SG { get; set; }
        public int IsLocked { get; set; } = 0;
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int Approved { get; set; } = 0;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? Revise { get; set; } = 0;
        public int? ReviseNo { get; set; } = 0;
        public int? ReviseBy { get; set; }
        public DateTime? ReviseDate { get; set; }
        public string ReviseReason { get; set; }
        public int Reject { get; set; } = 0;
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string RejectReason { get; set; }

        #region Additional Columns 
        public string BrandName { get; set; }
        public string FormulationByName { get; set; }
        public string UnitName { get; set; }
        public string ProductName { get; set; }
        public string FormulationStatusName { get; set; }
        public string LockedStatus { get; set; }
        public List<DropDownList> BrandList { get; set; } = new List<DropDownList>();
        public List<DropDownList> UnitList { get; set; } = new List<DropDownList>();
        public List<DropDownList> FormulationStatusList { get; set; } = new List<DropDownList>();
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        public List<DropDownList> BatchUOMList { get; set; } = new List<DropDownList>();
        
        //Child
        public List<DropDownList> StepList { get; set; } = new List<DropDownList>();
        public List<DropDownList> IngredientList { get; set; } = new List<DropDownList>();
        public List<RnD_FormulationChild> Childs { get; set; } = new List<RnD_FormulationChild>();
        public List<RnD_FormulationMethod> ChildsMethod { get; set; } = new List<RnD_FormulationMethod>();

        
        //public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        //public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        //public List<DropDownList> RnDByList { get; set; } = new List<DropDownList>();
        //public List<DropDownList> RnDTeamList { get; set; } = new List<DropDownList>();
        ////Child
        //public List<DropDownList> PhaseList { get; set; } = new List<DropDownList>(); 
        //public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        //public List<DropDownList> SDRFMasterList { get; set; } = new List<DropDownList>(); 
        //public List<RnD_FormulationSe> ChildsMethod { get; set; } = new List<RnD_FormulationMethod>();
        #endregion Additional Columns
    }
}

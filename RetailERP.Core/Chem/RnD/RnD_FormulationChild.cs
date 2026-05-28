using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("rnd_FormulationChild")]
    public class RnD_FormulationChild
    {
        public int FormulationChildID { get; set; }
        public int FormulationMasterID { get; set; }
        public int StepID { get; set; }
        public int IngredientID { get; set; }
        public decimal? IngredientPercentage { get; set; }
        public decimal? IngredientQtyKg { get; set; }
        public decimal? IngredientQtyGM { get; set; }
        public decimal? IngredientCost { get; set; }
        public decimal? IngredientPricePerQtyUsed { get; set; }
        public decimal? MOQInKg { get; set; }
        public decimal? LeadTime { get; set; }
        public int? LeadTimeUOMID { get; set; }
        public decimal? NaturalIndexPct { get; set; }
        public decimal? NaturalOriginIndexPct { get; set; }
        public decimal? OrganicIndexPct { get; set; }
        public decimal? OrganicOriginIndexPct { get; set; }
        public string RegulatoryWarning { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;


        #region Additional Columns 
        public string StepNo { get; set; }
        public string IngredientName { get; set; }
        public string IngredientCode { get; set; }
        public string INCIName { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public string LeadTimeUnit { get; set; }
        public decimal? NaturalIndexIngr { get; set; }
        public decimal? NaturalOriginIngr { get; set; }
        public decimal? OrganicIngre { get; set; }
        public decimal? OrganicOriginIngre { get; set; }

        public DropDownList objStepList { get; set; } = new DropDownList();
        public DropDownList objIngredientList { get; set; } = new DropDownList(); 
        public DropDownList objLeadTimeUOMList { get; set; } = new DropDownList(); 

        #endregion Additional Columns 
    }
    public class TT_RnD_FormulationChild
    {
        public int FormulationChildID { get; set; }
        public int FormulationMasterID { get; set; }
        public int? StepID { get; set; }
        public int? IngredientID { get; set; }
        public decimal? IngredientPercentage { get; set; }
        public decimal? IngredientQtyKg { get; set; }
        public decimal? IngredientQtyGM { get; set; }
        public decimal? PricePerKg { get; set; }
        public decimal? IngredientPricePerQtyUsed { get; set; }
        public decimal? MOQInKg { get; set; }
        public decimal? LeadTime { get; set; }
        public int? LeadTimeUOMID { get; set; }
        public decimal? NaturalIndexPct { get; set; }
        public decimal? NaturalOriginIndexPct { get; set; }
        public decimal? OrganicIndexPct { get; set; }
        public decimal? OrganicOriginIndexPct { get; set; }
        public decimal? IngredientCost { get; set; }
        public string RegulatoryWarning { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}

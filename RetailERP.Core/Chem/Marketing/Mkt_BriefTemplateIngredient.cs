using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplateIngredient
    {
        public int BTIngredientID { get; set; }
        public int BTMasterID { get; set; }
        public int IngredientRequirementID { get; set; }
        public int IngredientID { get; set; }
        public string RequirmentLevel { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }

    public class TT_Mkt_BriefTemplateIngredient
    {
        public int BTIngredientID { get; set; }
        public int BTMasterID { get; set; }
        public int IngredientRequirementID { get; set; }
        public int IngredientID { get; set; }
        public string RequirmentLevel { get; set; }
        public string Remarks { get; set; }
        public int RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }


}

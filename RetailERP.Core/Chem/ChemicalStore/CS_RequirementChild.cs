using RetailERP.Core.Common;
using Dapper.Contrib.Extensions; 
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.ChemicalStore
{
    [Table("CS_RequirementChild")]
    public class CS_RequirementChild
    {
        public CS_RequirementChild()
        {
             
        }
        public int RequirementChildID { get; set; }
        public int RequirementMasterID { get; set; }
        public int? StyleID { get; set; }
        public int? POMasterID { get; set; }
        public int? POChildID { get; set; }
        public int? IndentMasterID { get; set; }
        public int? IndentChildID { get; set; }
        public int? SectionID { get; set; }
        public int ProductID { get; set; }
        public int? UOMID { get; set; }
        public decimal RequirementQty { get; set; }
        public string ChildsRemarks { get; set; }
        public int? RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

        #region Additional Columns 
        [Write(false)]
            public string SectionName { get; set; }

            [Write(false)]
            public string ProductName { get; set; }
            [Write(false)]
            public string UOM { get; set; }
            [Write(false)]
            public string StockQty { get; set; }
            // Child List

            [Write(false)]
            public DropDownList objProductList { get; set; } = new DropDownList();
            [Write(false)]
            public DropDownList objSectionList { get; set; } = new DropDownList(); 
            public DropDownList objUOMList { get; set; } = new DropDownList(); 

            #endregion Additional Columns
    }
    public class TT_CS_RequirementChild
    {
        public int RequirementChildID { get; set; }
        public int RequirementMasterID { get; set; }
        public int? StyleID { get; set; }
        public int? POMasterID { get; set; }
        public int? POChildID { get; set; }
        public int? IndentMasterID { get; set; }
        public int? IndentChildID { get; set; }
        public int? SectionID { get; set; }
        public int? ProductID { get; set; }
        public int? UOMID { get; set; }
        public decimal RequirementQty { get; set; }
        public string ChildsRemarks { get; set; }
        public int? RowSLNo { get; set; } = 0;
        public int RowSLNoNew { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}

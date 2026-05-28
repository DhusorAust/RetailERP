using RetailERP.Core.Common;
using Dapper.Contrib.Extensions; 
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.ChemicalStore
{
    [Table("CS_IndentChild")]
    public class CS_IndentChild
    {
        public CS_IndentChild()
        {
             
        }
        public int IndentChildID { get; set; }
        public int PRMasterID { get; set; }
        public int? IndentMasterID { get; set; }
        public int? SSReturnChildID { get; set; }
        public int? ProductID { get; set; }
        public int? UOMID { get; set; }
        public DateTime? ValidityDate { get; set; } 
        public int? SectionID { get; set; }
        public decimal? PRQty { get; set; } = 0;
        public decimal? IndentQty { get; set; } = 0;
        public string PRChildRemarks { get; set; }
        public string IndentChildRemarks { get; set; }
        public int RowSLNo { get; set; } = 0;
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
        public DropDownList objUOMList { get; set; } = new DropDownList();


        [Write(false)]
        public DropDownList objSectionList { get; set; } = new DropDownList(); 

        #endregion Additional Columns
    }
    public class TT_CS_IndentChild
    {
        public int IndentChildID { get; set; }
        public int PRMasterID { get; set; }
        public int? IndentMasterID { get; set; }
        public int? SSReturnChildID { get; set; }
        public int? ProductID { get; set; }
        public int? UOMID { get; set; }
        public DateTime? ValidityDate { get; set; }
        public int? SectionID { get; set; }
        public decimal? PRQty { get; set; } = 0;
        public decimal? IndentQty { get; set; } = 0;
        public string PRChildRemarks { get; set; }
        public string IndentChildRemarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}

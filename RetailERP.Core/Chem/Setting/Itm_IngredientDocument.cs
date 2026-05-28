using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
namespace RetailERP.Core.Chem.Setting
{
    public class Itm_IngredientDocument
    {
        public int IngredientDocumentID { get; set; }
        public int IngredientID { get; set; }
        public string FilePath { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }

    public class TT_Itm_IngredientDocument
    {
        public int IngredientDocumentID { get; set; }
        public int IngredientID { get; set; }
        public string FilePath { get; set; }
        public int? RowSLNo { get; set; }
        public int IsDeleted { get; set; }
    }
}

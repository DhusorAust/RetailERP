using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    [Table("rnd_FeedbackFeel")]
    public class RnD_FeedbackFeel
    {
        public int FeedbackFeelID { get; set; }
        public int FeedbackChildID { get; set; }
        public int FeelID { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

        #region Additional Columns 
        public string FeelName { get; set; }
       
        //public string Phase { get; set; }
        //public string ProductCategoryName { get; set; }
        //public string ProductTypeName { get; set; }
        //public string ProductName { get; set; }
        //public DropDownList objPhaseList { get; set; } = new DropDownList();
        //public DropDownList objProductList { get; set; } = new DropDownList();

        #endregion Additional Columns 
    }
    public class TT_RnD_FeedbackFeel
    {
        public int FeedbackFeelID { get; set; }
        public int FeedbackChildID { get; set; }
        public int FeelID { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
}

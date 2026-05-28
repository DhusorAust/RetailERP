using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.RnD
{
    [Table("rnd_StabilityTestChild")]
    public class RnD_StabilityTestChild
    {
        public int StabilityTestChildID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public string StabilityTestTemperature { get; set; }
        public string ParameterName { get; set; }
        public string T0 { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string T5 { get; set; }
        public string T6 { get; set; }
        public int IsActive { get; set; }=1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        #region Additional Columns
        public DropDownList objFormulationParameterList { get; set; } = new DropDownList();
        public String TemperatureValue { get; set; } 

        #endregion
    }

    public class TT_RnD_StabilityTestChild
    {
        public int StabilityTestChildID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public string StabilityTestTemperature { get; set; }
        public string ParameterName { get; set; }
        public string T0 { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string T5 { get; set; }
        public string T6 { get; set; }
        public int IsActive { get; set; }=1;

    }
}

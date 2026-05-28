using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.RnD
{
    public class RnD_StabilityTestTemperature
    {
        public int StabilityTestTemperatureID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public string StabilityTestTemperature { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int DeletedBy { get; set; }
        public DateTime DeletedDate { get; set; }
    }
    public class TT_RnD_StabilityTestTemperature
    {
        public int StabilityTestTemperatureID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public string StabilityTestTemperature { get; set; }
        public bool IsDeleted { get; set; }=false;
        
    }
}

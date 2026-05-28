using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.RnD
{
    public class RnD_StabilityTestMaster : BaseEntity
    {
        public int StabilityTestMasterID { get; set; }
        public int FormulationMasterID { get; set; }
        public int IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        #region Additional Columns
        public string FormulationCode { get; set; }
        public string FormulationVersion { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public List<DropDownList> FormulationMasterList { get; set; } = new();
        public List<DropDownList> FormulationParameterList { get; set; } = new();
        public List<RnD_StabilityTestChild> Childs { get; set; }  = new();
        public List<RnD_StabilityTestTemperature> StabilityTestTemperatures  { get; set; }  = new();
        public List<Rnd_StabilityTestFilePath> StabilityTestFilePaths { get; set; } = new();
        #endregion
    }
}

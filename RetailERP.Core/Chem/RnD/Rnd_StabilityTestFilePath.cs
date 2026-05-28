using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.RnD
{
    public class Rnd_StabilityTestFilePath
    {
        public int StabilityTestFilePathID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public int StabilityTestTemperature { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int IsActive { get; set; }
    }
    public class TT_Rnd_StabilityTestFilePath
    {
        public int StabilityTestFilePathID { get; set; }
        public int StabilityTestMasterID { get; set; }
        public int StabilityTestTemperature { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int IsActive { get; set; }
    }
}

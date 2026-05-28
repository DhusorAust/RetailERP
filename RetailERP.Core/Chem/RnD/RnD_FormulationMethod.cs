using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class RnD_FormulationMethod
    {
        public int FormulationMethodID { get; set; }
        public int FormulationMasterID { get; set; }
        public string Instruction { get; set; }
        public string InstructionRemarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;
    }
    public class TT_RnD_FormulationMethod
    {
        public int FormulationMethodID { get; set; }
        public int FormulationMasterID { get; set; }
        public string Instruction { get; set; }
        public string InstructionRemarks { get; set; }
        public int RowSLNo { get; set; } = 0;
        public int RowSLNoNew { get; set; } = 0;       
        public int IsDeleted { get; set; } = 0;
    }
}

using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.Security
{
    [Table("MenuParameter")]
    public class MenuParameter
    {
        public MenuParameter()
        {
            ParameterID = 0;
            MenuID = 0;
            SequenceNo = 0;
        }
        public int ParameterID { get; set; }
        public int MenuID { get; set; }
        public string ParameterValue { get; set; }
        public int SequenceNo { get; set; }
    }
}

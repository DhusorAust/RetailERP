using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RetailERP.Core.Common;

namespace RetailERP.Core.Chem.SmsService
{
    public class ProductionDeliverySMS:BaseEntity
    {
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public DateTime TransDate { get; set; }
        public string ShortUnitName { get; set; }
        public decimal BagQty { get; set; }
        public decimal Qty { get; set; }
        public decimal CHCount { get; set; }
        public int ProjectSL { get; set; }
    }
}

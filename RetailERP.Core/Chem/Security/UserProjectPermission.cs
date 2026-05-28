using Dapper.Contrib.Extensions;
using RetailERP.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Security
{
    [Table("UserProjectPermission")]
    public class UserProjectPermission
    {
        public UserProjectPermission()
        {
            CreatedDate = DateTime.Now;
            IsActive = 1;
        }
        public int UserID { get; set; }
        public string ProjectID { get; set; } 
        public byte IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}

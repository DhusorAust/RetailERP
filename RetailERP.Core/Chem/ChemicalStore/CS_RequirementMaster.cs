using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.ChemicalStore
{
    [Table("CS_RequirementMaster")]
    public class CS_RequirementMaster : BaseEntity
    {
        public CS_RequirementMaster()
        { 
             
        }
        public int RequirementMasterID { get; set; }
        public int? UnitID { get; set; }
        public int? StoreID { get; set; }
        public string RequirementNo { get; set; } = AppGlobalFields.NEW;
        public DateTime RequirementDate { get; set; } = DateTime.Now;
        public int? RequirementBy { get; set; }
        public int? DepartmentID { get; set; }
        public int? SectionID { get; set; }
        public int? SubSectionID { get; set; }
        public int? SubStoreID { get; set; }
        public string SRKhataNo { get; set; }
        public string Remarks { get; set; }
        public int IsActive { get; set; } = 1;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool SendForApproval { get; set; } = false;
        public int? SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalDate { get; set; }
        public bool Approved { get; set; } = false;
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int ReviseNo { get; set; } = 0;
        public int? ReviseBy { get; set; }
        public DateTime? ReviseDate { get; set; }
        public string ReviseReason { get; set; } 
        public bool Reject { get; set; } = false;
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; }
        public string RejectReason { get; set; }
        #region Additional Columns 
        [Write(false)]
        public string UnitName { get; set; }
        [Write(false)]
        public string StoreName { get; set; }
        [Write(false)]
        public string RequirementByName { get; set; }
        [Write(false)]
        public string DepartmentName { get; set; }
        [Write(false)]
        public string SectionName { get; set; }
        [Write(false)]
        public string SubSectionName { get; set; }
        [Write(false)]
        public string SubStoreName { get; set; }

        [Write(false)]
        public bool IsRevise { get; set; } = false;

        // Master List 

        [Write(false)]
        public List<DropDownList> UnitList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> StoreList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> RequirementByList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> DepartmentList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> SectionList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> SubSectionList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> SubStoreList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();


        [Write(false)]
        public List<CS_RequirementChild> Childs { get; set; } = new List<CS_RequirementChild>();

        public Parameter parameter { get; set; }

        #endregion Additional Columns
    }
}

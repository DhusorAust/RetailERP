using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Chem.Setting.Store.ChemicalStore;
using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.ChemicalStore
{
    [Table("CS_IssueMaster")]
    public class CS_IssueMaster : BaseEntity
    {
        public CS_IssueMaster()
        {
            //IssueDate = System.DateTime.Now;
        }

        public int IssueMasterID { get; set; }
        public int? UnitID { get; set; }
        public int? StoreID { get; set; }
        public DateTime IssueDate { get; set; } = DateTime.Now;
        public string IssueNo { get; set; } = AppGlobalFields.NEW;
        public int IssueTypeID { get; set; }
        public int RequirementMasterID { get; set; }
        public int? DepartmentID { get; set; }
        public int? SectionID { get; set; }
        public int? SubSectionID { get; set; }
        public int? IssuedBy { get; set; }
        public string IssueRemarks { get; set; }
        public string IssueToLocation { get; set; }
        public string Remarks { get; set; }        
        public bool? IsActive { get; set; }
        public bool? IsReturnable { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public bool SendForApproval { get; set; }
        public int? SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalDate { get; set; } = DateTime.Now;
        public bool Approved { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; } = DateTime.Now;
        public bool Acknowledge { get; set; }
        public int? AcknowledgeBy { get; set; }
        public DateTime? AcknowledgeDate { get; set; } = DateTime.Now;
        public bool Reject { get; set; }
        public int? RejectBy { get; set; }
        public DateTime? RejectDate { get; set; } = DateTime.Now;
        public string RejectReason { get; set; }


        #region Additional Columns                
        [Write(false)] public string UnitName { get; set; }
        [Write(false)] public string StoreName { get; set; }
        [Write(false)] public string IssueTypeName { get; set; }
        [Write(false)] public string DepartmentName { get; set; }
        [Write(false)] public string SectionName { get; set; }
        [Write(false)] public string SubSectionName { get; set; }
        [Write(false)] public string IssuedByName { get; set; }

        [Write(false)] public string RequirementNo { get; set; }
        [Write(false)] public string RequirementByName { get; set; }

        public DateTime? RequirementDate { get; set; } = DateTime.Now;

        [Write(false)] public List<DropDownList> UnitList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> StoreList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> IssueTypeList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> SectionList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> IssuedByList { get; set; } = new List<DropDownList>();

        //// Child List 
        //[Write(false)] public List<DropDownList> Child_SectionList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        [Write(false)] public List<DropDownList> ReceiveChildList { get; set; } = new List<DropDownList>();

       
        public List<CS_IssueChild> Childs { get; set; } = new List<CS_IssueChild>();
        #endregion Additional Columns
    }
}

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
    [Table("CS_IndentMaster")]
    public class CS_IndentMaster : BaseEntity
    {
        public CS_IndentMaster()
        {
            IndentDate = System.DateTime.Now;
        }
        public int IndentMasterID { get; set; }
        public int UnitID { get; set; }
        public int StoreID { get; set; }
        public int PRMasterID { get; set; }
        public string IndentNo { get; set; } = AppGlobalFields.NEW;
        public DateTime? IndentDate { get; set; } //= DateTime.Now;
        public int IndentTypeID { get; set; }
        public int IndentBy { get; set; }
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
        public string PRNo { get; set; }
        [Write(false)]
        public DateTime PRDate { get; set; } = DateTime.Now;

        [Write(false)]
        public string IndentByName { get; set; }
        [Write(false)]
        public string PRByName { get; set; }
        [Write(false)]
        public string IndentTypeName { get; set; }

        [Write(false)]
        public string DepartmentName { get; set; }
        [Write(false)]
        public string SectionName { get; set; }
        [Write(false)]
        public string SubSectionName { get; set; } 
        
        [Write(false)]
        public string UnitName { get; set; }  

        [Write(false)]
        public string StoreName { get; set; }
        [Write(false)]
        public bool IsRevise { get; set; }

        // Master List 
        [Write(false)]
        public List<DropDownList> StoreList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> UnitList { get; set; } = new List<DropDownList>(); 

        [Write(false)]
        public List<DropDownList> IndentByList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> IndentTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ProductList { get; set; }
        // Child List 
        [Write(false)]
        public List<CS_IndentChild> Childs { get; set; } = new List<CS_IndentChild>();
        #endregion Additional Columns
    }
}

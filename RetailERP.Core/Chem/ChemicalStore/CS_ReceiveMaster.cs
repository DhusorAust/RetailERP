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
    [Table("CS_ReceiveMaster")]
    public class CS_ReceiveMaster : BaseEntity
    {
        public CS_ReceiveMaster()
        {
            //ReceiveDate = System.DateTime.Now;
        }
        public int ReceiveMasterID { get; set; }
        public string ReceiveNo { get; set; }
        public DateTime ReceiveDate { get; set; } = DateTime.Now;
        public int ReceiveTypeID { get; set; }
        public int ReceiveBy { get; set; }
        public int? CheckedBy { get; set; }
        public int? PIMasterID { get; set; }
        public int? SupplierID { get; set; }
        public int? Country { get; set; }
        public string ChallanNo { get; set; }
        public DateTime ChallanDate { get; set; } = DateTime.Now;
        public string GatePassNo { get; set; }
        public DateTime GatePassDate { get; set; } = DateTime.Now;
        public int? TransportID { get; set; }
        public string VehicleNo { get; set; }
        public string QCNo { get; set; }
        public string MRRNo { get; set; }

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


        public bool SendForQC { get; set; } = false;
        public int? SendForQCBy { get; set; }
        public DateTime? SendForQCDate { get; set; }
        public bool IsQCDone { get; set; } = false;    



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

        [Write(false)]
        public string ReceiveTypeName { get; set; }
        [Write(false)]
        public string ReceiveByName { get; set; }
        [Write(false)]
        public string CheckedByName { get; set; }
        [Write(false)]
        public string PIMasterName { get; set; }
        [Write(false)]
        public string SupplierName { get; set; }
        [Write(false)]
        public string CountryName { get; set; }
        [Write(false)]
        public string TransportName { get; set; }

        [Write(false)]
        public string IndentNos { get; set; }

        [Write(false)]
        public int? SSReturnMasterID { get; set; }

        [Write(false)]
        public int CHKID { get; set; }

        [Write(false)]
        public string check_row { get; set; }

        [Write(false)]
        public string RowSelected { get; set; }

        [Write(false)]
        public decimal? IndentQty { get; set; }
        [Write(false)]
        public decimal? ReceiveQty { get; set; }
        [Write(false)]
        public decimal? ReceivedQty { get; set; }

        [Write(false)]        
        public decimal? BalanceQty { get; set; }

        [Write(false)]
        public string MRRNos { get; set; }    


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

        [Write(false)]
        public List<DropDownList> QCStatustList { get; set; }


        [Write(false)]
        public List<DropDownList> QCBytList { get; set; }

        [Write(false)]
        public List<DropDownList> LocationList { get; set; }

        [Write(false)]
        public List<DropDownList> RackList { get; set; }

        // Child List 

        [Write(false)]
        public List<DropDownList> ReceiveTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ReceiveByList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> CheckedByList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> PIMasterList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> SupplierList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> CountryList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> TransportList { get; set; } = new List<DropDownList>();
        
        [Write(false)]
        public List<DropDownList> ERP_Setting_List { get; set; } = new List<DropDownList>();
        public List<CS_ReceiveChild> Childs { get; set; } = new List<CS_ReceiveChild>();
        #endregion Additional Columns
    }
}

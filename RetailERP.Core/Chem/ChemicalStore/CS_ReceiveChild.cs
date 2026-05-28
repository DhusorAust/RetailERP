using RetailERP.Core.Common;
using Dapper.Contrib.Extensions; 
using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Chem.ChemicalStore
{
    [Table("CS_ReceiveChild")]
    public class CS_ReceiveChild
    {
        public CS_ReceiveChild()
        {
             
        }
        public int PRMasterID { get; set; }
        public int? SSReturnChildID { get; set; }
        public DateTime? ValidityDate { get; set; } 
        public decimal? PRQty { get; set; } = 0;
        public decimal? IndentQty { get; set; } = 0;
        public string PRChildRemarks { get; set; }
        public string IndentChildRemarks { get; set; }
        public int ReceiveChildID { get; set; }
        public int StoreID { get; set; }
        public int UnitID { get; set; }
        public int ReceiveMasterID { get; set; }
        public int IndentMasterID { get; set; }
        public int IndentChildID { get; set; }
        public int ProductID { get; set; }
        public int? UOMID { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? SectionID { get; set; }
        public int? LocationID { get; set; }
        public int? RackID { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? RejectionQty { get; set; }
        public decimal? DamageQty { get; set; }

        public string ChildsRemarks { get; set; }
        public int? QCStatus { get; set; }
        public int? QCBy { get; set; }
        public DateTime? QCDate { get; set; }
        public string QCRemarks { get; set; }


        public string MRR_Number { get; set; }

        public int RowSLNo { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;


        #region Additional Columns 
        [Write(false)]
        public string SectionName { get; set; }

        [Write(false)]
        public string ProductName { get; set; }
        [Write(false)]
        public string UOM { get; set; }
        [Write(false)]
        public string StockQty { get; set; }
        [Write(false)]
        public string StockQtyB { get; set; }

        [Write(false)]
        public string QCStatusName { get; set; }


        [Write(false)]
        public string QCByName { get; set; }

        [Write(false)]
        public string UnitName { get; set; }

        [Write(false)]
        public string StoreName { get; set; }

        [Write(false)]
        public string LocationName { get; set; }

        [Write(false)]
        public string RackName { get; set; }


        [Write(false)]
        public string IndentTypeName { get; set; }

        [Write(false)]
        public string IndentNo { get; set; }

        [Write(false)]
        public decimal? ReceivedQty { get; set; } = 0;

        [Write(false)]
        public decimal? ReceivedQtyPrev { get; set; } = 0;

        // Child List

        [Write(false)]
        public DropDownList objProductList { get; set; } = new DropDownList();
        [Write(false)]
        public DropDownList objSectionList { get; set; } = new DropDownList();

        [Write(false)]
        public DropDownList objLocationList { get; set; } = new DropDownList();

        [Write(false)]
        public DropDownList objRackList { get; set; } = new DropDownList();



        [Write(false)]
        public DropDownList objQCStatusList { get; set; } = new DropDownList();

        [Write(false)]
        public DropDownList objQCByList { get; set; } = new DropDownList();




        #endregion Additional Columns
    }
    public class TT_CS_ReceiveChild
    {
        public int PRMasterID { get; set; }
        public int? SSReturnChildID { get; set; }
        public DateTime? ValidityDate { get; set; }
        public decimal? PRQty { get; set; } = 0;
        public decimal? IndentQty { get; set; } = 0;
        public string PRChildRemarks { get; set; }
        public string IndentChildRemarks { get; set; }
        public int ReceiveChildID { get; set; }
        public int StoreID { get; set; }
        public int UnitID { get; set; }
        public int ReceiveMasterID { get; set; }
        public int IndentMasterID { get; set; }
        public int IndentChildID { get; set; }
        public int ProductID { get; set; }
        public int? UOMID { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpireDate { get; set; }
        public int? SectionID { get; set; }
        public int? LocationID { get; set; }
        public int? RackID { get; set; }
        public decimal? ReceiveQty { get; set; }
        public decimal? RejectionQty { get; set; }
        public decimal? DamageQty { get; set; }
        public string ChildsRemarks { get; set; }
        public int? QCStatus { get; set; }
        public int? QCBy { get; set; }
        public DateTime? QCDate { get; set; }
        public string QCRemarks { get; set; }
        public bool? SendForMRR { get; set; } = false;
        public string MRR_Number { get; set; }        
        public int RowSLNo { get; set; } = 0;
        public int RowSLNoNew { get; set; } = 0;
        public int IsDeleted { get; set; } = 0;

    }
}

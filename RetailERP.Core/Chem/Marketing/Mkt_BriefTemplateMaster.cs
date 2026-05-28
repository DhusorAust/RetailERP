using RetailERP.Core.Common;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplateMaster : BaseEntity
    {
		public int BTMasterID { get; set; }
		public string ProjectName { get; set; }
		public int BrandID { get; set; }
		public int ProjectLeaderBy { get; set; }
		public DateTime CompletionDate { get; set; }
		public int ProductID { get; set; }
		public int OrderQuantity { get; set; }
		public decimal TargetPerQtyCost { get; set; }
		public int ServiceOptionID { get; set; }
		public decimal MaxBulkPrice { get; set; }
		public int ProductTypeID { get; set; }
		public string ProductPerformance { get; set; }
		public int TextureID { get; set; }
		public int ColorID { get; set; }
		public int FragranceID { get; set; }
		public string ProductClaims { get; set; }
		public string AdditionalComments { get; set; }
		public string Certification { get; set; }
		public int IsActive { get; set; }
		public int CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public int UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
		public int Approved { get; set; }
		public int ApprovedBy { get; set; }
		public DateTime ApprovedDate { get; set; }

		#region Additional Columns

		[Write(false)]
        public List<DropDownList> BrandList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ProductList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ProductTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ServiceOptionList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> TextureTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> ColorIList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> FragranceList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> GenderList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> CountryList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> IngredientList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> EmployeeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> UOMList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> PakagingLevelList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> PackagingTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> MaterialTypeList { get; set; } = new List<DropDownList>();
        [Write(false)]
        public List<DropDownList> FinishTypeList { get; set; } = new List<DropDownList>();

        [Write(false)]
        public List<Mkt_BriefTemplateReference> ReferenceChilds { get; set; } = new ();
        [Write(false)]
        public List<Mkt_BriefTemplateIngredient> IngredientChilds { get; set; } = new ();
        [Write(false)]
        public List<Mkt_BriefTemplateComplaince> ComplainceChilds { get; set; } = new ();
        [Write(false)]
        public List<Mkt_BriefTemplateProductTest> ProductTestChilds { get; set; } = new ();
        [Write(false)]
        public List<Mkt_BriefTemplateConsumer> ConsumerChilds { get; set; } = new ();
        [Write(false)]
        public List<Mkt_BriefTemplatePackaging> PackagingChilds { get; set; } = new ();
        [Write(false)]
        public string BrandName { get; set; }
        [Write(false)]
        public string ProductName { get; set; }
        [Write(false)]
        public string ProductTypeName { get; set; }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetailERP.Core.Chem.Marketing
{
    public class Mkt_BriefTemplatePackaging
    {
        public int BTPackagingID { get; set; }
        public int BTMasterID { get; set; }
        public int PakagingLevelID { get; set; }
        public int PackagingTypeID { get; set; }
        public string PackagingStyle { get; set; }
        public int PackageSize { get; set; }
        public int UOMID { get; set; }
        public int MaterialID { get; set; }
        public int FinishID { get; set; }
        public int PackagingColorID { get; set; }
        public int PrintColorID { get; set; }
        public string LabelDescription { get; set; }
        public string DesignDescription { get; set; }
        public string Certification { get; set; }
        public string Remarks { get; set; }
        public string FilePath { get; set; }
        public int RowSlNo { get; set; }
        public int IsDeleted { get; set; }

    }
    public class TT_Mkt_BriefTemplatePackaging
    {
        public int BTPackagingID { get; set; }
        public int BTMasterID { get; set; }
        public int PakagingLevelID { get; set; }
        public int PackagingTypeID { get; set; }
        public string PackagingStyle { get; set; }
        public int PackageSize { get; set; }
        public int UOMID { get; set; }
        public int MaterialID { get; set; }
        public int FinishID { get; set; }
        public int PackagingColorID { get; set; }
        public int PrintColorID { get; set; }
        public string LabelDescription { get; set; }
        public string DesignDescription { get; set; }
        public string Certification { get; set; }
        public string Remarks { get; set; }
        public string FilePath { get; set; }
        public int RowSlNo { get; set; }
        public int IsDeleted { get; set; }

    }


}

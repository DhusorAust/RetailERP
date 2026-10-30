using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using RetailERP.Core.Chem.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Composition;
using System.IO;
using RetailERP.Web.Models;

namespace RetailERP.Web.Controllers.rpt
{
    public class RptAccountsController : Controller
    {
        IWebHostEnvironment _env;
        int unitID = 0;
        public RptAccountsController(IWebHostEnvironment env) {
            _env = env;  
        }
        public int GetUnitID()
        {
            ALLMENU allMenu = new();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            unitID = allMenu.UserList[0].UnitID == 0 ? 2 : allMenu.UserList[0].UnitID;
            return unitID;
        }





    }
}

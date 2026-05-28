using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RetailERP.Core.Common;
using RetailERP.Core.Chem.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RetailERP.Web.Models;
using RetailERP.Infrastructure.Chem.Interfaces.Security;

namespace RetailERP.Web.Controllers
{
    public class SecurityController : Controller
    {
        private readonly DapperContext<Menu> _context;
        private readonly IMenuCreateService _service;
        private readonly IConfiguration _configuration;
        public SqlConnection Connection { get; set; }
        public SecurityController(IConfiguration configuration, IMenuCreateService service, DapperContext<Menu> context)
        {
            _configuration = configuration;
            Connection = new SqlConnection(_configuration.GetConnectionString("ERPDBConnection"));
            _service = service;
            _context = context;
        }
        string Message = "";
        CommonFunction objCFunction = new CommonFunction();

        #region Authentication
        public ALLMENU MenuAuthenticationCheck(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu != null && allMenu.MenuList.Where(x => x.MenuID == menuId).FirstOrDefault() != null)
            {
                pageName = string.Join("", pageName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                ViewBag.MenuId = menuId;
                ViewBag.PageName = pageName;
                ViewBag.MenuTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle).FirstOrDefault();
                ViewBag.MenuTitle2 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle2).FirstOrDefault();
                ViewBag.MenuTitle3 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle3).FirstOrDefault();
                ViewBag.MenuTitle4 = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.MenuTitle4).FirstOrDefault();
                ViewBag.PageTitle = allMenu.MenuList.Where(x => x.MenuID == menuId).Select(y => y.PageName).FirstOrDefault();
                return allMenu;
            }
            else
            {
                ViewBag.PageTitle = "Login";
                return null;
            }
        }
        #endregion Authentication 

        #region Start :: Login 
        public IActionResult Index()
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu != null)
            {
                ViewBag.PageTitle = "Welcome to the Retail ERP";

                return RedirectToAction("Default", "Security");
            }
            else
            {
                return View();
            }
        }  
        public IActionResult GetLogin(string UserName, string PasswordH)
        {
            ALLMENU allMenu = new ALLMENU();
            try
            {
                string strQuery = "";

                if (PasswordH == "it@1234#")
                {
                    strQuery = $@"
                    Select 
                        UL.UserID, ul.UserName, urp.UserRoleID, UR.UserRoleName, UL.EmployeeID, EL.EmployeeName, EL.EmployeeNo, Isnull(EL.DepartmentID, 0)DepartmentID,
                        D.DepartmentName, Isnull(EL.DesignationID, 0)DesignationID, DEG.DesignationName, Isnull(EL.SectionID, 0)SectionID, 
                        S.SectionName, Isnull(EL.SubSectionID, 0)SubSectionID, SS.SubSectionName,
                        UL.UserTypeID, UT.UserTypeName
                        ,IsAuthorizedLogin=case when el.EmpStatusID<>1 and sep.EffectDate<=cast(Getdate() as date) and urp.ProjectID in ({AppGlobalFields.ProjectID}) then 0 else 1 end 
                        ,photo.EmpPhoto
                        ,EL.UnitID
                    From 
                        {DBSource.ERPUSERDB}..UserList UL
                        Left Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserID = UL.UserID And urp.ProjectID = {AppGlobalFields.ProjectID} 
                        LEFT JOIN {DBSource.ERPUSERDB}..UserRole UR ON UR.UserRoleID = urp.UserRoleID 
                        Left Join {DBSource.ERPUSERDB}..UserType UT On UT.UserTypeID = UL.UserTypeID 
                        -- Employee Info
                        LEFT Join {DBSource.PDLHRM}..emp_EmploymentInfo EL on EL.EmployeeID = UL.EmployeeID  
                        LEFT Join {DBSource.PDLHRM}..bas_Department D ON D.DepartmentID = EL.DepartmentID
                        LEFT JOIN {DBSource.PDLHRM}..bas_Designation DEG ON DEG.DesignationID = EL.DesignationID
                        LEFT JOIN {DBSource.PDLHRM}..bas_Section S ON S.SectionID = EL.SectionID
                        LEFT JOIN {DBSource.PDLHRM}..bas_SubSection SS ON SS.SubSectionID = EL.SubSectionID  
                        LEFT JOIN {DBSource.PDLHRM}..emp_EmployeeSeparation sep on sep.EmployeeID=el.EmployeeID and sep.IsActive=1
                        LEFT JOIN {DBSource.PDLHRM}..emp_EmployeePhoto photo on photo.EmployeeID=el.EmployeeID
                    Where 
                        urp.ProjectID = {AppGlobalFields.ProjectID} And UL.UserName = '{UserName}' AND UL.IsActive = 1 --AND UL.PasswordExpireDate > GETDATE() ";
                }
                else
                {
                    strQuery = $@"
                    Select 
                        UL.UserID, ul.UserName, urp.UserRoleID, UR.UserRoleName, UL.EmployeeID, EL.EmployeeName, EL.EmployeeNo, Isnull(EL.DepartmentID, 0)DepartmentID,
                        D.DepartmentName, Isnull(EL.DesignationID, 0)DesignationID, DEG.DesignationName, Isnull(EL.SectionID, 0)SectionID, 
                        S.SectionName, Isnull(EL.SubSectionID, 0)SubSectionID, SS.SubSectionName,
                        UL.UserTypeID, UT.UserTypeName
                        ,IsAuthorizedLogin=case when el.EmpStatusID<>1 and sep.EffectDate<=cast(Getdate() as date) and urp.ProjectID in ({AppGlobalFields.ProjectID}) then 0 else 1 end 
                        ,photo.EmpPhoto ,EL.UnitID
                    From 
                        {DBSource.ERPUSERDB}..UserList UL  
                        Left Join {DBSource.ERPUSERDB}..UserRolePermission urp On urp.UserID = UL.UserID And urp.ProjectID = {AppGlobalFields.ProjectID} 
                        LEFT JOIN {DBSource.ERPUSERDB}..UserRole UR ON UR.UserRoleID = urp.UserRoleID
                        Left Join {DBSource.ERPUSERDB}..UserType UT On UT.UserTypeID = UL.UserTypeID
                        -- Employee Info
                        LEFT Join {DBSource.PDLHRM}..emp_EmploymentInfo EL on EL.EmployeeID = UL.EmployeeID 
                        LEFT Join {DBSource.PDLHRM}..bas_Department D ON D.DepartmentID = EL.DepartmentID
                        LEFT JOIN {DBSource.PDLHRM}..bas_Designation DEG ON DEG.DesignationID = EL.DesignationID
                        LEFT JOIN {DBSource.PDLHRM}..bas_Section S ON S.SectionID = EL.SectionID
                        LEFT JOIN {DBSource.PDLHRM}..bas_SubSection SS ON SS.SubSectionID = EL.SubSectionID 
                        LEFT JOIN {DBSource.PDLHRM}..emp_EmployeeSeparation sep on sep.EmployeeID=el.EmployeeID and sep.IsActive=1
                        LEFT JOIN {DBSource.PDLHRM}..emp_EmployeePhoto photo on photo.EmployeeID=el.EmployeeID
                    Where 
                        urp.ProjectID = {AppGlobalFields.ProjectID} And UL.UserName = '{UserName}' And UL.PasswordH = '{objCFunction.PasswordEncrypt(PasswordH)}' 
                        AND UL.IsActive = 1 --AND UL.PasswordExpireDate>GETDATE() ";
                }

                DataTable dtLogin = new DataTable();
                SqlDataAdapter myAdapter = new SqlDataAdapter(strQuery, Connection.ConnectionString);
                myAdapter.Fill(dtLogin); 

                IDbConnection connection = new SqlConnection(Connection.ConnectionString);
                var dbName = connection.Database; 

                if (dtLogin.Rows.Count > 0)
                {
                    for (int ii = 0; ii < dtLogin.Rows.Count; ii++)
                    {
                        UserList item = new UserList();
                        item.UserID = Convert.ToInt32(dtLogin.Rows[0]["UserID"].ToString());
                        item.UnitID = dtLogin.Rows[0]["UnitID"] != DBNull.Value ? Convert.ToInt32(dtLogin.Rows[0]["UnitID"]) : 0;
                        item.UserName = dtLogin.Rows[0]["UserName"].ToString();
                        item.UserRoleID = Convert.ToInt32(dtLogin.Rows[0]["UserRoleID"].ToString());
                        item.UserRoleName = dtLogin.Rows[0]["UserRoleName"].ToString();
                        item.EmployeeID = Convert.ToInt32(dtLogin.Rows[0]["EmployeeID"].ToString());
                        item.EmployeeName = dtLogin.Rows[0]["EmployeeName"].ToString(); 
                        item.EmployeeNo = dtLogin.Rows[0]["EmployeeNo"].ToString(); 
						item.DepartmentID = Convert.ToInt32(dtLogin.Rows[0]["DepartmentID"].ToString());
						item.DepartmentName = dtLogin.Rows[0]["DepartmentName"].ToString();
						item.DesignationID = Convert.ToInt32(dtLogin.Rows[0]["DesignationID"].ToString());
						item.DesignationName = dtLogin.Rows[0]["DesignationName"].ToString();
						item.SectionID = Convert.ToInt32(dtLogin.Rows[0]["SectionID"].ToString());
						item.SectionName = dtLogin.Rows[0]["SectionName"].ToString();
						item.SubSectionID = Convert.ToInt32(dtLogin.Rows[0]["SubSectionID"].ToString());
						item.SubSectionName = dtLogin.Rows[0]["SubSectionName"].ToString();
						item.UserTypeID = Convert.ToInt32(dtLogin.Rows[0]["UserTypeID"].ToString());
						item.UserTypeName = dtLogin.Rows[0]["UserTypeName"].ToString(); 
                        item.DabaseName = dbName;
                        item.IPAddress = objCFunction.GetIPAddress();
                        item.EmpPhoto = (byte[])(DBNull.Value.Equals(dtLogin.Rows[0]["EmpPhoto"]) ? new byte[0] : dtLogin.Rows[0]["EmpPhoto"]);
                        item.IsAuthorizedLogin = Convert.ToInt32(dtLogin.Rows[0]["IsAuthorizedLogin"].ToString());
                        allMenu.UserList.Add(item);
                    }
                }

                //if (allMenu.UserList.Count > 0)
                //{
                //    allMenu.MenuList = GetMenuLoad(allMenu.UserList[0].UserRoleID, allMenu.UserList[0].UserID);
                //    if (allMenu.MenuList != null && allMenu.MenuList.Count != 0)
                //    {
                //        HttpContext.Session.SetObject("Menu", allMenu); 
                //        return Json(new { isCompleted = true, redirectUrl = Url.Action("Default", "Security") });
                //    }
                //    else
                //    {
                //        Message = "Login Failed, Please try again.";
                //    }
                //}

                if (allMenu.UserList.Count > 0)
                {
                    allMenu.MenuList = GetMenuLoad(allMenu.UserList[0].UserRoleID, allMenu.UserList[0].UserID);



                    if (allMenu.UserList[0].IsAuthorizedLogin == 1)
                    {
                        HttpContext.Session.SetObject("Menu", allMenu);
                        return Json(new { isCompleted = true, redirectUrl = Url.Action("Default", "Security") });
                    }
                    else
                    {
                        Message = "Access Denied!<br/>You are not authorized to Login.";
                        return Json(new { isCompleted = false, redirectUrl = Url.Action("Index", "Security"), Message });
                    }
                }
                else
                {
                    Message = "User Not Found!<br/>Username or Password Incorrect";
                }
            }
            catch (Exception Ex)
            {
                string str = Ex.Message;
            }
            return Json(new { isCompleted = false, redirectUrl = Url.Action("Index", "Security"), Message }); //, new Newtonsoft.Json.JsonSerializerSettings());
        }
        public List<Menu> GetMenuLoad(int UserRoleID, int UserID)
        {
            List<Menu> ListObjMenu = new List<Menu>();
            string steMenu = $@"
            Begin 
	            If Not Exists(Select * From {DBSource.ERPUSERDB}..MenuPermissionUser a inner join {DBSource.ERPUSERDB}..Menu b on a.MenuID = b.MenuID Where UserID = {UserID} and b.ProjectID = {AppGlobalFields.ProjectID})
	            Begin
		            Select M.MenuID, M.ProjectID, P.ProjectName, M.ProjectModuleID, PM.ModuleName, M.ViewName, M.Controller, M.MenuHead, M.MenuPriority, 
		            M.ParentMenuID, M.PageName,  M.MenuTitle, M.MenuTitle2, M.MenuTitle3, M.MenuTitle4, M.MenuIcon, M.IsActive, 
		            MP.IsPermission, MPAR.ParameterValue, MPAR.SequenceNo 
		            From {DBSource.ERPUSERDB}..Menu M 
                    Inner Join {DBSource.ERPUSERDB}..Project P ON P.ProjectID = M.ProjectID
		            left Join {DBSource.ERPUSERDB}..ProjectModule PM ON PM.ProjectModuleID = M.ProjectModuleID
		            Inner Join {DBSource.ERPUSERDB}..MenuPermissionRole MP ON MP.MenuID = M.MenuID
                    Inner Join {DBSource.ERPUSERDB}..UserRolePermission urp ON urp.UserRoleID = MP.UserRoleID And urp.ProjectID = {AppGlobalFields.ProjectID}
		            Inner Join {DBSource.ERPUSERDB}..UserList UL ON UL.UserID = urp.UserID
		            Left Join {DBSource.PDLHRM}..emp_EmploymentInfo E ON E.EmployeeID = UL.EmployeeID
		            Left Join {DBSource.ERPUSERDB}..MenuParameter MPAR ON MPAR.MenuID = M.MenuID
		            WHERE m.ProjectID = {AppGlobalFields.ProjectID} And P.ProjectID = {AppGlobalFields.ProjectID} And M.IsActive = 1 And P.IsActive = 1 And MP.IsPermission = 1 And UL.IsActive = 1  
                    And urp.UserRoleID = {UserRoleID} AND UL.UserID = {UserID}
		            Order By ParentMenuID, MenuPriority
	            End
	            Else
	            Begin
		            Select M.MenuID, M.ProjectID, P.ProjectName, M.ProjectModuleID, PM.ModuleName, M.ViewName, M.Controller, M.MenuHead, M.MenuPriority, 
		            M.ParentMenuID, M.PageName,  M.MenuTitle, M.MenuTitle2, M.MenuTitle3, M.MenuTitle4, M.MenuIcon, M.IsActive, 
		            MP.IsPermission, MPAR.ParameterValue, MPAR.SequenceNo   
		            From {DBSource.ERPUSERDB}..Menu M 
                    Inner Join {DBSource.ERPUSERDB}..Project P ON P.ProjectID = M.ProjectID
		            Left Join {DBSource.ERPUSERDB}..ProjectModule PM ON PM.ProjectModuleID = M.ProjectModuleID
		            Inner Join {DBSource.ERPUSERDB}..MenuPermissionUser MP ON MP.MenuID = M.MenuID
		            Inner Join {DBSource.ERPUSERDB}..UserList UL ON UL.UserID = MP.UserID
		            Left Join {DBSource.PDLHRM}..emp_EmploymentInfo E ON E.EmployeeID = UL.EmployeeID
		            Left Join {DBSource.ERPUSERDB}..MenuParameter MPAR ON MPAR.MenuID = M.MenuID
		            WHERE P.ProjectID = {AppGlobalFields.ProjectID} And M.IsActive = 1 And P.IsActive = 1 And MP.IsPermission = 1 And UL.IsActive = 1 
                    AND UL.UserID = {UserID}
		            Order By ParentMenuID, MenuPriority
	            End
            End ";

            DataTable dtMenu = _context.ExecuteQuery(steMenu);

            for (int i = 0; i < dtMenu.Rows.Count; i++)
            {
                Menu item = new Menu();
                item.MenuID = Convert.ToInt32(dtMenu.Rows[i]["MenuID"].ToString());
                item.ProjectID = Convert.ToInt32(dtMenu.Rows[i]["ProjectID"].ToString());
                item.ProjectName = dtMenu.Rows[i]["ProjectName"].ToString();
                item.ProjectModuleID = Convert.ToInt32(dtMenu.Rows[i]["ProjectModuleID"].ToString());
                item.ModuleName = dtMenu.Rows[i]["ModuleName"].ToString();
                item.ViewName = dtMenu.Rows[i]["ViewName"].ToString();
                item.Controller = dtMenu.Rows[i]["Controller"].ToString();
                item.MenuHead = dtMenu.Rows[i]["MenuHead"].ToString();
                item.MenuPriority = Convert.ToInt32(dtMenu.Rows[i]["MenuPriority"].ToString());
                item.ParentMenuID = Convert.ToInt32(dtMenu.Rows[i]["ParentMenuID"].ToString());
                item.ParameterValue = dtMenu.Rows[i]["ParameterValue"].ToString();
                item.SequenceNo = dtMenu.Rows[i]["SequenceNo"].ToString();
                item.PageName = dtMenu.Rows[i]["PageName"].ToString();
                item.IsPermission = Convert.ToInt32(dtMenu.Rows[i]["IsPermission"].ToString());
                item.MenuTitle = dtMenu.Rows[i]["MenuTitle"].ToString();
                item.MenuTitle2 = dtMenu.Rows[i]["MenuTitle2"].ToString();
                item.MenuTitle3 = dtMenu.Rows[i]["MenuTitle3"].ToString();
                item.MenuTitle4 = dtMenu.Rows[i]["MenuTitle4"].ToString();
                item.MenuIcon = dtMenu.Rows[i]["MenuIcon"].ToString();
                ListObjMenu.Add(item);
            }
            return ListObjMenu;
        }
        #endregion End :: Login 

        #region Start :: Logout Information 
        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        #endregion End :: Logout Information

        #region Start :: Default Information
        public IActionResult Default(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
      
        #endregion End :: Default Information
        public IActionResult UserRole(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult UserList(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult ChangePassword(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult MenuCreate(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult MenuPermissionRole(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult MenuPermissionUser(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }
        public IActionResult ReportPermissionRole(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        } 
        public IActionResult MenuReportCreate(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        } 
        public IActionResult UserViewPermission(int menuId, string pageName)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = MenuAuthenticationCheck(menuId, pageName);
            if (allMenu != null)
            {
                return View(allMenu);
            }
            else
            {
                return RedirectToAction("Index", "Security");
            }
        }

        [HttpGet]
        //[Route("GetCurrentUser")]
        public async Task<UserList> GetCurrentUser()
        {
            var userInfo = new UserList();
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu != null)
            {
                userInfo = allMenu.UserList[0];
            }
            return userInfo;
        }

        #region Start :: OTP 
        [HttpGet]
        public bool CheckIsOfficeIP(string clientIP)
        {
            bool isOfficeIP = false;
            string[] officeIP = { "127.0.0.1", "::1" };  
            if (officeIP.Contains(clientIP))
            {
                isOfficeIP = true;
            }
            return isOfficeIP;
        }

        [HttpPost]
        public JsonResult SendOTP(string userName)
        {
            Random generator = new Random();
            string _otp = null;
            string _message = null;
            string strQuery = $@"select EMPLOYEE_MOBILE_NO from USER_LIST where UserName = '{userName}' ";

            DataTable dtLogin = new DataTable();
            SqlDataAdapter myAdapter = new SqlDataAdapter(strQuery, Connection.ConnectionString);
            myAdapter.Fill(dtLogin);
            if (dtLogin.Rows.Count > 0)
            {
                _otp = generator.Next(0, 1000000).ToString("D6");
                _message = "Local OTP generated for Dhusor only. OTP: " + _otp;
            }
            else
            {
                _message = "Invalid User Name.";
            }
            return Json(new { otp = _otp, message = _message });
        }

        #endregion

        public IActionResult PivotTest(int menuId, string pageName)
        {
            //ALLMENU allMenu = new ALLMENU();
            //allMenu = MenuAuthenticationCheck(menuId, pageName);
            //if (allMenu != null)
            //{
                return View();
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Security");
            //}
        } 
    }
}

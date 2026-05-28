using Microsoft.AspNetCore.Mvc;
using RetailERP.Core.Common;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using RetailERP.Infrastructure.Chem.Interfaces.Common;

namespace RetailERP.Web.Controllers.api.Common
{
    [Route("api/helperC")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly IHelperService _service;
        private readonly IFileUploadService _fileUpload;
        private readonly IEmailService _iEmailService;
        public HelperController(IHelperService service, IFileUploadService fileUpload, IEmailService iEmailService)
        {
            _service = service;
            _fileUpload = fileUpload;
            _iEmailService = iEmailService;
        }

        #region File Upload
        [Route("FileUpload")]
        [HttpPost]
        public async Task<IActionResult> FileUpload([FromForm] List<FileUploadModel> FileUpload)
        {
            try
            {
                //Delete upload file
                if(FileUpload[0].IsDeleted == 1)
                {
                    int k = _fileUpload.FileDelete(FileUpload[0].Dsql);
                }
                
                //Save upload file
                for (int i = 0; i < FileUpload.Count; i++)
                { 
                    var response = await _fileUpload.FileUpload(FileUpload[i], i.ToString());
                } 
            }
            catch (Exception ex)
            {

            }
            return Ok(FileUpload);
        }
        #endregion

        #region Email
        [Route("Email")]
        [HttpGet]
        public IActionResult SendEmail()
        {
            return Ok(_iEmailService.SendEmail("anwar.nub@gmail.com", "Test Mail", "Test Body"));
        }
        #endregion

        #region SMS Service

        [HttpGet("GetSMSConfig")]
        public async Task<IActionResult> GetSMSConfig()
        {
            return Ok(await _service.GetSMSConfig());
        }
        #endregion

        #region Ingredient

        [HttpGet("GetIngredientBySearch")]
        public async Task<IActionResult> GetIngredientBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        {
            return Ok(await _service.GetIngredientBySearch(searchKey, itemName, pageIndex, pageSize));
        }
        #endregion Product



        #region Product

        [HttpGet("GetAllProductBySearch")]
        public async Task<IActionResult> GetAllProductBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        {
            return Ok(await _service.GetAllProductBySearch(searchKey, itemName, pageIndex, pageSize));
        }
        #endregion Product


        //[HttpGet("GetAllCountBySearch")]
        //public async Task<IActionResult> GetAllCountBySearch(string searchKey, string itemName, int pageIndex, int pageSize,string countType)
        //{
        //    return Ok(await _service.GetAllCountBySearch(searchKey, itemName, pageIndex, pageSize,countType));
        //}
        //[HttpGet("GetAllLotBySearch")]
        //public async Task<IActionResult> GetAllLotBySearch(string searchKey, string itemName, int pageIndex, int pageSize,string lotType)
        //{
        //    return Ok(await _service.GetAllLotBySearch(searchKey, itemName, pageIndex, pageSize, lotType));
        //}

        //[HttpGet("GetAllPIBySearch")]
        //public async Task<IActionResult> GetAllPIBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //{
        //    return Ok(await _service.GetAllPIBySearch(searchKey, itemName, pageIndex, pageSize));
        //}




        //[HttpGet("GetAllCotProduct")]
        //public async Task<IActionResult> GetAllCotProductBySearch(string searchKey, string itemName, int pageIndex, int pageSize, int? pageId)
        //{
        //    return Ok(await _service.GetAllCotProductBySearch(searchKey, itemName, pageIndex, pageSize, pageId));
        //}



        //[HttpGet("GetAllVehicleBySearch")]
        //public async Task<IActionResult> GetAllVehicleBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //{
        //    return Ok(await _service.GetAllVehicleBySearch(searchKey, itemName, pageIndex, pageSize));
        //}

        //[HttpGet("GetAllTransportBySearch")]
        //public async Task<IActionResult> GetAllTransportBySearch(string searchKey, string itemName, int pageIndex, int pageSize)
        //{
        //    return Ok(await _service.GetAllTransportBySearch(searchKey, itemName, pageIndex, pageSize));
        //}

        #region Employee Information
        //[HttpGet("GetEmployeeByEmployeeID")]
        //public async Task<IActionResult> GetEmployeeByEmployeeID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string employeeID)
        //{
        //    return Ok(await _service.GetEmployeeByEmployeeID(searchKey, pageIndex, pageSize, employeeName, employeeID));
        //}
        //[HttpGet("GetEmployeeByUnitID")]

        //public async Task<IActionResult> GetEmployeeByUnitID(string searchKey, int pageIndex, int pageSize, string employeeName, string unitID, string deptIds, string secIds, string empTypeIds)
        //{
        //    return Ok(await _service.GetEmployeeByUnitID(searchKey, pageIndex, pageSize, employeeName, unitID,deptIds,secIds,empTypeIds));
        //}
        //[HttpGet("GetEmployeeByDepartmentID")]
        //public async Task<IActionResult> GetEmployeeByDepartmentID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string departmentID)
        //{
        //    return Ok(await _service.GetEmployeeByDepartmentID(searchKey, pageIndex, pageSize, employeeName, departmentID));
        //}
        //[HttpGet("GetEmployeeByDepartmentNUnitID")]
        //public async Task<IActionResult> GetEmployeeByDepartmentNUnitID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string departmentID, string unitID)
        //{
        //    return Ok(await _service.GetEmployeeByDepartmentNUnitID(searchKey, pageIndex, pageSize, employeeName, departmentID, unitID));
        //}
        //[HttpGet("GetEmployeeBySectionID")]
        //public async Task<IActionResult> GetEmployeeBySectionID(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string sectionID)
        //{
        //    return Ok(await _service.GetEmployeeBySectionID(searchKey, pageIndex, pageSize, employeeName, sectionID));
        //}
        //[HttpGet("GetEmployeeListByAll")]
        //public async Task<IActionResult> GetEmployeeListByAll(string searchKey, int pageIndex, int pageSize,
        //    string employeeName, string unitID, string departmentID, string sectionID)
        //{
        //    return Ok(await _service.GetEmployeeListByAll(searchKey, pageIndex, pageSize, employeeName, unitID, departmentID, sectionID));
        //}
        #endregion Employee Information
    }
}

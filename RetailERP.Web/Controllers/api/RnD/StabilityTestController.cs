using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.RnD;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using RetailERP.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Web.Controllers.api.RnD
{
    [Route("api/StabilityTest")]
    [ApiController]
    public class StabilityTestController : ControllerBase
    {
        private readonly IStabilityTestService _service;
        private readonly IWebHostEnvironment _env;
        public StabilityTestController(IStabilityTestService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }
        [HttpPost]
        [Route("list/{status}")]
        public async Task<IActionResult> GetList([FromBody] GridOptions options, Status status)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            var records = await _service.GetList(options, status);
            return Ok(records);
        }
        [HttpGet]
        [Route("new")]
        public async Task<IActionResult> GetNew()
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetNew());
        }
        [HttpGet]
        [Route("edit")]
        public async Task<IActionResult> GetEdit(int id)
        {
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            return Ok(await _service.GetEdit(id));
        }

        [Route("save")]
        [HttpPost]
        public async Task<IActionResult> Save([FromForm] RnD_StabilityTestMaster model)
        {
            ALLMENU allMenu = new ALLMENU();
            allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            _service.User = HttpContext.Session.GetObject<ALLMENU>("Menu").UserList[0];
            RnD_StabilityTestMaster entity = new ();
            if (allMenu != null)
            {
                entity = model;
                if (entity.StabilityTestMasterID > 0)
                {
                    entity.SaveOption = 2;
                    //if (entity.Approved > 0)
                    //{
                    //    entity.SaveOption = 3;
                    //}
                    //else if (entity.Reject > 0) { entity.SaveOption = 5; }
                }
                else
                {
                    entity.SaveOption = 1;
                }

                entity.UserBy = allMenu.UserList[0].UserID.ToString();
                if (!IsValidTemperatureFiles())
                {
                    entity.ResultId = 0;
                    entity.Message = "Only .jpg and .png image files are allowed.";
                    return Ok(entity);
                }

                var savedEntity = await _service.SaveAsync(entity);
                if (savedEntity.ResultId > 0)
                {
                    var deletedFilePathIds = GetDeletedFilePathIds();
                    if (deletedFilePathIds.Count > 0)
                    {
                        await _service.SoftDeleteFilePathsAsync(deletedFilePathIds);
                    }

                    var filePaths = await SaveTemperatureFiles(savedEntity.ResultId);
                    if (filePaths.Count > 0)
                    {
                        await _service.SaveFilePathsAsync(filePaths);
                    }
                }
                return Ok(savedEntity);
            }
            else
            {
                entity.IsLogin = 1;
                entity.redirectUrl = Url.Action("Index", "Security");
                return Ok(entity);
            }
        }

        private bool IsValidTemperatureFiles()
        {
            var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png"
            };

            foreach (var file in Request.Form.Files)
            {
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                var extension = Path.GetExtension(file.FileName);
                if (!allowedExtensions.Contains(extension))
                {
                    return false;
                }
            }

            return true;
        }

        [Route("file/delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteFile([FromForm] int stabilityTestFilePathId)
        {
            ALLMENU allMenu = HttpContext.Session.GetObject<ALLMENU>("Menu");
            if (allMenu == null)
            {
                return Ok(new RnD_StabilityTestMaster
                {
                    IsLogin = 1,
                    redirectUrl = Url.Action("Index", "Security")
                });
            }

            _service.User = allMenu.UserList[0];
            var deletedRows = await _service.SoftDeleteFilePathsAsync(new List<int> { stabilityTestFilePathId });
            return Ok(new
            {
                ResultId = deletedRows,
                Message = deletedRows > 0 ? "File deleted successfully." : "File delete failed."
            });
        }

        private List<int> GetDeletedFilePathIds()
        {
            return Request.Form["DeletedStabilityTestFilePathIDs"]
                .Where(x => int.TryParse(x, out _))
                .Select(int.Parse)
                .Distinct()
                .ToList();
        }

        private async Task<List<Rnd_StabilityTestFilePath>> SaveTemperatureFiles(int stabilityTestMasterId)
        {
            var result = new List<Rnd_StabilityTestFilePath>();
            var files = Request.Form.Files;
            var temperatures = Request.Form["StabilityTestFileTemperatures"].ToList();

            if (files == null || files.Count == 0)
            {
                return result;
            }

            var webRootPath = _env.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var relativeFolder = "\\Files\\RnD\\StabilityTest\\";
            var uploadFolder = Path.Combine(webRootPath, "Files", "RnD", "StabilityTest");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file == null || file.Length == 0)
                {
                    continue;
                }

                var temperatureText = i < temperatures.Count ? temperatures[i] : "0";
                int.TryParse(temperatureText, out var temperature);

                var originalFileName = Path.GetFileName(file.FileName);
                var extension = Path.GetExtension(originalFileName);
                var fileName = $"{stabilityTestMasterId}_{temperature}_{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}{extension}";
                var fullPath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                result.Add(new Rnd_StabilityTestFilePath
                {
                    StabilityTestMasterID = stabilityTestMasterId,
                    StabilityTestTemperature = temperature,
                    FileName = originalFileName,
                    FilePath = relativeFolder + fileName,
                    IsActive = 1
                });
            }

            return result;
        }
    }
}

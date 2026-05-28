using RetailERP.Core.Chem.ChemicalStore;
using RetailERP.Core.Chem.Marketing;
using RetailERP.Core.Chem.RnD;
using RetailERP.Core.Chem.Security;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager; 
using RetailERP.Infrastructure.Chem.Interfaces.Marketing;
using RetailERP.Infrastructure.Chem.Interfaces.RnD;
using Dapper;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.RnD
{   
    public class StabilityTestService : IStabilityTestService
    {
        private readonly DapperContext<RnD_StabilityTestMaster> _context;
        private readonly SqlConnection _connection;
        public UserList User { get; set; }
        public StabilityTestService(DapperContext<RnD_StabilityTestMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }
        private string CommonQuery = $@"            
             Select FormulationMasterID As id, FormulationCode As [text] From rnd_FormulationMaster Where IsActive = 1;
            ";
        public async Task<GridEntity<RnD_StabilityTestMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string orderBy = "";
            string condition = "";

            if (status == Status.EDIT)
            {
                condition = $@" And fm.Approved = 0 And fm.Reject = 0 ";
            } 
            else if (status == Status.APPROVED)
            {
                condition = $@" And fm.Approved = 1 And fm.Reject = 0 ";
            }
            else if (status == Status.REJECT)
            {
                condition = $@" And fm.Reject = 1 ";
            }
            else if (status == Status.ALL)
            {
                condition = $@" ";
            }
             
            sql = $@"
                 Select 
                    fm.*, b.BrandName, pm.ProductName, uom.UOM,a.StabilityTestMasterID
   
   
                From rnd_StabilityTestMaster a
	                Inner join rnd_FormulationMaster fm on fm.FormulationMasterID=a.FormulationMasterID
                    Inner Join bas_Brand b On b.BrandID = fm.BrandID
   
                    Inner Join bas_CS_ProductMaster pm On pm.ProductID = fm.ProductID
                    Inner Join bas_UOM uom On uom.UOMID = fm.BatchUOMID
    
                Where fm.IsActive = 1 {condition} "; 
                    
                orderBy = "StabilityTestMasterID Desc";

            return await _context.GetKendoGridDataAsync<RnD_StabilityTestMaster>(options, sql, orderBy, "");
        }
        public async Task<RnD_StabilityTestMaster> GetNew()
        {
            var query = $@"{CommonQuery} ";

            RnD_StabilityTestMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                if (data != null)
                {
                    data.FormulationMasterList = records.Read<DropDownList>().ToList();
                    //data.IngredientList = records.Read<DropDownList>().ToList(); 
                }                
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }
        public async Task<RnD_StabilityTestMaster> GetEdit(int id)
        {
            var query = $@" 
                 select * from rnd_StabilityTestMaster where StabilityTestMasterID={id} and IsActive=1
                 select * from rnd_StabilityTestChild where StabilityTestMasterID={id} and IsActive=1
                select distinct StabilityTestMasterID,StabilityTestTemperature from rnd_StabilityTestChild where StabilityTestMasterID={id} and IsActive=1
                select * from rnd_StabilityTestFilePath where StabilityTestMasterID={id} and IsActive=1


                {CommonQuery} ";

            RnD_StabilityTestMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<RnD_StabilityTestMaster>();
                data.Childs = records.Read<RnD_StabilityTestChild>().ToList();
                data.StabilityTestTemperatures = records.Read<RnD_StabilityTestTemperature>().ToList();
                data.StabilityTestFilePaths = records.Read<Rnd_StabilityTestFilePath>().ToList();
               

                if (data != null)
                {
                    data.FormulationMasterList = records.Read<DropDownList>().ToList();
                   

                    
                } 
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }
        public async Task<RnD_StabilityTestMaster> SaveAsync(RnD_StabilityTestMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                List<TT_RnD_StabilityTestChild> TTList = GetChild(entity.Childs);
                //List<TT_RnD_FormulationMethod> TTListB = GetChildB(entity.ChildsMethod);

                DataSet DetailsData = CommonFunction.ToDataSet(TTList);
                var doDetailsData = DetailsData == null ? null : DetailsData.GetXml();

                //DataSet DetailsDataB = CommonFunction.ToDataSet(TTListB);
                //var doDetailsDataB = DetailsDataB == null ? null : DetailsDataB.GetXml();

                parameters = new DynamicParameters();

                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@dsxmlu1", doDetailsData, DbType.Xml, ParameterDirection.Input);
                //parameters.Add("@dsxmlu2", doDetailsDataB, DbType.Xml, ParameterDirection.Input);

                parameters.Add("@StabilityTestMasterID", entity.StabilityTestMasterID);
                parameters.Add("@FormulationMasterID", entity.FormulationMasterID);
               

              
                parameters.Add("@IsActive", entity.IsActive);


                parameters.Add("@UserBy", entity.UserBy);


                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@Message", dbType: DbType.String, size: -1, direction: ParameterDirection.Output);
                entity.NoofRows = await _context.ExecuteAsync("spSet_rnd_StabilityTest", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");
                entity.Message = parameters.Get<string>("@Message");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = entity.Message+" Save operation failed.";
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.InnerException.ToString();
            }
            finally
            {
                parameters = null;
            }
            return entity;
        } 
        public async Task<int> SaveFilePathsAsync(List<Rnd_StabilityTestFilePath> filePaths)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                return 0;
            }

            string sql = @"
                Insert Into rnd_StabilityTestFilePath
                    (StabilityTestMasterID, StabilityTestTemperature, FileName, FilePath, IsActive)
                Values
                    (@StabilityTestMasterID, @StabilityTestTemperature, @FileName, @FilePath, @IsActive)";

            return await _context.ExecuteAsync(sql, filePaths);
        }
        public async Task<int> SoftDeleteFilePathsAsync(List<int> filePathIds)
        {
            if (filePathIds == null || filePathIds.Count == 0)
            {
                return 0;
            }

            string sql = @"
                Update rnd_StabilityTestFilePath
                Set IsActive = 0
                Where StabilityTestFilePathID In @FilePathIds";

            return await _context.ExecuteAsync(sql, new { FilePathIds = filePathIds });
        }
        public List<TT_RnD_StabilityTestChild> GetChild(List<RnD_StabilityTestChild> Childs)
        {
            var TTList = new List<TT_RnD_StabilityTestChild>();
            foreach (var item in Childs)
            {
                var objItem = new TT_RnD_StabilityTestChild
                {
                    StabilityTestChildID = item.StabilityTestChildID,
                    StabilityTestMasterID = item.StabilityTestMasterID,
                    StabilityTestTemperature = item.StabilityTestTemperature,
                    ParameterName = item.ParameterName,
                    T0 = item.T0,
                    T1 = item.T1,
                    T2 = item.T2,
                    T3 = item.T3,
                    T4 = item.T4,
                    T5 = item.T5,
                    T6 = item.T6
                };
                TTList.Add(objItem);
            }
            
            
            return TTList;
        }
        public List<TT_RnD_StabilityTestTemperature> GetChildB(List<RnD_StabilityTestTemperature> Childs)
        {
            var TTList = new List<TT_RnD_StabilityTestTemperature>();
            foreach (RnD_StabilityTestTemperature item in Childs)
            {
                var objItem = new TT_RnD_StabilityTestTemperature
                {
                    StabilityTestMasterID = item.StabilityTestMasterID,
                    StabilityTestTemperatureID = item.StabilityTestTemperatureID,
                    StabilityTestTemperature = item.StabilityTestTemperature
                };
                TTList.Add(objItem);
            }
            return TTList;
        }
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}

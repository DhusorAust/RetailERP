using RetailERP.Core.Chem.Setting.Store.ChemicalStore;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Setting.Store.ChemicalStore
{
    public class ProductMasterService : IProductMasterService
    {
        private readonly DapperContext<Bas_CS_ProductMaster> _context;
        private readonly SqlConnection _connection;
        public ProductMasterService(DapperContext<Bas_CS_ProductMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        } 
        private string CommonQuery = $@"
            Select ProductCategoryID id,ProductCategoryName text From Bas_CS_ProductCategory Where IsActive = 1;
            Select ProductTypeID id, ProductTypeName text From bas_CS_ProductType Where IsActive = 1;
            Select ItemTypeID id, ItemType text From bas_ItemType Where IsActive = 1; ";
        public async Task<GridEntity<Bas_CS_ProductMaster>> GetList(GridOptions options, Status status, string lotType)
        { 
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "ProductID Desc"; 

            if (status == Status.EDIT)
            {
                condition += " And IsNull(a.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " And IsNull(a.Approved,0) = 1";
            }

            sql = $@"
                Select 
                    a.*, b.ProductCategoryName, c.ProductTypeName, 
                    (Case When a.IsActive = 1 Then 'Active' Else 'Inactive' End)IsActiveStatus  
                From
                    Bas_CS_ProductMaster a
                    Left Join Bas_CS_ProductCategory as b on a.ProductCategoryID = b.ProductCategoryID 
                    Left Join bas_CS_ProductType as c on c.ProductTypeID = a.ProductTypeID 

                {condition};"; 

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_CS_ProductMaster>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }  
        }
        public async Task<Bas_CS_ProductMaster> GetNew()
        {
            var query = $@"{CommonQuery};";
            Bas_CS_ProductMaster data = new Bas_CS_ProductMaster();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = new Bas_CS_ProductMaster();
                data.ProductCategoryList = records.Read<DropDownList>().ToList();
                data.ProductTypeList = records.Read<DropDownList>().ToList();
                data.ItemTypeList = records.Read<DropDownList>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw ex;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open) _connection.Close();
            }
            return data;
        } 
        public async Task<Bas_CS_ProductMaster> GetEdit(int id)
        { 
            var query = $@"
                Select a.* From bas_CS_ProductMaster a Where ProductID = {id};

                {CommonQuery}  ";

            Bas_CS_ProductMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_CS_ProductMaster>();
                data.ProductCategoryList = records.Read<DropDownList>().ToList();
                data.ProductTypeList = records.Read<DropDownList>().ToList();
                data.ItemTypeList = records.Read<DropDownList>().ToList();
            }
            catch (Exception ex)
            {
                data.Message = ex.Message;
                data.ErrNo = 1;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open) _connection.Close();
            }
            return data;
        } 
        public async Task<Bas_CS_ProductMaster> SaveAsync(Bas_CS_ProductMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@ProductID", entity.ProductID);
                parameters.Add("@ProductName", entity.ProductName);
                parameters.Add("@ProductCategoryID", entity.ProductCategoryID);
                parameters.Add("@ProductTypeID", entity.ProductTypeID);
                parameters.Add("@ItemTypeID", entity.ItemTypeID); 
                parameters.Add("@Remarks", entity.Remarks); 
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_CS_ProductMaster", parameters, 30, CommandType.StoredProcedure);
                entity.ResultId = parameters.Get<int>("@IdentityValue");
                entity.ErrorNo = parameters.Get<int>("@ErrNo");

                if (entity.NoofRows > 0 && entity.ResultId > 0)
                {
                    entity.Message = "Save operation successful.";
                }
                else
                {
                    entity.Message = "Save operation failed.";
                }
            }
            catch (Exception ex)
            {
                entity.Message = ex.Message;
            }
            finally
            {
                parameters = null;
            }
            return entity;
        } 
    }
}

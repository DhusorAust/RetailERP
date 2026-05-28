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
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly DapperContext<Bas_CS_ProductCategory> _context;
        private readonly SqlConnection _connection;
        public ProductCategoryService(DapperContext<Bas_CS_ProductCategory> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@" ";
        public async Task<GridEntity<Bas_CS_ProductCategory>> GetList(GridOptions options, Status status, string lotType)
        { 
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "ProductCategoryID Desc"; 

            if (status == Status.EDIT)
            {
                condition += " And IsNull(a.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " And IsNull(a.Approved,0) = 1";
            }

            sql = $@"
                Select a.*, (Case When a.IsActive = 1 Then 'Active' Else 'Inactive' End) IsActiveStatus 
                From bas_CS_ProductCategory a {condition};";  

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_CS_ProductCategory>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }  
        }
        public async Task<Bas_CS_ProductCategory> GetNew()
        {
            var query = $@"{CommonQuery};";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Bas_CS_ProductCategory data = new Bas_CS_ProductCategory();
                return data;
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
        } 
        public async Task<Bas_CS_ProductCategory> GetEdit(int id)
        {

            var query =
            $@"Select * From bas_CS_ProductCategory  where  ProductCategoryID={id};";
            Bas_CS_ProductCategory data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_CS_ProductCategory>();
                
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
        public async Task<Bas_CS_ProductCategory> SaveAsync(Bas_CS_ProductCategory entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@ProductCategoryID", entity.ProductCategoryID);
                parameters.Add("@ProductCategoryName", entity.ProductCategoryName);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_CS_ProductCategory", parameters, 30, CommandType.StoredProcedure);
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

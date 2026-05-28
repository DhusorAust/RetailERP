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
    public class ProductTypeService : IProductTypeService
    {
        private readonly DapperContext<bas_CS_ProductType> _context;
        private readonly SqlConnection _connection;
        public ProductTypeService(DapperContext<bas_CS_ProductType> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@" ";
        public async Task<GridEntity<bas_CS_ProductType>> GetList(GridOptions options, Status status, string lotType)
        { 
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "ProductTypeID desc"; 

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
                From bas_CS_ProductType a {condition};"; 

            try
            {
                return await _context.GetKendoGridDataAsync<bas_CS_ProductType>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }  
        }
        public async Task<bas_CS_ProductType> GetNew()
        {
            var query = $@" ";
            bas_CS_ProductType data = new bas_CS_ProductType();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = new bas_CS_ProductType();            
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
        public async Task<bas_CS_ProductType> GetEdit(int id)
        {
            var query = $@"Select a.* From bas_CS_ProductType a Where ProductTypeID = {id}; ";
            bas_CS_ProductType data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<bas_CS_ProductType>();
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
        public async Task<bas_CS_ProductType> SaveAsync(bas_CS_ProductType entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@ProductTypeID", entity.ProductTypeID);              
                parameters.Add("@ProductTypeName", entity.ProductTypeName);
                parameters.Add("@Remarks", entity.Remarks);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_CS_ProductType", parameters, 30, CommandType.StoredProcedure);
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

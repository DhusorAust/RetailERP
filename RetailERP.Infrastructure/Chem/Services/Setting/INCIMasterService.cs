using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Setting; 
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Setting.Store.ChemicalStore
{
    public class INCIMasterService : IINCIMasterService
    {
        private readonly DapperContext<Bas_INCIMaster> _context;
        private readonly SqlConnection _connection;
        public INCIMasterService(DapperContext<Bas_INCIMaster> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@" ";
        public async Task<GridEntity<Bas_INCIMaster>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "INCIID Desc";

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
                    a.*, (Case When a.IsAllergen = 0 Then 'No' Else 'Yes' End)AllergenStatus,
                    (Case When a.IsActive = 1 Then 'Active' Else 'Inactive' End)IsActiveStatus
                From bas_INCIMaster a {condition}; ";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_INCIMaster>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_INCIMaster> GetNew()
        {
            var query = $@"{CommonQuery};";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Bas_INCIMaster data = new Bas_INCIMaster();
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
        public async Task<Bas_INCIMaster> GetEdit(int id)
        { 
            var query = $@"Select * From bas_INCIMaster Where INCIID = {id};";
            Bas_INCIMaster data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_INCIMaster>(); 
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
        public async Task<Bas_INCIMaster> SaveAsync(Bas_INCIMaster entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@INCIID", entity.INCIID);
                parameters.Add("@INCIName", entity.INCIName);
                parameters.Add("@CASNo", entity.CASNo);
                parameters.Add("@IsAllergen", entity.IsAllergen);
                parameters.Add("@Remarks", entity.Remarks);  
                parameters.Add("@IsActive", entity.IsActive); 
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_INCIMaster", parameters, 30, CommandType.StoredProcedure);
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

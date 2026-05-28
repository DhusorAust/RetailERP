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
    public class SeasonService : ISeasonService
    {
        private readonly DapperContext<Bas_Season> _context;
        private readonly SqlConnection _connection;
        public SeasonService(DapperContext<Bas_Season> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"  ";
        public async Task<GridEntity<Bas_Season>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "SeasonID Desc";

            if (status == Status.EDIT)
            {
                condition += " And IsNull(b.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " And IsNull(b.Approved,0) = 1";
            }

            sql = $@" 
                Select 
                    b.*, (Case When b.IsActive = 1 Then 'Active' Else 'Inactive' End)IsActiveStatus,
                    FORMAT(b.StartMonth, 'MMM-yyyy') StartMonthDate, FORMAT(b.EndMonth, 'MMM-yyyy') EndMonthDate
                From 
                    Bas_Season b  {condition};";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_Season>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_Season> GetNew()
        {
            var query = $@"{CommonQuery};";
            Bas_Season data = new Bas_Season();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
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
        public async Task<Bas_Season> GetEdit(int id)
        { 
            var query = $@"
                Select * From Bas_Season Where SeasonID = {id};
                {CommonQuery};";

            Bas_Season data = new();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_Season>();
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
        public async Task<Bas_Season> SaveAsync(Bas_Season entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@SeasonID", entity.SeasonID);
                parameters.Add("@SeasonName", entity.SeasonName);
                parameters.Add("@StartMonth", entity.StartMonth); 
                parameters.Add("@EndMonth", entity.EndMonth); 
                parameters.Add("@IsActive", entity.IsActive); 
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_Season", parameters, 30, CommandType.StoredProcedure);
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

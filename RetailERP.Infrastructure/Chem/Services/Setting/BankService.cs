using RetailERP.Core.Chem.Setting;
using RetailERP.Core.Common;
using RetailERP.Core.Common.KendoGridManager;
using RetailERP.Infrastructure.Chem.Interfaces.Setting;
using RetailERP.Infrastructure.Chem.Interfaces.Setting.Store.ChemicalStore;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.Setting.Store.ChemicalStore
{
    public class BankService : IBankService
    {
        private readonly DapperContext<Bas_Bank> _context;
        private readonly SqlConnection _connection;
        public BankService(DapperContext<Bas_Bank> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            --Select * From bas_Bank;
        ";
        public async Task<GridEntity<Bas_Bank>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "BankID desc";

            if (status == Status.EDIT)
            {
                condition += " and a.IsActive=1 and IsNull(a.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " and a.IsActive=1  and IsNull(a.Approved,0) = 1";
            }

            sql = $@"Select a.*  From bas_Bank a {condition};";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_Bank>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_Bank> GetNew()
        {
            var query = $@"{CommonQuery};";

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Bas_Bank data = new Bas_Bank();
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
        public async Task<Bas_Bank> GetEdit(int id)
        {

            var query =
            $@"Select * From bas_Bank  where  BankID={id};";
            Bas_Bank data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_Bank>();

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
        public async Task<Bas_Bank> SaveAsync(Bas_Bank entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BankID", entity.BankID);
                parameters.Add("@BankCode", entity.BankCode);
                parameters.Add("@BankName", entity.BankName);
                parameters.Add("@BankShortName", entity.BankShortName);
                parameters.Add("@BankAddress", entity.BankAddress); 
                parameters.Add("@SwiftCode", entity.SwiftCode);
                parameters.Add("@ADCode", entity.ADCode);
                parameters.Add("@IsBeneficiaryBank", entity.IsBeneficiaryBank);
                parameters.Add("@IsAdvisingBank", entity.IsAdvisingBank);
                parameters.Add("@IsNegoBank", entity.IsNegoBank);
                parameters.Add("@IsActive", entity.IsActive); 
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_Bank", parameters, 30, CommandType.StoredProcedure);
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

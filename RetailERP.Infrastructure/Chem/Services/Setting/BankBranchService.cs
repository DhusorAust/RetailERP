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
    public class BankBranchService : IBankBranchService
    {
        private readonly DapperContext<Bas_BankBranch> _context;
        private readonly SqlConnection _connection;
        public BankBranchService(DapperContext<Bas_BankBranch> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = 
        $@"
            -- Bank
            Select CAST(BankID AS varchar) id, BankName [text] From bas_Bank;
        ";
        public async Task<GridEntity<Bas_BankBranch>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "BankBranchID desc";
            if (status == Status.EDIT)
            {
                condition += " and a.IsActive=1 and IsNull(a.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " and a.IsActive=1  and IsNull(a.Approved,0) = 1";
            }

            sql = $@"
                Select a.*,b.BankName,b.BankShortName  From bas_BankBranch a
                Inner Join bas_Bank b On a.BankID = b.BankID
                {condition};
            ";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_BankBranch>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_BankBranch> GetNew()
        {
            var query = $@"{CommonQuery};";
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Bas_BankBranch data = new Bas_BankBranch();
                data.BankList = records.Read<DropDownList>().ToList();
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
        public async Task<Bas_BankBranch> GetEdit(int id)
        {
            var query =
            $@"
                Select * From Bas_BankBranch  where  BankBranchID={id};
                {CommonQuery}
            ";
            Bas_BankBranch data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_BankBranch>();
                data.BankList = records.Read<DropDownList>().ToList();
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
        public async Task<Bas_BankBranch> SaveAsync(Bas_BankBranch entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BankBranchID", entity.BankBranchID);
                parameters.Add("@BankID", entity.BankID);
                parameters.Add("@BranchCode", entity.BranchCode);
                parameters.Add("@BranchName", entity.BranchName);
                parameters.Add("@BranchShortName", entity.BranchShortName);
                parameters.Add("@BranchAddress", entity.BranchAddress);
                parameters.Add("@SwiftCode", entity.SwiftCode);
                parameters.Add("@RoutingNo", entity.RoutingNo);
                parameters.Add("@IsBeneficiaryBankBranch", entity.IsBeneficiaryBankBranch);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync("spSet_Bas_BankBranch", parameters, 30, CommandType.StoredProcedure);
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

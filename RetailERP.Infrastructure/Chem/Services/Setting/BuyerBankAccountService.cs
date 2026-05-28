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
    public class BuyerBankAccountService : IBuyerBankAccountService
    {
        private readonly DapperContext<Bas_BuyerBankAccount> _context;
        private readonly SqlConnection _connection;
        public BuyerBankAccountService(DapperContext<Bas_BuyerBankAccount> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = 
        $@"
            -- BuyerList
            Select CAST(BuyerID AS varchar) id, BuyerName [text] From bas_Buyer
            -- BankList
            Select CAST(BankID AS varchar) id, BankName [text] From bas_Bank
            -- BankBranchList
            Select CAST(BankBranchID AS varchar) id, BranchName [text] From bas_BankBranch
        ";
        public async Task<GridEntity<Bas_BuyerBankAccount>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "BuyerBankAccountID desc";
            if (status == Status.EDIT)
            {
                condition += " and a.IsActive=1 and IsNull(a.Approved,0) = 0";
            }
            else if (status == Status.APPROVED)
            {
                condition += " and a.IsActive=1  and IsNull(a.Approved,0) = 1";
            }

            sql = $@"
                Select a.*,b.BankName,c.BranchName,d.BuyerName  From Bas_BuyerBankAccount a
                Inner Join bas_Bank b on a.BankID = b.BankID
			    Inner Join bas_BankBranch c on a.BankBranchID = c.BankBranchID
				Inner Join bas_Buyer d on a.BuyerID = d.BuyerID
                {condition};
            ";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_BuyerBankAccount>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_BuyerBankAccount> GetNew()
        {
            var query = $@"{CommonQuery};";
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                Bas_BuyerBankAccount data = new Bas_BuyerBankAccount();
                data.BuyerList = records.Read<DropDownList>().ToList();
                data.BankList = records.Read<DropDownList>().ToList();
                data.BankBranchList = records.Read<DropDownList>().ToList();

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
        public async Task<Bas_BuyerBankAccount> GetEdit(int id)
        {
            var query =
            $@"
                Select * From bas_BuyerBankAccount  where  BuyerBankAccountID={id};
                {CommonQuery}
            ";
            Bas_BuyerBankAccount data = new();
            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_BuyerBankAccount>();
                data.BuyerList = records.Read<DropDownList>().ToList();
                data.BankList = records.Read<DropDownList>().ToList();
                data.BankBranchList = records.Read<DropDownList>().ToList();
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
        public async Task<Bas_BuyerBankAccount> SaveAsync(Bas_BuyerBankAccount entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BuyerBankAccountID", entity.BuyerBankAccountID);
                parameters.Add("@BuyerID", entity.BuyerID);
                parameters.Add("@BankAccountName", entity.BankAccountName);
                parameters.Add("@BankAccountNo", entity.BankAccountNo);
                parameters.Add("@BankID", entity.BankID);
                parameters.Add("@BankBranchID", entity.BankBranchID);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                entity.NoofRows = await _context.ExecuteAsync("spSet_Bas_BuyerBankAccount", parameters, 30, CommandType.StoredProcedure);
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

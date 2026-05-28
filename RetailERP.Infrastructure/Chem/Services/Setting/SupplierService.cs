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
    public class SupplierService : ISupplierService
    {
        private readonly DapperContext<Bas_Supplier> _context;
        private readonly SqlConnection _connection;
        public SupplierService(DapperContext<Bas_Supplier> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            Select CountryID As id, CountryName As [text] From bas_Country Where IsActive = 1; ";
        public async Task<GridEntity<Bas_Supplier>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "SupplierID Desc";

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
                    b.*, c.CountryName, (Case When b.IsActive = 1 Then 'Active' Else 'Inactive' End)IsActiveStatus 
                From 
                    bas_Supplier b
                    Left Join bas_Country c On c.CountryID = b.CountryID {condition};";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_Supplier>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_Supplier> GetNew()
        {
            var query = $@"{CommonQuery};";
            Bas_Supplier data = new Bas_Supplier();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data.CountryList = records.Read<DropDownList>().ToList();
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
        public async Task<Bas_Supplier> GetEdit(int id)
        { 
            var query = $@"
                Select * From bas_Supplier Where SupplierID = {id};
                {CommonQuery};";

            Bas_Supplier data = new();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_Supplier>();
                data.CountryList = records.Read<DropDownList>().ToList();
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
        public async Task<Bas_Supplier> SaveAsync(Bas_Supplier entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);

                parameters.Add("@SupplierID", entity.SupplierID);
                parameters.Add("@SupplierName", entity.SupplierName);
                parameters.Add("@ShortSupplierName", entity.ShortSupplierName);
                parameters.Add("@Phone", entity.Phone);
                parameters.Add("@MobileNo", entity.MobileNo);
                parameters.Add("@Email", entity.Email);
                parameters.Add("@Website", entity.Website);
                parameters.Add("@CountryID", entity.CountryID);
                parameters.Add("@OwnerName", entity.OwnerName);
                parameters.Add("@OwnerContactNo", entity.OwnerContactNo);
                parameters.Add("@Address", entity.Address);
                parameters.Add("@ContactPersonName", entity.ContactPersonName);
                parameters.Add("@ContactNo", entity.ContactNo);
                parameters.Add("@TermsNConditions", entity.TermsNConditions);
                parameters.Add("@FilePath", entity.FilePath);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_Supplier", parameters, 30, CommandType.StoredProcedure);
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

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
    public class BrandService : IBrandService
    {
        private readonly DapperContext<Bas_Brand> _context;
        private readonly SqlConnection _connection;
        public BrandService(DapperContext<Bas_Brand> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        private string CommonQuery = $@"
            Select CountryID As id, CountryName As [text] From bas_Country Where IsActive = 1; ";
        public async Task<GridEntity<Bas_Brand>> GetList(GridOptions options, Status status)
        {
            string sql = "";
            string condition = " Where 1 = 1 ";
            string orderBy = "BrandID Desc";

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
                    bas_Brand b
                    Left Join bas_Country c On c.CountryID = b.CountryID {condition};";

            try
            {
                return await _context.GetKendoGridDataAsync<Bas_Brand>(options, sql, orderBy, "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<Bas_Brand> GetNew()
        {
            var query = $@"{CommonQuery};";
            Bas_Brand data = new Bas_Brand();

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
        public async Task<Bas_Brand> GetEdit(int id)
        { 
            var query = $@"
                Select * From bas_Brand Where BrandID = {id};
                {CommonQuery};";

            Bas_Brand data = new();

            try
            {
                await _connection.OpenAsync();
                var records = await _connection.QueryMultipleAsync(query);
                data = await records.ReadFirstOrDefaultAsync<Bas_Brand>();
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
        public async Task<Bas_Brand> SaveAsync(Bas_Brand entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@BrandID", entity.@BrandID);
                parameters.Add("@BrandName", entity.BrandName);
                parameters.Add("@BrandShortName", entity.BrandShortName);
                parameters.Add("@Phone", entity.Phone);
                parameters.Add("@MobileNo", entity.MobileNo); 
                parameters.Add("@Email", entity.Email);
                parameters.Add("@Website", entity.Website);
                parameters.Add("@CountryID", entity.CountryID);
                parameters.Add("@Address", entity.Address);
                parameters.Add("@ContactPersonName", entity.ContactPersonName);
                parameters.Add("@ContactNo", entity.ContactNo);
                parameters.Add("@IsActive", entity.IsActive); 
                parameters.Add("@Approved", entity.Approved);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("spSet_bas_Brand", parameters, 30, CommandType.StoredProcedure);
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

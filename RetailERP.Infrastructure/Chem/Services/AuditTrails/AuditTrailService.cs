using RetailERP.Core.Chem.AuditTrails;
using RetailERP.Core.Common;
using RetailERP.Infrastructure.Chem.Interfaces.AuditTrails;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RetailERP.Infrastructure.Chem.Services.AuditTrails
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly DapperContext<AuditTrailEntity> _context;
        private readonly SqlConnection _connection;
        private readonly CommonFunction _commonFunction = new();
        public AuditTrailService(DapperContext<AuditTrailEntity> context)
        {
            _context = context;
            _connection = _context.Connection;
        }

        public async Task<AuditTrailEntity> SaveAsync(AuditTrailEntity entity)
        {
            var parameters = (dynamic)null;
            try
            {
                parameters = new DynamicParameters();
                parameters.Add("@SaveOption", entity.SaveOption);
                parameters.Add("@AuditTrailID", entity.AuditTrailID);
                parameters.Add("@AuditTrailType", entity.AuditTrailType);
                parameters.Add("@AuditTrailDescription", entity.AuditTrailDescription);
                parameters.Add("@MenuID", entity.MenuID);
                parameters.Add("@ProjectID", Convert.ToInt32(AppGlobalFields.ProjectID));
                parameters.Add("@ViewPage", entity.ViewPage);
                parameters.Add("@IsActive", entity.IsActive);
                parameters.Add("@UserBy", entity.UserBy);
                parameters.Add("@IdentityValue", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
                parameters.Add("@ErrNo", 0, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                entity.NoofRows = await _context.ExecuteAsync("ERPDB..spSet_AuditTrail", parameters, 30, CommandType.StoredProcedure);

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
        public async Task<TransactionUserInfo> GetTransactionUserInfo(string pageName,int id)
        {
            var query = "";
           
            if (pageName == "PIInfo")
            {
                query = $@"	 SELECT 
	             EntryBy=b.UserName
	            ,EditBy=c.UserName
	            ,EntryDate,EditDate
	            ,PIID FROM exp_PIInfoM a
	             left join ERPDB.dbo.UserList b on a.EntryBy=b.UserID
	             left join ERPDB.dbo.UserList c on a.EditBy=c.UserID
	             WHERE PIID={id}";
            }

            try
            {
                var data = await _context.GetSingleDataAsync<TransactionUserInfo>(query);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (_connection.State == ConnectionState.Open) _connection.Close();
            }
        }
    }
}

using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using RetailERP.Core.Common.KendoGridManager;


namespace RetailERP.Core.Common
{
    public class DapperContext<T> : DbContext where T : class
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public SqlConnection Connection { get; set; }
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            Connection = new SqlConnection(_configuration.GetConnectionString("RetailERPConnection"));
        }

        #region Get data using following function 
        public async Task<List<CT>> GetDataAsync<CT>(string query) where CT : class
        {
            try
            {
                await Connection.OpenAsync();
                var records = await Connection.QueryAsync<CT>(query);
                return records.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }
        }
        public async Task<IEnumerable<CT>> GetEnumerbleDataAsync<CT>(string query) where CT : class
        {
            try
            {
                await Connection.OpenAsync();
                var records = await Connection.QueryAsync<CT>(query);
                return records;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }
        }
        public async Task<GridEntity<T>> GetKendoGridDataAsync_SP<CT>(GridOptions options, string conStrName, string procedureName, string callName, string orderBy, DynamicParameters sp_params)
        {
            try
            {
                options.take = options.skip + options.take;
                var filterby = "";
                if (options.filter != null)
                {
                    filterby = options != null ? GridQueryBuilder<T>.FilterCondition(options.filter) : "";
                }

                sp_params.Add("@callname", callName);
                sp_params.Add("@skip", options.skip);
                sp_params.Add("@take", options.take);
                sp_params.Add("@filter", filterby);
                sp_params.Add("@orderby", orderBy);

                var result = await Connection.QueryMultipleAsync(procedureName, sp_params, commandType: CommandType.StoredProcedure);
                var count = result.Read<int>().ToList();
                int totalCount = count[0];
                var list = result.Read<T>().ToList();
                return new GridResult<T>().Data1(list, totalCount);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<GridEntity<T>> GetKendoGridDataAsync<CT>(GridOptions options, string query, string orderBy, string condition)
        {
            try
            {
                query = query.Replace(';', ' ');

                string sqlQuery = options != null ? GridQueryBuilder<T>.Query(options, query, orderBy, condition) : query;

                if (!string.IsNullOrEmpty(condition))
                {
                    condition = " WHERE " + condition;
                }

                var condition1 = options != null ? GridQueryBuilder<T>.FilterCondition(options.filter) : "";
                if (!string.IsNullOrEmpty(condition1))
                {
                    if (!string.IsNullOrEmpty(condition))
                    {
                        condition += " And " + condition1;
                    }
                    else
                    {
                        condition = " WHERE " + condition1;
                    }
                }

                Connection.Open();
                var records = Connection.Query<T>(sqlQuery).ToList();
                var sqlCount = "SELECT COUNT(*) FROM (" + query + " ) As tbl " + condition;
                var count = Connection.Query<int>(sqlCount).ToList();
                var totalCount = count[0];
                var result = new GridResult<T>().Data(records, totalCount);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }

        }         
        public async Task<GridEntity<T>> GetKendoGridDataAsyncWithTempTable<CT>(GridOptions options, string tempQuery, string mainQuery, string dropTableQuery, string orderBy, string condition)
        {
            try
            {
                mainQuery = mainQuery.Replace(';', ' ');

                string sqlQuery1 = options != null ? GridQueryBuilder<T>.Query(options, mainQuery, orderBy, condition) : mainQuery;

                string countQuery = tempQuery + " " + mainQuery + " " + dropTableQuery;
                string sqlQuery = tempQuery + " " + sqlQuery1 + " " + dropTableQuery;

                if (!string.IsNullOrEmpty(condition))
                {
                    condition = " WHERE " + condition;
                }

                var condition1 = options != null ? GridQueryBuilder<T>.FilterCondition(options.filter) : "";
                if (!string.IsNullOrEmpty(condition1))
                {
                    if (!string.IsNullOrEmpty(condition))
                    {
                        condition += " And " + condition1;
                    }
                    else
                    {
                        condition = " WHERE " + condition1;
                    }
                }

                Connection.Open();
                var records = Connection.Query<T>(sqlQuery).ToList();
                var sqlCount = tempQuery +" SELECT COUNT(*) FROM (" + mainQuery + " ) As tbl " + condition +" "+ dropTableQuery;
                var count = Connection.Query<int>(sqlCount).ToList();
                var totalCount = count[0];
                var result = new GridResult<T>().Data(records, totalCount);
                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }

        }

        #endregion Get data using following function
        public async Task<int> ExecuteAsync(string query, object param, int commandTimeOut = 30, CommandType commandType = CommandType.Text)
        {
            SqlTransaction transaction = null;

            try
            {
                await Connection.OpenAsync();
                transaction = Connection.BeginTransaction();
                int rows = await Connection.ExecuteAsync(query, param, transaction, commandTimeOut, commandType);
                transaction.Commit();
                return rows;
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                throw ex;
            }
            finally
            {
                transaction.Dispose();
                Connection.Close();
            }
        }
        public DataTable ExecuteQuery(string sQuery)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlDataAdapter myAdapter = new SqlDataAdapter(sQuery, Connection.ConnectionString);
                myAdapter.Fill(dt);
            }
            catch
            {

            }

            return dt;
        }
        public async Task<CT> GetSingleDataAsync<CT>(string query) where CT : class
        {
            try
            {
                await Connection.OpenAsync();
                var records = await Connection.QueryAsync<CT>(query);
                return records.ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Connection.Close();
            }
        }
        public async Task<List<CT>> GetDataAsync_SP<CT>(string procedureName, string callName, DynamicParameters sp_params) where CT : class
        {
            try
            {
                await Connection.OpenAsync();
                if (callName != "")
                {
                    sp_params.Add("@callname", callName);
                }
                var result = await Connection.QueryMultipleAsync(procedureName, sp_params, commandType: CommandType.StoredProcedure);
                var list = result.Read<CT>().ToList();
                return list.ToList();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw ex.InnerException;
            }
            finally
            {
                Connection.Close();
            }
        }

        public async Task<DataTable> SqlDataTable(string sql)
        {
            var dt = new DataTable();
            try
            {
                SqlCommand cmd = new(sql, Connection);
                cmd.CommandTimeout = 120;
                await Connection.OpenAsync();
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
                await Connection.CloseAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable SqlDataTable2(string sql)
        {
            var dt = new DataTable();
            try
            {
                SqlCommand cmd = new(sql, Connection);
                cmd.CommandTimeout = 120;
                 Connection.OpenAsync();
                SqlDataAdapter da = new(cmd);
                da.Fill(dt);
                 Connection.CloseAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
    }
}

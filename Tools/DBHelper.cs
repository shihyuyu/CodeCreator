using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CodeCreator
{
    public class DBHelper
    {
        private static string _ConnectionString = "";
        //System.Configuration.ConfigurationManager.ConnectionStrings["MSSQL"].ConnectionString;

        public static string ConnectionString 
        {
            get { return (!string.IsNullOrEmpty(_ConnectionString)) ? _ConnectionString : 
                    @""; }
            set { _ConnectionString = value; } 
        }

        public static DynamicParameters Parameters = new DynamicParameters();
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>單一值</returns>
        public static object QueryScalar(string sql, object param=null)
        {
            object ret = null;
            using (var conn = new SqlConnection(_ConnectionString))
            {
                ret = conn.ExecuteScalar(sql, param);
            }
            return ret;
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>單一值</returns>
        public static T QueryScalar<T>(string sql, object param = null)
        {
            T ret = default(T);
            using (var conn = new SqlConnection(_ConnectionString))
            {
                ret = conn.ExecuteScalar<T>(sql, param);
            }
            return ret;
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>回傳一筆資料</returns>
        public static T QueryOne<T>(string sql, object param = null)
        {
            T ret = default(T);
            using (var conn = new SqlConnection(_ConnectionString))
            {
                ret = conn.Query<T>(sql, param).FirstOrDefault();
            }
            return ret;
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>回傳一筆資料</returns>
        public static dynamic QueryOne(string sql, object param = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                var output = conn.Query(sql, param).ToList();
                if (output.Count == 1)
                {
                    return output.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        public static List<T> Query<T>(string sql, object param=null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                var output = conn.Query<T>(sql, param).ToList();
                return output;
            }
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        public static IEnumerable<dynamic> Query(string sql, object param = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                return conn.Query(sql,  param);
            }
        }
        /// <summary>
        /// 執行SQL語法(DQL)
        /// </summary>
        /// <param name="sql">SQL語法(DQL)</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        public static DataTable QueryByDataTable(string sql, object param = null)
        {
            DataTable ret = new DataTable();
            using (var conn = new SqlConnection(_ConnectionString))
            {
                var dataReader = conn.ExecuteReader(sql, param);
                ret.Load(dataReader);
            }
            return ret;
        }
        /// <summary>
        /// 執行SQL語法(DML)
        /// </summary>
        /// <param name="sql">DML語法</param>
        /// <param name="param">參數</param>
        /// <returns>資料異動筆數</returns>
        public static int Execute(string sql, object param = null)
        {
            using (var conn = new SqlConnection(_ConnectionString))
            {
                int output = conn.Execute(sql, param);
                return output;
            }
        }
        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <typeparam name="T">回傳型別</typeparam>
        /// <param name="spname">預存程序名稱</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        public static List<T> StoredProcedure<T>(string spname, object param = null)
        {
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                var output = conn.Query<T>(spname, param, commandType: CommandType.StoredProcedure).ToList();
                return output;
            }
        }
        /// <summary>
        /// 執行StoredProcedure
        /// </summary>
        /// <param name="spname">預存程序名稱</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        public static IEnumerable<dynamic> StoredProcedure(string spname, object param = null)
        {
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                return conn.Query(spname, param, commandType: CommandType.StoredProcedure);
            }
        }
        /// <summary>
        /// 執行StoredProcedure不回傳參數
        /// 準備參數
        /// DynamicParameters parameters = new DynamicParameters();
        /// parameters.Add("@Param1", "abc", DbType.String, ParameterDirection.Input);
        /// parameters.Add("@OutPut1", dbType: DbType.Int32, direction: ParameterDirection.Output);
        /// parameters.Add("@Return1", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
        /// _ = conn.Execute(spname, param, commandType: CommandType.StoredProcedure);
        /// //接回Output值
        /// var outputResult = parameters.Get<int>("@OutPut1");
        /// //接回Return值
        /// var returnResult = parameters.Get<int>("@Return1");
        /// </summary>
        /// <param name="spname">預存程序名稱</param>
        /// <param name="param">參數</param>
        /// <returns>資料異動比數</returns>
        public static int StoredProcedureNonQuery(string spname, object param = null)
        {
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                int output = conn.Execute(spname, param, commandType: CommandType.StoredProcedure);
                return output;
            }
        }
        /// <summary>
        /// 執行Transaction，失敗Rollback
        /// </summary>
        /// <param name="sql">多筆SQL語法</param>
        /// <param name="param">參數</param>
        /// <returns>資料異動比數</returns>
        public static int Transaction(string[] sql, object[] param = null)
        {
            int ret = 0;
            using (SqlConnection conn = new SqlConnection(_ConnectionString))
            {
                conn.Open();

                IDbTransaction tran = conn.BeginTransaction("Transaction");
                try
                {
                    int i = 0;
                    foreach (string s in sql)
                    {
                        conn.Execute(s, param[i], tran);
                        i++;
                    }

                    tran.Commit();
                    ret = i;
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    ret = 0;
                }

                conn.Close();
            }

            return ret;
        }
    }
}
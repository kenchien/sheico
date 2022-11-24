using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Common.Helper {
    public interface IDapper {
        List<T> GetList<T>(string sqlString, object param = null, CommandType? commandType = CommandType.Text,
            int? commandTimeout = 180);

        DataTable GetDataTable(string sqlString, object param = null, IDbTransaction tra = null,
            int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

        int ExecSql(string sql, object param = null, IDbTransaction tra = null,
            int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

        object ExecuteScalar(string sql, object param = null, IDbTransaction tra = null,
            int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

        //void BulkMerge(object DataSource, string DestinationTableName, string AutoMapKeyName = null);
    }

    /// <summary>
    /// 簡化存取dapper
    /// </summary>
    public class DapperBase : IDapper {
        private readonly string _connectionString;

        public DapperBase(string connectionString) {
            _connectionString = connectionString;
        }

        //private IDbConnection IDbConnection {
        //    get {
        //        IDbConnection _conn = new OracleConnection(_connectionString);
        //        return _conn;
        //    }
        //}

        public DataTable GetDataTable(string sqlString, object param = null, IDbTransaction tra = null,
                                        int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {
            DataSet ds = new DataSet();//ken,如果撈取的資料有pkey重複會產生錯誤,外面加一層DataSet.EnforceConstraints=false
            ds.EnforceConstraints = false;
            DataTable dt = ds.Tables.Add();

            using (var db = new OracleConnection(_connectionString)) {
                dt.Load(db.ExecuteReader(sqlString, param, tra, commandTimeout, commandType));
            }

            return dt;
        }

        public List<T> GetList<T>(string sqlString, object param = null, CommandType? commandType = CommandType.Text,
                                    int? commandTimeout = 180) {
            var list = new List<T>();

            using (var db = new OracleConnection(_connectionString)) {
                //db.Query 是dapper的延伸Extension,還有很多不錯的函數可以用
                IEnumerable<T> ts = db.Query<T>(sqlString, param, null, true, commandTimeout, commandType);

                if (ts != null)
                    list = ts.ToList();
            }

            return list;
        }

        /// <summary>
        /// 直接呼叫底層DbConnection.Execute
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tra"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int ExecSql(string sql, object param = null, IDbTransaction tra = null,
                             int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {

            using (var db = new OracleConnection(_connectionString)) {
                int resultCount = db.Execute(sql, param, tra, commandTimeout, commandType);
                return resultCount;
            }

        }

        /// <summary>
        /// 直接呼叫底層DbConnection.ExecuteScalar
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tra"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object param = null, IDbTransaction tra = null,
                                    int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {

            using (var db = new OracleConnection(_connectionString)) {
                var resultCount = db.ExecuteScalar(sql, param, tra, commandTimeout, commandType);
                return resultCount;
            }

        }

    }

}

using System.Collections.Generic;
using System.Data;

namespace Clothes.Dal.DBComponment
{
    public interface IDapper
    {
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
}

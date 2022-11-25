using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Clothes.Dal.Repository {
   public interface IRepository<TEntity> {
      #region Query

      IQueryable<TQuery> QueryBySql<TQuery>(string sql, object param = null, int limit = 2000,
          bool checkLimit = false, int? commandTimeout = 180); //where TQuery : class;

      DataTable QueryToDataTable(string sqlCommend, object param = null, int limit = 100000,
          bool checkLimit = false, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

      #endregion
      bool ExecuteSql(string sql, object param = null, IDbTransaction tra = null,
               int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

      int ExecSql(string sql, object param = null, IDbTransaction tra = null,
      int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

      object ExecuteScalar(string sql, object param = null, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text);

      #region EF function (需使用EF6,暫時不使用)
      //IQueryable<TEntity> QueryAll();
      //IQueryable<TEntity> QueryByCondition(string sql, int limit = 2000, bool checkLimit = false);
      //IQueryable<TEntity> QueryByCondition(Expression<Func<TEntity, bool>> expression, int limit = 2000,
      //    bool checkLimit = false);
      #endregion

      #region CRUD (需使用EF6,暫時不使用)

      //////bool Update(TEntity entity);
      //////bool Create(TEntity entity);
      //////bool Delete(TEntity entity);

      #endregion


      //////void BulkInsert<T>(IList<T> entities) where T : class;
      //////void BulkUpdate<T>(IList<T> entities) where T : class;

      //void BulkMerge(object DataSource, string DestinationTableName, string AutoMapKeyName = null);

   }
}
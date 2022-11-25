using Clothes.Dal.DBComponment;
using Clothes.Dal.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;


namespace Clothes.Dal.Repository {
   public class Repository<TEntity> : IRepository<TEntity> where TEntity : class {
      private readonly IDapper _dapper;
      //private readonly sheicoContext _context;
      private const string NoDataMessage = "查無資料";
      private const int CommandTimeout = 6000;

      //public Repository(IDapper dapper, sheicoContext context) {
      //   _dapper = dapper;
      //   _context = context;
      //}

      public Repository(IDapper dapper) {
         _dapper = dapper;

      }

      public IQueryable<TQuery> QueryBySql<TQuery>(string sql, object param = null, int limit = 2000,
                                             bool checkLimit = false, int? commandTimeout = 180) //where TQuery : class
      {
         try {
            List<TQuery> res = _dapper.GetList<TQuery>(sql, param, CommandType.Text, commandTimeout);

            if (checkLimit) {
               var rowCount = res.Count();
               if (rowCount <= 0) {
                  throw new Exception(NoDataMessage);
               } else if (rowCount > limit) {
                  throw new Exception($"超過 {limit} 筆資料");
               }
            }

            return res.AsQueryable();
         } catch (Exception ex) {
            throw ex;
         }
      }



      /// <summary>
      /// condition by sql string
      /// </summary>
      /// <param name="sqlCommend"></param>
      /// <returns></returns>
      public DataTable QueryToDataTable(string sqlCommend, object param = null, int limit = 100000,
          bool checkLimit = false, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {
         try {
            var dt = _dapper.GetDataTable(sqlCommend, param, tra, commandTimeout, commandType);
            var rowCount = dt.Rows.Count;

            if (checkLimit) {
               if (rowCount <= 0) {
                  throw new Exception(NoDataMessage);
               } else if (rowCount > limit) {
                  throw new Exception($"超過 {limit} 筆資料");
               }
            }

            return dt;
         } catch (Exception ex) {
            throw ex;
         }
      }


      /// <summary>
      /// 執行sql,如果影響筆數>=1,則回傳成功 (類似另一個function ExecSql)
      /// </summary>
      /// <param name="sqlCommend"></param>
      /// <param name="param"></param>
      /// <param name="tra"></param>
      /// <param name="commandTimeout"></param>
      /// <param name="commandType"></param>
      /// <returns></returns>
      public bool ExecuteSql(string sql, object param = null, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {
         try {
            int rows = _dapper.ExecSql(sql, param, tra, commandTimeout, commandType);
            if (rows >= 1) return true;
            return false;
         } catch (Exception) {
            throw;
         }
      }

      /// <summary>
      /// 執行sql,回傳影響的筆數
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="param"></param>
      /// <param name="tra"></param>
      /// <param name="commandTimeout"></param>
      /// <param name="commandType"></param>
      /// <returns></returns>
      public int ExecSql(string sql, object param = null, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {
         return _dapper.ExecSql(sql, param, tra, commandTimeout, commandType);
      }

      /// <summary>
      /// 執行sql,回傳第一筆的第一個欄位值(以object形式回傳,後續自行轉換型別)
      /// </summary>
      /// <param name="sql"></param>
      /// <param name="param"></param>
      /// <param name="tra"></param>
      /// <param name="commandTimeout"></param>
      /// <param name="commandType"></param>
      /// <returns></returns>
      public object ExecuteScalar(string sql, object param = null, IDbTransaction tra = null,
          int? commandTimeout = 6000, CommandType? commandType = CommandType.Text) {
         return _dapper.ExecuteScalar(sql, param, tra, commandTimeout, commandType);
      }



      #region EF function (需使用EF6,暫時不使用)
      //////public IQueryable<TEntity> QueryAll() {
      //////   try {
      //////      int rowCount = _context.Set<TEntity>().Count();
      //////      if (rowCount <= 0) {
      //////         throw new Exception(NoDataMessage);
      //////      } else if (rowCount < 1000) {
      //////         IQueryable<TEntity> entities = _context.Set<TEntity>().AsQueryable();
      //////         return entities;
      //////      } else {
      //////         throw new Exception($"超過 1000 筆資料");
      //////      }
      //////   } catch (Exception) {
      //////      throw;
      //////   }
      //////}
      //////      /// <summary>
      ///////// condition by Linq
      ///////// </summary>
      ///////// <param name="expression"></param>
      ///////// <returns></returns>
      //////public IQueryable<TEntity> QueryByCondition(Expression<Func<TEntity, bool>> expression, int limit = 2000,
      //////    bool checkLimit = false) {
      //////   try {
      //////      var rowCount = _context.Set<TEntity>().Count(expression);

      //////      if (checkLimit) {
      //////         if (rowCount > limit) {
      //////            throw new Exception($"超過 {limit} 筆資料");
      //////         }
      //////      }

      //////      var entities = _context.Set<TEntity>().Where(expression).AsQueryable();
      //////      return entities;
      //////   } catch (Exception) {
      //////      throw;
      //////   }
      //////}

      ///////// <summary>
      ///////// condition by sql string
      ///////// </summary>
      ///////// <param name="sql"></param>
      ///////// <returns></returns>
      //////public IQueryable<TEntity> QueryByCondition(string sql, int limit = 2000, bool checkLimit = false) {
      //////   try {
      //////      List<TEntity> res = _dapper.GetList<TEntity>(sql);

      //////      if (checkLimit) {
      //////         var rowCount = res.Count();
      //////         if (rowCount <= 0) {
      //////            throw new Exception(NoDataMessage);
      //////         } else if (rowCount > limit) {
      //////            throw new Exception($"超過 {limit} 筆資料");
      //////         }
      //////      }

      //////      return res.AsQueryable();
      //////   } catch (Exception ex) {
      //////      throw ex;
      //////   }
      //////}

      #endregion



      #region CRUD (需使用EF6,暫時不使用)
      //////public bool Update(TEntity entity) {
      //////   try {
      //////      _context.Set<TEntity>().Update(entity);

      //////      if (_context.SaveChanges() >= 1) return true;

      //////      return false;
      //////   } catch (Exception ex) {
      //////      throw ex;
      //////   }
      //////}

      //////public bool Create(TEntity entity) {
      //////   try {
      //////      _context.Set<TEntity>().Add(entity);

      //////      if (_context.SaveChanges() >= 1) return true;

      //////      return false;
      //////   } catch (InvalidOperationException) {//很可能是p key重複
      //////      return false;
      //////   } catch (DbUpdateException) {//當p key重複又要寫入logOfException or execRecord,就會進入這裡
      //////      return false;
      //////   } catch (Exception ex) {
      //////      throw ex;
      //////   }
      //////}

      //////public bool Delete(TEntity entity) {
      //////   try {
      //////      _context.Set<TEntity>().Remove(entity);

      //////      return _context.SaveChanges() >= 1;
      //////   } catch (Exception) {
      //////      throw;
      //////   }
      //////}
      #endregion



      ///////// <summary>
      ///////// Bulk Insert Or Update (使用原生DbContext處理,不用額外套件)
      ///////// </summary>
      ///////// <typeparam name="T"></typeparam>
      ///////// <param name="entities"></param>
      ///////// <param name="bulkConfig"></param>
      ///////// <param name="progress"></param>
      //////public void BulkInsert<T>(IList<T> entities) where T : class {
      //////   //ken,本來要用EFCore.BulkExtension套件,但是這鳥套件只支援MSSQL,不支援mysql,有夠靠北,自己寫!!

      //////   var count = entities.Count;
      //////   var batchcount = 100;//每次處理100筆,基本這個單位就夠水準
      //////   for (int k = 0; k <= (count / batchcount); k++) {
      //////      if (k == count / batchcount && count % batchcount > 0)//沒有整除,有多出幾筆,要特別處理
      //////         _context.AddRange(entities.Skip(k * batchcount));//UpdateRange 如果有pKey就代表InsertOrUpdate
      //////      else
      //////         _context.AddRange(entities.Skip(k * batchcount).Take(batchcount));

      //////      _context.SaveChanges();
      //////   }//for (int k = 0; k <= (count / batchcount); k++)

      //////}

      ///////// <summary>
      ///////// Bulk Insert Or Update (使用原生DbContext處理,不用額外套件)
      ///////// </summary>
      ///////// <typeparam name="T"></typeparam>
      ///////// <param name="entities"></param>
      ///////// <param name="bulkConfig"></param>
      ///////// <param name="progress"></param>
      //////public void BulkUpdate<T>(IList<T> entities) where T : class {
      //////   //ken,本來要用EFCore.BulkExtension套件,但是這鳥套件只支援MSSQL,不支援mysql,有夠靠北,自己寫!!

      //////   var count = entities.Count;
      //////   var batchcount = 100;//每次處理100筆,基本這個單位就夠水準
      //////   for (int k = 0; k <= (count / batchcount); k++) {
      //////      if (k == count / batchcount && count % batchcount > 0)//沒有整除,有多出幾筆,要特別處理
      //////         _context.UpdateRange(entities.Skip(k * batchcount));//UpdateRange 如果有pKey就代表InsertOrUpdate
      //////      else
      //////         _context.UpdateRange(entities.Skip(k * batchcount).Take(batchcount));

      //////      _context.SaveChanges();
      //////   }//for (int k = 0; k <= (count / batchcount); k++)

      //////}

      /// <summary>
      /// insert and update (使用Z.BulkOperations套件)
      /// </summary>
      /// <param name="DataSource"></param>
      /// <param name="DestinationTableName">要更新哪個table</param>
      /// <param name="AutoMapKeyName">p key</param>
      /// <returns></returns>
      //public void BulkMerge(object DataSource, string DestinationTableName, string AutoMapKeyName = null)
      //{
      //    _dapper.BulkMerge(DataSource, DestinationTableName, AutoMapKeyName);
      //}
   }
}
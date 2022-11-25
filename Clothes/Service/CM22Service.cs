using System.Data;
using System.Linq;
using Clothes.Dal.Repository;
using Clothes.Dal.Models;
using System;
using System.Collections.Generic;

namespace Clothes.Service {
   public class CM22Service {
      private string DebugFlow = "";//ken,debug專用
      private string ErrorMessage = "";//ken,debug專用
      private readonly IRepository<T14> _T14Repository;

      public CM22Service(IRepository<T14> t14Repository) {
         _T14Repository = t14Repository;
      }



      /// <summary>
      /// query T14
      /// </summary>
      /// <param name="sevId"></param>
      /// <returns></returns>
      public DataTable GetT14(string T1405, string T1401, string T1403) {
         try {
            string sql =
                $@"select * from T14
                where T1405=@T1405
                and T1401=@T1401
                and T1403=@T1403";

            return _T14Repository.QueryToDataTable(sql,
                new { T1405, T1401, T1403 });
         } catch (Exception ex) {
            throw ex;
         }
      }


      /// <summary>
      /// query T15
      /// </summary>
      /// <param name="sevId"></param>
      /// <returns></returns>
      public DataTable GetT15(string T1501, string T1502, string T1512,
                              string T1505, string T1506, string T1507) {
         try {
            //成品料號	T1501
            //成品顏色代碼	T1502
            //成品作法版次	T1512
            //成品作法項次	T1505
            //部件作法編碼	T1506
            //部件作法版次	T1507

            string sql =
                $@"select * from T15
                where T1501=@T1501
                and T1502=@T1502
                and T1512=@T1512
                and T1505=@T1505
                and T1506=@T1506
                and T1507=@T1507";

            return _T14Repository.QueryToDataTable(sql,
                new {
                   T1501, T1502, T1512,
                   T1505, T1506, T1507
                });
         } catch (Exception ex) {
            throw ex;
         }
      }


            public List<T15> GetT15test(string T1501, string T1502, string T1512,
                              string T1505, string T1506, string T1507) {
         try {
            //成品料號	T1501
            //成品顏色代碼	T1502
            //成品作法版次	T1512
            //成品作法項次	T1505
            //部件作法編碼	T1506
            //部件作法版次	T1507

            string sql =
                $@"select * from T15
                where T1501=@T1501
                and T1502=@T1502
                and T1512=@T1512
                and T1505=@T1505
                and T1506=@T1506
                and T1507=@T1507";

            return _T14Repository.QueryBySql<T15>(sql,
                new {
                   T1501, T1502, T1512,
                   T1505, T1506, T1507
                }).ToList();
         } catch (Exception ex) {
            throw ex;
         }
      }



   }
}
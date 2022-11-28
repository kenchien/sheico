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

      public List<T> GetColorList<T>(string key) {
         try {
            //成品料號	T1501
            //成品顏色代碼	T1502
            //成品作法版次	T1512
            //成品作法項次	T1505
            //部件作法編碼	T1506
            //部件作法版次	T1507

            string sql =
$@"select 'BKAQ' as value, 'BKAQ-text' as text
union all select 'AAA','AAA-text'
";

            return _T14Repository.QueryBySql<T>(sql,
                new {
                   key = "T1502"
                }).ToList();
         } catch (Exception ex) {
            throw ex;
         }
      }

            public List<T01> GetAllBig() {
         try {
            string sql =
$@"select t01.*
from t01
where t0104 > convert(varchar, getdate(), 112)
order by t01.t0102
";

            return _T14Repository.QueryBySql<T01>(sql).ToList();
         } catch (Exception ex) {
            throw ex;
         }
      }

      public List<T> GetAllBigList<T>() {
         try {
            string sql =
$@"select t0102 as Value, t0103+' '+t0102 as Text
from t01
where t0104 > convert(varchar, getdate(), 112)
order by t01.t0102
";

            return _T14Repository.QueryBySql<T>(sql).ToList();
         } catch (Exception ex) {
            throw ex;
         }
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

            sql = $@"select t15.* 
from T15
join (select t1501,t1502,t1506,max(t1507) as t1507 from t15 group by t1501,t1502,t1506) g on g.t1501=t15.t1501 and g.t1502=t15.t1502 and g.t1506=t15.t1506
left join T14 on T14.T1401=g.t1506 and T14.T1403 = g.t1507
where T15.T1501=@T1501
and T15.T1502=@T1502";

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
      public DataTable GetT15(string T1501, string T1502, string T1512 = null,
                              string T1513 = null, string T1514 = null) {
         try {
            //成品料號	T1501
            //成品顏色代碼	T1502
            //成品作法版次	T1512
            //成品作法項次	T1505
            //部件作法編碼	T1506
            //部件作法版次	T1507

            string sql =
$@"select
t15.t1505,
t15.t1503,--t01.t0103+' '+t01.t0102 as bigname,
t15.t1504,
t02.t0202 as middleName,
t15.t1506,

t14.t1402 as descc,
t15.t1507,
t14.t1530 as memo,
t14.t1406 as unitcode,
t14.t1414 as pic,
(case when isnull(t151.t15101,0)=0 then N'未建' else N'已建' end) as help
from t15
left join t14 on t14.t1401=t15.t1506 and t14.t1403=t15.t1507 and t14.t1405=t15.t1504
left join t01 on t01.t0102=t15.t1503
left join t02 on t02.t0201=t15.t1504
left join t151 on t151.t15101=t15.t1501 
                  and t151.t15102=t15.t1502 
                  and t151.t15159=t15.t1512 
                  and t151.t15103=t15.t1505 
                  and t151.t15104=t15.t1506 
                  and t151.t15105=t15.t1507
where t15.T1501=@T1501
and t15.T1502=@T1502";

            if (!string.IsNullOrEmpty(T1512))
               sql += " and t15.T1512=@T1512";
            if (!string.IsNullOrEmpty(T1513))
               sql += " and t15.T1513=@T1513";
            if (!string.IsNullOrEmpty(T1514))
               sql += " and t15.T1514=@T1514";

            sql += " order by t15.t1501,t15.t1502,t15.t1505,t15.t1506,t15.t1507 desc";

            return _T14Repository.QueryToDataTable(sql,
                new {
                   T1501, T1502, T1512,
                   T1513, T1514
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



      /// <summary>
      /// query T151
      /// </summary>
      /// <param name="sevId"></param>
      /// <returns></returns>
      public DataTable GetT151(string T15101, string T15102, string T15159,
                               string T15103, string T15104, string T15105) {
         try {
            //T151(成品作法輔料檔)
            //成品料號	T15101	CHAR(10)
            //成品顏色代碼	T15102	CHAR(4)
            //成品作法版次	T15159	CHAR(3)
            //成品作法項次	T15103	CHAR(4)
            //部件作法編碼	T15104	CHAR(8)
            //部件作法版次	T15105	CHAR(3)


            string sql =
$@"select t151.t15106,
t151.t15107,
'輔料簡稱' as xx,
t151.t15108,
t151.t15109,
t151.t15112
from t151
where t151.t15101=@T15101
and t151.t15102=@T15102
and t151.t15159=@T15159
and t151.t15103=@T15103
and t151.t15104=@T15104
and t151.t15105=@T15105
";


            return _T14Repository.QueryToDataTable(sql,
                new {
                   T15101, T15102, T15159, T15103, T15104, T15105
                });
         } catch (Exception ex) {
            throw ex;
         }
      }
   }
}
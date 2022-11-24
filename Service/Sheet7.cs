namespace MaintainReport {

    /// <summary>
    /// 介接
    /// </summary>
    public class Sheet7 : SheetBase {

        public Sheet7() {
            SheetName = "介接";
            SheetIndex = 7;
            DumpStartCell = "E3";
            DrawBorder = false;

            DateCell = "E1";
            OldRange = new int[] { 1, 5, 35, 16 };
            NewRange = new int[] { 1, 7, 35, 18 };
            DumpRange = new int[] { 3, 5, 35, 6 };

            SetSql();
        }
        protected override void SetSql() {
            //1.字串裡面的{要變成 {{,  "要變成""
            //2.EMR/QINV/TRACE 都有IDA_0101 (帳號地區/疾病查詢),不過這個錯了也沒關係,所以不列入觀察指標
            
            sql = $@"
with base as (
  select 1 as sort,'LIMS' as sysId,'IDA_0302' as api from dual
  union all select 2,'LIMS','IDA_0303' from dual
  union all select 3,'LIMS','IDA_0304' from dual
  union all select 4,'LIMS','IDA_0353' from dual
  union all select 5,'LIMS','IDA_0401' from dual
  union all select 6,'LIMS','IDA_0402' from dual
  union all select 7,'NIDRS','AddTransaction' from dual
  union all select 8,'NIDRS','NewReportSample' from dual
  union all select 9,'NIDRS','SampleQuery' from dual
  union all select 10,'NIDRS','SampleResult' from dual
  union all select 11,'NIDRS','ReportUpdate_Upd' from dual
  union all select 12,'NIDRS','ReportUpdate_Del' from dual
  union all select 13,'SQMS','IDA_0302' from dual
  union all select 14,'SQMS','IDA_0351' from dual
  union all select 15,'SQMS','IDA_0352' from dual
  union all select 16,'TRACE','IDA_0303' from dual
  union all select 17,'TRACE','IDA_0305' from dual
  union all select 18,'TRACE','IDA_0306' from dual
  union all select 19,'TRACE','IDA_0308' from dual
  union all select 20,'TRACE','IDA_0309' from dual
  union all select 21,'HAS','IDA_0305' from dual
  union all select 22,'HAS','IDA_0308' from dual
  union all select 23,'QINV','IDA_0303' from dual
  union all select 24,'QINV','IDA_0305' from dual
  union all select 25,'QINV','IDA_0306' from dual
  union all select 26,'QINV','IDA_0309' from dual
  union all select 27,'QINV','IDA_0353' from dual
  union all select 28,'ZONE','IDA_0501' from dual
  union all select 29,'ZONE','IDA_0502' from dual
  union all select 30,'NIDRS','ZONE' from dual
  union all select 31,'TB','TB_REPORT' from dual
  union all select 32,'TB','TB_DETERMINED' from dual
  union all select 33,'TB','TB_UPDATE' from dual
),su as (
  select user_id,api as call_api,
    count(0) as totalCount,
    --  排除HAS的錯誤 0x00304002 [通報單號與通報疾病不相符]..
    --  排除HAS的錯誤 0x00305002 [通報單已刪除]..
    sum(case when response_status !=200 and nvl(error_code,0) in ('0x00305002','0x00304002','0x00001002','0x00353001') then 0 when response_status !=200 then 1 else 0 end) as errorCount
  from log_api
  where request_time >= :startDate
  and request_time <= :endDate
  group by user_id,api
),lims as (
  select 'NIDRS' as sysId,
  method as api,
  count(0) as totalCount,
  sum(case when response_status=0 then 1 when response_data like '%Success"":false%' then 1 else 0 end) as errorCount
  from log_lims
  where event_time >= :startDate
  and event_time <= :endDate
  --2021/9/9 排除 刪除錯誤,查無通報單或刪除註記錯誤 or 該筆資料已被刪除或查無送驗單
  and not (method in ('ReportUpdate_Del','ReportUpdate_Upd') and response_data like '%查無%')
  group by method

), tb_det_success as (
    select 'TB','TB_DETERMINED',count(0) as successCount
    from log_job
    where job_name='TB_SYNC'
    and log_level='INFO'
    and datetime >= :startDate
    and datetime <= :endDate
                                              --Write REPORT_DETERMINED back to SQL Server
    and message like 'IDA REPORT_DETERMINED%' --IDA REPORT_DETERMINED 797ec8e8b5fd41bcba591b5c71xxxxxx generated
), tb_det_fail as (
    select 0 as failCount from dual
), tb_update_success as (
    select 'TB','TB_UPDATE',count(0) as successCount
    from log_job
    where job_name='TB_SYNC'
    and log_level='INFO'
    and datetime >= :startDate
    and datetime <= :endDate
    and message like 'Write TB_M_TB_REPORT_UPDATE back to SQL Server %' --Write TB_M_TB_REPORT_UPDATE back to SQL Server (REPORT_ID: 110xxxxxx3420)
), tb_update_fail as (
    select count(0) as failCount
    from log_job
    where job_name='TB_SYNC'
    and log_level='ERROR'
    and datetime >= :startDate
    and datetime <= :endDate
    and message not like '`DealWithIdaReports` caught exception: TBSync.ReportNotFoundException: Report not found!%'
    --我們這邊找不到這7個report_id  '0953600002270','0963700001151','0973200007806','094TB00015057','0950200002624','0973900001663','0989900059977'
),tb as (
    select 'TB' as sysId,'TB_REPORT' as api,sum(tb_report_total) as totalCount,sum(tb_report_failure) as errorCount
    from tb_log
    where process_time >= :startDate
    and process_time <= :endDate

    union all  
    select tb_det_success.*,tb_det_fail.failCount
    from tb_det_success,tb_det_fail

    union all  
    select tb_update_success.*,tb_update_fail.failCount
    from tb_update_success,tb_update_fail

),gis_sum as (
  select count(0) as totalCount
  from log_job
  where job_name='GIS'
  and datetime >= :startDate
  and datetime <= :endDate
  and message like '% Address %'
),gis_err as (
  --如果要抓對的,關鍵字是 StatusCode"":""1""
  --排除message like '%查無定位資料(無TGOS定位)%'
  select count(0) as errorCount
  from log_job
  where job_name='GIS'
  and datetime >= :startDate
  and datetime <= :endDate
  and message not like '%Address%'
  and message not like '%已發生類型%'
  and message not like '%查無定位資料(無TGOS定位)%'
  and message not like 'Start%'
  and message not like 'Console%'
  and message not like 'ZONE%'
  and message not like 'Query%'
  and message not like 'Stop%'
),gis_merge as (
  select 'NIDRS' as sysId,
  'ZONE' as api,
  gis_sum.totalCount,
  gis_err.errorCount
  from gis_sum,gis_err
) --合併顯示
select --b.sysId,b.api,
nvl(nvl(lims.totalCount,nvl(tb.totalCount,su.totalCount)),g.totalCount) as totalCount,
nvl(nvl(lims.errorCount,nvl(tb.errorCount,su.errorCount)),g.errorCount) as errorCount
from base b
left join su on su.user_id=b.sysId and su.call_api=b.api
left join lims on lims.sysId=b.sysId and lims.api=b.api
left join tb on tb.sysId=b.sysId and tb.api=b.api
left join gis_merge g on g.sysId=b.sysId and g.api=b.api
order by b.sort
";

        }


    }

}

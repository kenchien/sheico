using Common.Helper;

namespace MaintainReport {

    /// <summary>
    /// 介接異常清單
    /// </summary>
    public class Sheet8 : SheetBase {
        public Sheet8() {
            SheetName = "介接異常清單";
            SheetIndex = 8;
            DumpStartCell = "B3";
            DrawBorder = false;

            SkipNoDataError = true;

            DateCell = "";
            ClearRange = new int[] { 3, 1, 503, 8 };

            SetSql();
        }


        protected override void SetSql() {
        //1.字串裡面的{要變成 {{,  "要變成""
        //2.測試時輸入的:endDate為字串,正式執行時輸入的型態為datetime,過濾條件有差異

            sql = $@"

with api as (
  select TO_CHAR(response_time, 'mm/dd hh24:mi:ss') as response_time,
    user_id,
    api,
    report_id,
    1 as totalCount, --sample_id,
    error_code,
    parameters,
    error_message
  from log_api
  where response_time >= :startDate
  and response_time <= :endDate
  and response_status!=200
  and nvl(error_code,0) not in ('0x00305002','0x00304002','0x00001002','0x00353001')
  order by response_time desc

),lims as (
  select TO_CHAR(event_time, 'mm/dd hh24:mi:ss') as event_time,
    N'NIDRS' as user_id,
    method,
    report_id,
    1 as totalCount, --sample_id,
    response_status,
    request_data as parameters,
    response_data as error_message
  from log_lims s
  where event_time >= :startDate
  and event_time <= :endDate
  and ( response_status=0 or response_data like '%Success"":false%' )
  --2021/9/9 排除 刪除錯誤,查無通報單或刪除註記錯誤 or 該筆資料已被刪除或查無送驗單
  and not (method in ('ReportUpdate_Del','ReportUpdate_Upd') and response_data like '%查無%')
  order by event_time desc

),tb as (
    select TO_CHAR(datetime, 'mm/dd hh24:mi:ss') as event_time,
    N'TB' as user_id,
    null as method,
    null as report_id,
    0 as totalCount, --count(0) as totalCount,
    N'ERROR' as response_status,
    null,
    message
    from log_job
    where job_name='TB_SYNC'
    and log_level='ERROR'
    and datetime >= :startDate
    and datetime <= :endDate
    and message not like '`DealWithIdaReports` caught exception: TBSync.ReportNotFoundException: Report not found!%'
    and rownum<501
    --group by log_level,dbms_lob.substr(message,4000,1)

),merge as (
    select * from api
    union all
    select * from lims
    union all
    select * from tb
),ready as (
    select * from merge
    order by response_time
)
select * from ready
where rownum<2001
";

        }


    }

}

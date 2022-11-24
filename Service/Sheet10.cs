using Common.Helper;

namespace MaintainReport {

    /// <summary>
    /// 其它介接異常清單
    /// </summary>
    public class Sheet10 : SheetBase {
        public Sheet10() {
            SheetName = "其它介接異常清單";
            SheetIndex = 10;
            DumpStartCell = "B3";
            DrawBorder = false;

            SkipNoDataError = true;

            DateCell = "";
            ClearRange = new int[] { 3, 1, 503, 8 };

            SetSql();
        }


        protected override void SetSql() {
            //1.字串裡面的{要變成 {{,  "要變成""
            //2.注意varchar2跟nvarchar2的轉換,以及clob跟nclob

            sql = $@"

with niis_err as ( --NIIS,排除ErrorCode=00 查無此個案

    select event_time,report_id,N'01' as errcode,'身分證號格式錯誤' as errmsg from log_niis
    where response_data like '%ErrorCode"":""01%'
    and event_time >= :startDate
    and event_time <= :endDate

    union all
    select event_time,report_id,N'02' as errcode,'居留證號格式錯誤' as errmsg from log_niis
    where response_data like '%ErrorCode"":""02%'
    and event_time >= :startDate
    and event_time <= :endDate

    union all
    select event_time,report_id,N'9xx' as errcode,'其他錯誤' as errmsg from log_niis
    where response_data like '%ErrorCode"":""9%'
    and event_time >= :startDate
    and event_time <= :endDate

    union all
    select event_time,report_id,response_status as errcode,dbms_lob.substr(response_data,200,1) as errmsg from log_niis 
    where response_status!='1..'
    and event_time >= :startDate
    and event_time <= :endDate
  

),exit_err as (  --入出境資料,entry_exit_error_code=1005是查無資料(不算錯誤)
  select log_date,e.system_id,e.entry_exit_error_code,null as errMsg
  from report_entry_and_exit e
  where log_date >= :startDate
  and log_date <= :endDate
  and entry_exit_error_code!='1005'
  and entry_exit_error_code is not null

),eec_err as (
  select accept_time,a.cdaguid,null,cast(a.xml_status_notes as varchar2(2000))
  from eec_request_apply a
  where index_type!='醫療影像及報告'
  and xml_status_notes not like '%找不到報告%'   --先排除掉找不到報告(連成功次數都不計算)
  and accept_time >= :startDate
  and accept_time <= :endDate
  and xml_status_notes!='Success'

),merge as (
    select 'NIIS' as system_name,e.* from niis_err e
    union all select '入出境資料' as system_name,e.* from exit_err e
    union all select '電子病歷調閱數' as system_name,eec_err.* from eec_err
),ready as (
    select * from merge
    order by system_name,event_time desc
)
select * from ready
where rownum<501
";

        }


    }

}

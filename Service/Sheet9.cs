namespace MaintainReport {

    /// <summary>
    /// 其它介接
    /// </summary>
    public class Sheet9 : SheetBase {

        public Sheet9() {
            SheetName = "其它介接";
            SheetIndex = 9;
            DumpStartCell = "C4";
            DrawBorder = false;

            DateCell = "C2";
            OldRange = new int[] { 2, 3, 8, 14 };//近六天的範圍(左上右下)
            NewRange = new int[] { 2, 5, 8, 16 };
            DumpRange = new int[] { 4, 3, 8, 4 };//當天填入的範圍(左上右下)

            SetSql();

            //DumpStartCell2 = "C6";
        }


        protected override void SetSql() {
            //1.字串裡面的{要變成 {{,  "要變成""

            sql = $@"
with niis_err as ( --NIIS,排除ErrorCode=00 查無此個案
  --select event_time,report_id,N'00' as errcode,'查無此個案' as errmsg from log_niis
  --where response_data like '%ErrorCode"":""00%'
  --and event_time >= :startDate
  --and event_time <= :endDate

  --union all
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

),niis_err_sum as (
  select 'NIIS' as itemName,
  count(0) as errorCount
  from niis_err

),niis_total as (
  select count(0) as totalCount 
  from log_niis
  where event_time >= :startDate
  and event_time <= :endDate

),doh_merge as ( --衛福部死亡資料
  select dhodied_error_code,log_date
  from report_dohdied
  where log_date >= :startDate
  and log_date <= :endDate
  union all
  select dhodied_error_code,log_date
  from report_tb_dohdied
  where log_date >= :startDate
  and log_date <= :endDate

),doh_sum as (
  select '衛福部死亡資料' as itemName,
  count(0) as totalCount,
  sum(case when dhodied_error_code is not null then 1 else 0 end) as errorCount
  from doh_merge

),entry_exit_sum as (  --入出境資料,entry_exit_error_code=1005是查無資料(不算錯誤)
  select '入出境資料' as itemName,
  count(0) as totalCount,
  sum(case when entry_exit_error_code='1005' then 0 when entry_exit_error_code is not null then 1 else 0 end) as errorCount
  from report_entry_and_exit
  where log_date >= :startDate
  and log_date <= :endDate

),eec_total as (
    select '電子病歷調閱數' as itemName,
    count(0) as totalCount,  --總調閱數
    sum(case when xml_status_notes!='Success' then 1 else 0 end) as errorCount  --失敗調閱數
    from eec_request_apply a
    where index_type!='醫療影像及報告'
    and xml_status_notes not like '%找不到報告%'   --先排除掉找不到報告(連成功次數都不計算)
    and accept_time >= :startDate
    and accept_time <= :endDate

),eec_upload as (
  select '上傳數' as itemName,
  count(0) as upCount, --使用者上傳檔案的次數
  null as errorCount
  from ANAMNESIS
  where upload_date >= :startDate
  and upload_date <= :endDate
)
select nvl(t.totalCount,0),nvl(e.errorCount,0) from niis_err_sum e,niis_total t
union all select nvl(totalCount,0)-nvl(errorCount,0),nvl(errorCount,0) from doh_sum
union all select nvl(totalCount,0)-nvl(errorCount,0),nvl(errorCount,0) from entry_exit_sum
union all select nvl(totalCount,0)-nvl(errorCount,0),nvl(errorCount,0) from eec_total
union all select nvl(upCount,0),null from eec_upload

";

        }

        //        public override DataTable GetDataTable2(object param = null) {
        //            DapperBase dapperBase = new DapperBase(ConnectionString);

        //            string sql2 = $@"
        //with upload as (
        //  select count(0) as upCount --使用者上傳檔案的次數
        //  from ANAMNESIS
        //  where upload_date >= :startDate
        //  and upload_date <= :endDate
        //),total as (
        //  select count(0) as totalCount,  --總調閱數
        //  sum(case when xml_status_notes!='Success' then 1 else 0 end) as errorCount  --失敗調閱數
        //  from eec_request_apply
        //  where index_type!='醫療影像及報告'
        //  and accept_time >= :startDate
        //  and accept_time <= :endDate
        //)
        //select nvl(t.totalCount,0) as totalCount from total t
        //union all select nvl(t.errorCount,0) from total t
        //union all select nvl(u.upCount,0) from upload u";

        //            var dt = dapperBase.GetDataTable(sql2, param);

        //            return dt;
        //        }

    }

}

namespace MaintainReport {

    /// <summary>
    /// EMR
    /// </summary>
    public class Sheet2 : SheetBase {

        public Sheet2() {
            SheetName = "EMR";
            SheetIndex = 2;
            DumpStartCell = "B3";
            DrawBorder = false;
            
            DateCell = "";

            SetSql();
        }


        protected override void SetSql() {

            sql = $@"
with all_hos as ( --找出所有申請EMR的醫院
    select distinct hospital_id
    from hospital_emr_auth
    where hospital_name not like '%土城%'
),all_hos_name as ( --找到所有申請EMR的醫院名稱(總清單)
    select h.id as hospital_id,h.name as hospital_name
    from hospital h
    join all_hos a on a.hospital_id=h.id
    where nvl(h.deleted,0)=0
    --order by hospital_name
),u_report as ( --先作一個unique_report
    select system_id,max(revise_history) as revise_history
    from report
    group by system_id
),err_idno_count_err as ( --算EMR同個身分證錯誤只算一個 對了互消(錯誤)
    select count(*),  CREATOR_UNIT
        ,idno
        ,TO_CHAR(event_time, 'yyyy/mm/dd') as DD
        from log_emr
        where
        error_code = '通報失敗'
        and event_time >=  :startDate
        and event_time <= :endDate

    group by CREATOR_UNIT,idno,TO_CHAR(event_time, 'yyyy/mm/dd')
), err_idno_count_OK as (--算EMR同個身分證錯誤只算一個 對了互消(正確)
    select count(*),  CREATOR_UNIT
        ,idno
        ,TO_CHAR(event_time, 'yyyy/mm/dd') as DD
        from log_emr
        where
        error_code = '通報成功'
        and event_time >=  :startDate
        and event_time <= :endDate

    group by CREATOR_UNIT,idno,TO_CHAR(event_time, 'yyyy/mm/dd')

), err_idno_count_SUB_ERROR_OK as ( --成功失敗互減
    select creator_unit, idno, DD from err_idno_count_err
    minus
    select creator_unit, idno, DD from  err_idno_count_OK

),err_ok_count as ( --合併 相減後的 count
    select count(*) as total_err_ok_count,creator_unit from err_idno_count_SUB_ERROR_OK
    group by creator_unit
    --order by creator_unit desc

),total_rep as (  --總通報量= 成功新增的法傳通報單數量(所有來源/所有疾病)
    select r.hospital as hospital_id, count(0) as totalCount
    from report r
    join u_report u on u.SYSTEM_ID=r.SYSTEM_ID and u.REVISE_HISTORY=r.REVISE_HISTORY
    join all_hos a on a.hospital_id=r.hospital
  where r.cdc_received_date >= :startDate
  and r.cdc_received_date <= :endDate
    group by r.hospital
),emr_api as ( --EMR成功通報數/EMR失敗通報數
    select creator_unit as hospital_id,
    sum(case when error_code='通報成功' and is_report=1 and report_id is not null then regexp_count(report_id,'[,]')+1 else 0 end) as successCount
    --,sum(case when error_code='通報失敗' then 1 else 0 end) as errorCount
    from log_emr
    where method=1
    and is_report=1
    and event_time >= :startDate
    and event_time <= :endDate
    group by creator_unit
) --合併顯示
select --a.hospital_id,
a.hospital_name,
nvl(emr_api.successCount,0) as successCount,
nvl(err_ok_count.total_err_ok_count,0) as errorCount,
nvl(total_rep.totalCount,0) as totalCount
from all_hos_name a
left join emr_api on emr_api.hospital_id=a.hospital_id
left join total_rep on total_rep.hospital_id=a.hospital_id
left join err_ok_count on err_ok_count.creator_unit=a.hospital_id
order by a.hospital_name
";

        }

    }

}

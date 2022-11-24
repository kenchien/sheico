using Common.Helper;

namespace MaintainReport {

    /// <summary>
    /// EMR失敗清單
    /// </summary>
    public class Sheet3 : SheetBase {

        public Sheet3() {
            SheetName = "EMR失敗清單";
            SheetIndex = 3;
            DumpStartCell = "B3";
            DrawBorder = false;
            
            SkipNoDataError = true;
            
            DateCell = "";
            ClearRange = new int[] { 3, 1, 503, 8 };

            SetSql();
        }

        protected override void SetSql() {

            sql = $@"
with base as (
    select e.*,
    rank () over (partition by idno order by event_time desc) as rnk
    from log_emr e
    where e.event_time >= :startDate
    and e.event_time <= :endDate
),summ as (
    select idno,min(event_time) as stime,count(0) as count from base group by idno
)
select TO_CHAR(summ.stime, 'mm/dd hh24:mi') as stime,
    TO_CHAR(event_time, 'mm/dd hh24:mi') as etime,
    e.CREATOR_UNIT,
    h.name,
    SUBSTR(crypto('2',e.idno), 1, 6) as idno,
    report_id,
    error_code,
    summ.count as tryCount,
    error_message
from base e
left join summ on summ.idno=e.idno
left join hospital h on h.id=e.CREATOR_UNIT
where e.rnk=1
and error_code=N'通報失敗'
and error_message not like '%今天已通報%'
order by h.name,idno
";

        }


    }

}

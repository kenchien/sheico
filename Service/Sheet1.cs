namespace MaintainReport {

    /// <summary>
    /// 通報
    /// </summary>
    public class Sheet1 : SheetBase {

        public Sheet1() {
            SheetName = "通報";
            SheetIndex = 1;
            DumpStartCell = "B4";
            DrawBorder = false;

            DateCell = "D2";
            OldRange = new int[] { 2, 4, 21, 15 };
            NewRange = new int[] { 2, 6, 21, 17 };
            DumpRange = new int[] { 4, 4, 21, 5 };

            SetSql();
        }


        protected override void SetSql() {

            sql = $@"
with urep as (  --法傳個案-找到每筆通報單最新版本
  select report.system_id as system_id,max (report.revise_history) as revise_history
  from report
  group by system_id
),total_rep as (  --法傳個案-找到範圍內的通報單
  select r.id,r.disease,r.residence_county,r.source_name
  from report r
  join urep u on u.system_id=r.system_id and u.revise_history=r.revise_history
  where nvl(r.deleted,0) = 0
  and r.cdc_received_date >= :startDate
  and r.cdc_received_date <= :endDate
),sum_rep as (  --法傳個案-依照 通報來源 先做gorup by
  select t.source_name,count(t.id) as reportcount
  from total_rep t
  group by t.source_name

),curep as (  --群聚個案-找到每筆通報單最新版本
  select id ,max(revise_history) as revise_history
  from cluster_idv_report r
  group by id
),total_crep as (  --群聚個案-找到範圍內的通報單
  select r.id,r.residence_county,r.source
  from cluster_idv_report r
  join curep u on u.id=r.id and u.revise_history=r.revise_history
  where nvl(r.deleted,0) = 0
  and r.cdc_received_day >= :startDate
  and r.cdc_received_day <= :endDate
),sum_crep as (  --群聚個案-依照 通報來源 先做gorup by
  select t.source,count(t.id) as reportcount
  from total_crep t
  group by t.source

),all_source as (  --全部的通報來源
  select id as source,name as source_name,0 as reportCount
  from report_source
  where nvl(deleted,0) = 0
) --合併顯示(全部的通報來源+法傳個案summary+群聚個案summary)
select a.source,
a.source_name,
r.reportCount,
cr.reportcount as clusterReportCount
from all_source a
left join sum_rep r on r.source_name=a.source_name
left join sum_crep cr on cr.source=a.source
order by a.source_name
";

        }

    }

}

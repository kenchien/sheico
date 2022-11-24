namespace MaintainReport {

    /// <summary>
    /// 陽轉
    /// </summary>
    public class Sheet4 : SheetBase {


        public Sheet4() {
            SheetName = "陽轉";
            SheetIndex = 4;
            DumpStartCell = "A4";
            DrawBorder = false;
            
            DateCell = "B2";
            OldRange = new int[] { 2, 2, 6, 13 };
            NewRange = new int[] { 2, 4, 6, 15 };
            DumpRange = new int[] { 4, 2, 6, 3 };

            SetSql();
        }


        protected override void SetSql() {

            sql = $@"
with base as (  --建立三種陽轉項目
  select 1 as sort,'85' as source,'驗出其他傳染病通報' as source_name from dual
  union all select 2,'84','接觸者檢驗陽性通報' from dual
  union all select 3,'86','群聚個案檢驗陽性通報' from dual

),h85 as (
    select  s.barcode,s.disease_id,s.summary_result,s.is_check_other_disease,s.check_other_disease_id,s.event_time
    from sample_result s
    join report_sample rs on s.barcode=rs.sample_id and s.disease_id=rs.sample_disease_item --找母單,原單+來源非SQMS
    where s.is_check_other_disease=1
    and s.event_time >= :startDate --要用event_time才能跟report對齊,不能用summary_result_Date
    and s.event_time <= :endDate
    and rs.to_report_id is not null --原單
    and rs.create_user!='SQMS'  --排除來源87SQMS陽轉 
  
),h84 as ( --預期84接觸者檢驗陽性
    select distinct (case when  s.SUMMARY_RESULT=5 or s.is_check_other_disease=1 then 1 else 0 end) as c,
    r.source,r.id,rc.contacts_report_id,r.create_date,
    s.barcode,s.SUMMARY_RESULT,s.is_check_other_disease,s.sample_date
    from sample_result s
    join report_contacts rc on rc.sample_id=s.barcode and rc.disease_id = s.disease_id
    left join report r on r.id=rc.contacts_report_id and r.disease=rc.disease_id and r.revise_history=1
    join disease d on nvl(d.deleted,0)=0 and d.id=s.disease_id and d.POSITIVE_TO_REPORT in ('1','2')  --0=不處理,1=檢驗陽性直接通報,2=檢驗陽性待通報
    where s.event_time >= :startDate
    and s.event_time <= :endDate
    and (s.SUMMARY_RESULT=5 or s.is_check_other_disease=1 or r.id is not null)
    
),h86 as (
  select distinct c.id
  from cluster_idv_report c
  join ( select id ,max(revise_history) as revise_history from cluster_idv_report r group by id) u on u.id=c.id and u.revise_history = c.revise_history
  join cluster_idv_report_sample crs on crs.idv_report_id=c.id
  join sample_result_pathogen p on p.barcode=crs.sample_id and p.disease_id=crs.disease_id
  join disease d on d.id = crs.disease_id and nvl(d.deleted,0) = 0
  where nvl(p.pathogen_result,'0') in ('2','3') --必要條件,群聚找檢體陽性,2=陽性,3=其它病原體
  and ( (crs.to_report_id is not null and crs.to_report_system_id is not null) --已成案
        or d.positive_to_report in (1,2) --待成案 (疾病設定能夠陽轉通報)
      )
  and p.pathogen_result_day >= :startDate
  and p.pathogen_result_day <= :endDate
  
),merge_hope as (  --分別找出三種陽轉預期的總量
    select '85' as source,count(0) as totalCount from h85
    
    union all
    select '84',count(0) from h84 where c>0
    
    union all
    select '86',count(0) from h86
),fact as ( --分別找出正常(DATA_COMPLETENESS=0)+待補正(DATA_COMPLETENESS=1)+待通報(檢驗陽性待通報)的 陽轉通報單
  select r.source,r.disease,r.id,r.source_form_id
  from report r
  where r.source in (84,85,86)
  and r.revise_history = 1
  and r.create_date >= :startDate
  and r.create_date <= :endDate

  union all
  select rt.source,rt.disease_id,rt.report_id,rt.original_report_id
  from report_tobe rt
  where rt.is_report is null --is_report null:尚未通報，0:不通報，1:已通報
  and rt.create_date >= :startDate
  and rt.create_date <= :endDate

),merge_fact as ( --合併所有(正常+待補正+待通報)通報單數字
  select to_char(source) as source ,count(0) as totalCount
  from fact
  group by source
)
select '('||b.source||')'||b.source_name as 來源,
h.totalCount as 預期,
--f.totalCount as 實際,
(case when h.totalCount-nvl(f.totalCount,0) <0 then 0 else h.totalCount-nvl(f.totalCount,0) end) as 相差 --如果相減為負,表示LIMS送驗單有再次異動檢驗結果或is_check_other_disease
from base b
left join merge_hope h on h.source=b.source
left join merge_fact f on f.source=b.source
order by b.sort
";

        }


    }

}

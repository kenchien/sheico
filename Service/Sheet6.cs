namespace MaintainReport {

    /// <summary>
    /// 自動研判
    /// </summary>
    public class Sheet6 : SheetBase {


        public Sheet6() {
            SheetName = "自動研判";
            SheetIndex = 6;
            DumpStartCell = "C3";
            DrawBorder = false;

            DateCell = "C2";
            OldRange = new int[] { 2, 3, 10, 8 };
            NewRange = new int[] { 2, 4, 10, 9 };
            DumpRange = new int[] { 4, 3, 10, 3 };

            SetSql();
        }


        protected override void SetSql() {

            sql = $@"
with base_dup as (
    --2.主子單--自動主子單標示中,(子單為確定病例但主單非確定病例)的數量(確定病例為determined_status=5 or 30)
    select d.*
    from report_duplicate d
    where nvl(d.deleted,0) = 0
    and d.is_master=0
    and d.CREATE_USER='SysAdmin'
    and d.create_date >= :startDate
    and d.create_date <= :endDate
), rdv_child as (  --先作一個system_id+determined_status,才好找主子單的確定病例
    select r.disease,rd.determined_status,r.id,r.system_id,urd.revise_history,r.revise_history as rh --注意這邊抓研判的最新版本,並非report
    from report r
    join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
    join unique_report_determined urd on urd.system_id = r.system_id
    join report_determined rd on rd.system_id = urd.system_id and rd.revise_history=urd.revise_history
    join base_dup d on d.slave_system_id=r.system_id
    where nvl(r.deleted,0) = 0
    and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
    and rd.determined_status=30
), rdv_parent as (  --先作一個system_id+determined_status,才好找主子單的確定病例
    select r.disease,rd.determined_status,r.id,r.system_id,urd.revise_history,r.revise_history as rh --注意這邊抓研判的最新版本,並非report
    from report r
    join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
    join unique_report_determined urd on urd.system_id = r.system_id
    join report_determined rd on rd.system_id = urd.system_id and rd.revise_history=urd.revise_history
    join base_dup d on d.master_system_id=r.system_id
    where nvl(r.deleted,0) = 0
    and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
    and (rd.determined_status!=30 and rd.determined_status!=5)
)

--1.主子單--所有自動主子單標示的主單數量(排除手動)
select count(distinct slave_system_id) as totalCount
from report_duplicate d
where nvl(d.deleted,0) = 0
and is_master=1
and d.CREATE_USER='SysAdmin'
and d.create_date >= :startDate
and d.create_date <= :endDate

union all
--2.主子單--自動主子單標示中,(子單為確定病例但主單非確定病例)的數量(確定病例為determined_status=5 or 30)
select count(distinct d.master_system_id) as totalCount
--d.*,par.determined_status,child.determined_status
from base_dup d
join rdv_child child on child.system_id=d.slave_system_id
join rdv_parent par on par.system_id=d.master_system_id

union all
--3.自動研判--確定病例的數量
select count(distinct d.system_id) as totalCount
from report_determined d
inner join unique_report_determined ud on ud.system_id = d.system_id and ud.revise_history = d.revise_history
inner join report_determined_log g on g.SYSTEM_ID=d.system_id and g.has_disease_log=1 --境外移入有異動
where d.determined_type=2 --自動研判
and d.determined_status=30 --30=確定病例,22=極可能病例,11=可能病例,31=不符合疾病分類,0=尚無研判結果
and d.created >= :startDate
and d.created <= :endDate

union all
--4.自動研判--自動研判境外移入的通報單數量
select count(distinct rd.system_id)
--r.id,trunc(rd.created) as rdate,rd.created,rd.system_id,rd.revise_history,rd.determined_disease,rd.determined_status,rd.infected_country,rd.infected_source,rd.creator
from report_determined rd
join unique_report_determined urd on urd.system_id = rd.system_id and urd.revise_history=rd.revise_history
join report r on r.system_id=rd.system_id
join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
where nvl(r.deleted,0) = 0
and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
and rd.determined_type=2 --自動研判
and rd.creator = 'AutoDeter' --使用者後來單純改境外移入時,determined_type還是會維持為自動,只能靠這個欄位判斷
and rd.created >= :startDate
and rd.created <= :endDate
and rd.INFECTED_SOURCE=2 --0=無需研判,1=本土病例,2=境外移入,3=經疫調後無法研判,4=特殊場域

union all
--5.自動研判--(需研判感染來源疾病)通報單有國外旅遊史或國外居住史的數量(個案研判為確定/極可能/可能並且有國外旅遊史或國外居住史)
--參考disease.IDENTIFY_INFECTING_SOURCE=1 是否研判感染來源
--參考report.HAS_TRAVEL_HISTORY  是否有旅遊史
select count(distinct rd.system_id)
--r.id,d.IDENTIFY_INFECTING_SOURCE,trunc(rd.created) as rdate,rd.created,rd.system_id,rd.revise_history,rd.determined_disease,rd.determined_status,rd.infected_country,rd.infected_source
from report_determined rd
join unique_report_determined urd on urd.system_id = rd.system_id and urd.revise_history=rd.revise_history
join report r on r.system_id=rd.system_id
join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
inner join disease d on d.id=r.disease and nvl(d.deleted,0)=0
where nvl(r.deleted,0) = 0
and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
and exists (select system_id from report_travel t where t.system_id=rd.system_id and t.travel_type in ('1','2'))
and rd.determined_type=2 --自動研判
and rd.creator = 'AutoDeter' --使用者後來單純改境外移入時,determined_type還是會維持為自動,只能靠這個欄位判斷
and rd.infected_source is not null
and rd.created >= :startDate
and rd.created <= :endDate
and rd.determined_status in ('30','22','11') --30=確定病例,22=極可能病例,11=可能病例,31=不符合疾病分類,0=尚無研判結果
and d.IDENTIFY_INFECTING_SOURCE=1

union all
--6.自動研判--自動研判死因相關性的數量
select count(distinct rd.system_id) as totalCount
--rd.*
from report_determined rd
inner join report_determined_log g on g.SYSTEM_ID=rd.system_id and g.has_death_log=1 --死因相關性有異動
join report r on r.system_id=rd.system_id
join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
inner join disease d on d.id=r.disease and nvl(d.deleted,0)=0
where nvl(r.deleted,0) = 0
and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
and d.IDENTIFY_DEATH_RELATE=1
and rd.determined_type=2 --自動研判
and rd.DEATH_RELATE=1 --0.否 1.是 2.無須研判
and rd.created >= :startDate
and rd.created <= :endDate

union all
--7.自動研判--(需研判死因相關性疾病)通報單「是否死亡」為「是」的數量(個案研判為確定/極可能/可能並且個案死亡)
--參考disease.IDENTIFY_DEATH_RELATE=1 是否自動研判死因相關性
select count(distinct rd.system_id) as totalCount
--rd.*
from report_determined rd
inner join report r on r.system_id=rd.system_id
join unique_report ur on ur.system_id=r.system_id and ur.revise_history=r.revise_history
inner join disease d on d.id=r.disease and nvl(d.deleted,0)=0
where nvl(r.deleted,0) = 0
and r.create_date > to_date('2021/9/5','yyyy/mm/dd')
and rd.determined_type=2 --自動研判
and rd.determined_status in ('30','22','11')
and r.DEATH=1
and d.IDENTIFY_DEATH_RELATE=1
and rd.created >= :startDate
and rd.created <= :endDate


union all
--8.自動研判群聚--已判陽性的通報單數量
select count(distinct r.id) as totalCount --r.*,d.determined_result
from unique_cluster_report c
inner join cluster_report r on r.id=c.id and r.revise_history = c.revise_history
inner join unique_cluster_report_determined ur on ur.cluster_report_id = c.id
inner join cluster_report_determined d on ur.cluster_report_id = d.cluster_report_id and
              ur.revise_history = d.revise_history
where d.determined_result = 1
and d.created >= :startDate
and d.created <= :endDate
";


        }


    }

}

using Common.Helper;
using MaintainReport.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.NetworkInformation;

namespace MaintainReport {

    /// <summary>
    /// 主動告警(目前只針對IDA_0302/IDA_0303/IDA_0304)
    /// </summary>
    public class Alarm {

        /// <summary>
        /// console版本,只發主動告警(目前只針對IDA_0302/IDA_0303/IDA_0304)
        /// </summary>
        /// <param name="dbconn"></param>
        /// <returns>成功寄出幾筆(如果失敗會全部重寄)</returns>
        public int AlarmSendMail(string dbconn) {

            var Alarm = GetData(dbconn, GetSql(), null);
            if (Alarm?.Count == 0) return 0;

            string toRecipients = ConfigurationManager.AppSettings["AlarmMailTo"]?.ToString();
            if (string.IsNullOrEmpty(toRecipients)) throw new Exception("no find AlarmMailTo in config");

            //setup subject
            NetworkHelper networkHelper = new NetworkHelper();
            var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
            var temp = localIp.IndexOf(".31") > 0 ? "測試機" : "正式機";

            MailHelper mailHelper = new MailHelper(dbconn);

            //ken,防意外發生,以這個功能為例子,每小時檢驗有新的就寄送,原則不可能超過30筆,當一次須寄送超過300筆,直接輸出error
            if (Alarm.Count > 300) throw new Exception("此功能寄信原則不可能超過300筆,請檢查sql和資料表");


            //setup content and send mail
            foreach (var item in Alarm) {
                string mailSubject = $@"{item.title}";

                string content = $@"發生時間={item.request_time:yyyy-MM-dd HH:mm:ss}
通報單號={item.report_id}
送驗單號={item.sample_id}
錯誤訊息={item.error_message}

參數={item.param}

此信件為自動發信,請勿回覆.
                ";

                //mailHelper.SendMailAsync(toRecipients, mailSubject, content);
                mailHelper.SendMail(toRecipients, mailSubject, content);

            }

            return 1;
        }

        virtual protected string GetSql() {

            var sql = $@"
select request_time,
'NIDRS即時告警_'||api||'_'||substr(error_message,1,30)||'...' as title,
report_id,
sample_id,
parameters as param,
error_message
from log_api
where user_id='LIMS'
and api in ('IDA_0302','IDA_0303','IDA_0304')
and response_status!='200'
and request_time >= :startDate
--and request_time <= :endDate
order by request_time desc
            ";

            return sql;
        }


        //virtual protected DataTable GetDataTable(string conn, string sql, object param = null) {
        //    DapperBase dapperBase = new DapperBase(conn);
        //    var dt = dapperBase.GetDataTable(sql, param);

        //    return dt;
        //}

        virtual protected List<AlarmModel> GetData(string conn, string sql, object param = null) {
            DapperBase dapperBase = new DapperBase(conn);
            var dt = dapperBase.GetList<AlarmModel>(sql, param);

            return dt;
        }



    }

}

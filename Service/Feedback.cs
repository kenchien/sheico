using Common.Helper;
using MaintainReport.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Net.NetworkInformation;

namespace MaintainReport {

    /// <summary>
    /// 問題反映
    /// </summary>
    public class Feedback {

        /// <summary>
        /// console版本,只發問題反映
        /// </summary>
        /// <param name="dbconn"></param>
        /// <returns>成功寄出幾筆(如果失敗會全部重寄)</returns>
        public int FeedbackSendMail(string dbconn) {

            string toRecipients = ConfigurationManager.AppSettings["FeedbackMailTo"]?.ToString();
            if (string.IsNullOrEmpty(toRecipients)) throw new Exception("no toRecipients");

            //var dtFeedback = GetDataTable(dbconn, GetSql(), null);
            //if (dtFeedback?.Rows?.Count == 0) throw new Exception("don't have any feedback data to send.");
            var Feedback = GetData(dbconn, GetSql(), null);
            if (Feedback?.Count == 0) return 0;

            //setup subject
            NetworkHelper networkHelper = new NetworkHelper();
            var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
            var temp = localIp.IndexOf(".31") > 0 ? "測試機" : "正式機";
            string mailSubject = $@"NIDRS系統問題反映 ({temp}) {DateTime.Now:yyyy-MM-dd}";

            MailHelper mailHelper = new MailHelper(dbconn);

            //ken,防意外發生,以這個功能為例子,每小時檢驗有新的就寄送,原則不可能超過30筆,當一次須寄送超過30筆,直接輸出error
            if (Feedback.Count > 30) throw new Exception("此功能寄信原則不可能超過30筆,請檢查sql和資料表");


            //setup content and send mail
            foreach (var item in Feedback) {

                string content = $@"問題反應時間={item.created_date:yyyy-MM-dd HH:mm:ss}
問題反映類型={item.feedback_type}
問題反映內容={item.feedback_content}
部門={item.filled_by_name}
名稱={item.filled_by_dept}
聯絡電話={item.contact_tel}
手機號碼={item.mobile_phone}
Email={item.email}

此信件為自動發信,請勿回覆.
                ";

                //mailHelper.SendMailAsync(toRecipients, mailSubject, content);
                mailHelper.SendMail(toRecipients, mailSubject, content);

            }

            //一次多筆更新
            return ExecSql(dbconn, UpdateSql());

        }

        virtual protected string GetSql() {

            var sql = $@"
select to_char(f.created_date,'yyyy/mm/dd hh24:mi:ss') as created_date,
    f.feedback_type as feedback_type,
	CAST(f.feedback_content AS VARCHAR2(2000)) as feedback_content,
	f.filled_by_name,
	f.filled_by_dept,
	f.contact_tel as contact_tel,
	f.mobile_phone as mobile_phone,
	f.email
from FEEDBACK f
where f.sended = 0
order by f.created_date desc
            ";

            return sql;
        }


        virtual protected DataTable GetDataTable(string conn, string sql, object param = null) {
            DapperBase dapperBase = new DapperBase(conn);
            var dt = dapperBase.GetDataTable(sql, param);

            return dt;
        }

        virtual protected List<FeedbackModel> GetData(string conn, string sql, object param = null) {
            DapperBase dapperBase = new DapperBase(conn);
            var dt = dapperBase.GetList<FeedbackModel>(sql, param);

            return dt;
        }

        /// <summary>
        /// 一次多筆更新
        /// </summary>
        /// <returns></returns>
        virtual protected string UpdateSql() {

            var sql = $@"
update FEEDBACK
set sended=1
where sended=0
            ";

            return sql;
        }

        virtual protected int ExecSql(string conn, string sql, object param = null) {
            DapperBase dapperBase = new DapperBase(conn);
            return dapperBase.ExecSql(sql);
        }

    }

}

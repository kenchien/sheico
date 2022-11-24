using Common.Helper;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;

namespace MaintainReport {

    public class ShareFunction {
        private ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ShareFunction() {

        }

        /// <summary>
        /// 設定資料庫連線(預設) + 設定log4net寫入DB
        /// </summary>
        public string GetConnectionString() {
            var localConn = Environment.GetEnvironmentVariable("connection", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(localConn)) {
                Log.Info($"machine connection setting is null");
            }
            //連線字串,先找local設定,沒有的話就讀取app.config.SmartIDAConnection
            var conn = string.IsNullOrEmpty(localConn) ?
                    ConfigurationManager.ConnectionStrings["SmartIDAConnection"].ToString()
                    : localConn;


            //設定log4net寫入DB
            // 如果未設定,表示儲存於相同DB
            var hier = (Hierarchy)LogManager.GetRepository();
            if (hier != null) {
                var adoNetAppenders = hier.GetAppenders().OfType<log4net.Appender.AdoNetAppender>();
                foreach (var adoNetAppender in adoNetAppenders) {
                    if (string.IsNullOrEmpty(adoNetAppender.ConnectionString?.Trim())) {
                        adoNetAppender.ConnectionString = conn;
                        adoNetAppender.ConnectionType = typeof(Oracle.ManagedDataAccess.Client.OracleConnection).AssemblyQualifiedName;
                        adoNetAppender.ActivateOptions();
                    }
                }
            }

            return conn;
        }

        /// <summary>
        /// 底層是發信Helper,這邊只是把要傳進去的參數整理好
        /// </summary>
        /// <param name="dbconn"></param>
        /// <param name="outputFilename"></param>
        /// <returns></returns>
        public bool SendMailWithDefaultSetting(string dbconn, string outputFilename) {
            try {
                string toRecipients = ConfigurationManager.AppSettings["MailTo"]?.ToString();

                NetworkHelper networkHelper = new NetworkHelper();
                var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
                string mailSubject = $@"[NIDRS]系統指標 ({localIp}) {DateTime.Now:yyyy-MM-dd}";

                string content = "此信件為自動發信,請勿回覆.\r\n內容請參考附件";

                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(outputFilename);

                MailHelper mailHelper = new MailHelper(dbconn);
                //mailHelper.SendMailAsync(toRecipients, mailSubject, content, false, attachment);
                mailHelper.SendMail(toRecipients, mailSubject, content, false, attachment);
                return true;
            } catch (Exception ex) {
                Log.Error(ex);
                return false;
            }
        }

    }

}

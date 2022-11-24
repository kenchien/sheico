using log4net;
using MaintainReport.Models;
using System;
using System.Net.Mail;
using System.Reflection;
using System.Threading;

namespace Common.Helper {
   /// <summary>
   /// 寄信的動作和一般呼叫函數一樣，程式會等動作做完後﹝確定送到mail server﹞才繼續往下跑，
   /// 如果一次要寄出多封的郵件，將變得很沒效率﹝程式會卡住﹞，最好能採用非同步的方式來執行程式
   /// (PS:不過用非同步要注意收攏結果,不是把信送出就當作成功)先把全部改為同步送信
   /// </summary>
   public class MailHelper {

      private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      public delegate bool SendMailDelegate(string toRecipients,
                                              string mailSubject,
                                              string content,
                                              bool useBCC = false,
                                              Attachment attachment = null);

      public string ConnectionString { get; set; }

      protected string smtpServer;
      protected string mailSender;
      protected int smtpPort;
      protected bool smtpCredential;
      protected bool needSSL;
      protected string loginUser;
      protected string loginPassword;
      protected string mailSenderDisplay;

      public MailHelper() {

      }

      public MailHelper(string connectionString) {
         ConnectionString = connectionString;
      }

      /// <summary>
      /// 讀取發信設定(如果資料庫異常,則直接讀取固定參數來發信)
      /// </summary>
      protected bool GetMailSetting() {
         //依序為
         //SMTP_MAIL_NEED_CREDENTIAL=SMTP MAIL NEED CREDENTIAL SSL
         //SMTP_MAIL_NEED_SSL=SMTP MAIL NEED SSL
         //SMTP_MAIL_SENDER=郵件發送帳號
         //SMTP_MAIL_SENDER_DISPLAY=郵件發送帳號顯示名稱
         //SMTP_MAIL_SERVER_PASSWORD=SMTP MAIL SERVER PASSWORD
         //SMTP_MAIL_SERVER_PORT=SMTP MAIL SERVER PORT
         //SMTP_MAIL_SERVER_URL=SMTP MAIL SERVER URL
         //SMTP_MAIL_SERVER_USER=SMTP MAIL SERVER USER
         //SMTP_MAIL_STATIC_SEND_TIME=郵件發送固定時間

         try {
#if DEBUG
            //資料庫異常,直接讀取固定參數設定做發信
            smtpServer = "smtp.gmail.com";
            mailSender = "andisand@gmail.com";
            smtpPort = 587;//25;//587;
            smtpCredential = true;
            needSSL = true;
            loginUser = "andisand@gmail.com";
            loginPassword = "kzpzlmnvbalqltsn";
            mailSenderDisplay = $@"傳染病通報系統(NIDRS)";//郵件發送帳號顯示名稱
            return true;
#endif

            //ken,改成讀取db的SmartIdaConfig
            string sql = $@"
select DATA_KEY,DATA_VALUE from SMARTIDA_CONFIG
where DATA_KEY in
('SMTP_MAIL_SENDER','SMTP_MAIL_SERVER_URL',
'SMTP_MAIL_SERVER_PORT','SMTP_MAIL_SERVER_USER',
'SMTP_MAIL_SERVER_PASSWORD','SMTP_MAIL_NEED_SSL',
'SMTP_MAIL_NEED_CREDENTIAL','SMTP_MAIL_STATIC_SEND_TIME',
'SMTP_MAIL_SENDER_DISPLAY')
order by DATA_KEY
";
            DapperBase dapperBase = new DapperBase(ConnectionString);
            var idaConfigs = dapperBase.GetList<SmartIdaConfig>(sql);


            bool.TryParse(idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_NEED_CREDENTIAL").DATA_VALUE, out smtpCredential);
            bool.TryParse(idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_NEED_SSL").DATA_VALUE, out needSSL);
            mailSender = idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SENDER").DATA_VALUE;
            mailSenderDisplay = idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SENDER_DISPLAY").DATA_VALUE;
            loginPassword = idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SERVER_PASSWORD").DATA_VALUE;
            int.TryParse(idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SERVER_PORT").DATA_VALUE, out smtpPort);
            smtpServer = idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SERVER_URL").DATA_VALUE;
            loginUser = idaConfigs.Find(x => x.DATA_KEY == "SMTP_MAIL_SERVER_USER").DATA_VALUE;
            return true;
         } catch (Exception) {

            //資料庫異常,直接讀取固定參數設定做發信
            smtpServer = "192.168.171.212";
            mailSender = "cdcnidrs@service.cdc.gov.tw";
            smtpPort = 25;
            smtpCredential = false;
            needSSL = false;
            loginUser = "";
            loginPassword = "";
            mailSenderDisplay = $@"傳染病通報系統(NIDRS)";//郵件發送帳號顯示名稱
            return false;
         }
      }



      /// <summary>
      /// 發信簡單版
      /// </summary>
      /// <param name="toRecipients">多人用逗號分開</param>
      /// <param name="mailSubject">主旨</param>
      /// <param name="content">內容</param>
      /// <param name="attachment">一個附件</param>
      public bool SendMail(string toRecipients,
                              string mailSubject,
                              string content,
                              bool useBCC = false,
                              Attachment attachment = null) {
         try {
            bool readDbMailSettingIsOk = GetMailSetting();
            if(!readDbMailSettingIsOk)
               Log.Error($@"read db mail setting error!");

            Log.Info($@"ready send mail to {toRecipients}, mailSubject={mailSubject}");
            Log.Info($"......smtpServer={smtpServer},smtpPort={smtpPort},mailSender={mailSender},mailSenderDisplay={mailSenderDisplay}");
            Log.Info($"......needSSL={needSSL},smtpCredential={smtpCredential},loginUser={loginUser},loginPassword=no display");

            using (MailMessage msg = new MailMessage()) {
               msg.From = new MailAddress(mailSender, mailSenderDisplay, System.Text.Encoding.UTF8);
               if (useBCC)
                  msg.Bcc.Add(toRecipients);
               else
                  msg.To.Add(toRecipients);

               msg.Subject = mailSubject;
               msg.SubjectEncoding = System.Text.Encoding.UTF8;
               msg.Body = content;
               msg.IsBodyHtml = false;
               msg.BodyEncoding = System.Text.Encoding.UTF8;
               msg.Priority = MailPriority.Normal;

               if (attachment != null)
                  msg.Attachments.Add(attachment);

               using (SmtpClient client = new SmtpClient(smtpServer, smtpPort)) {
                  client.DeliveryMethod = SmtpDeliveryMethod.Network;
                  if (smtpCredential)
                     client.Credentials = new System.Net.NetworkCredential(loginUser, loginPassword);
                  if (needSSL)
                     client.EnableSsl = true;
                  client.Send(msg);
               }//using (SmtpClient client = new SmtpClient(smtpServer, smtpPort)) {

               msg.Attachments.Dispose();
               msg.Dispose();
            }//using (MailMessage msg = new MailMessage()) {

            Log.Info("done, successful send mail.");
            return true;
         } catch (Exception ex) {
            Log.Error("SendMail Error", ex);
            return false;
         }
      }

      /// <summary>
      /// 非同步寄信
      /// </summary>
      /// <param name="toRecipients"></param>
      /// <param name="mailSubject"></param>
      /// <param name="content"></param>
      /// <param name="useBCC"></param>
      /// <param name="attachment"></param>
      /// <returns></returns>
      public bool SendMailAsync(string toRecipients,
                                  string mailSubject,
                                  string content,
                                  bool useBCC = false,
                                  Attachment attachment = null) {
         try {
            SendMailDelegate dc = new SendMailDelegate(this.SendMail);
            IAsyncResult result = dc.BeginInvoke(toRecipients, mailSubject, content, useBCC, attachment, null, null);

            #region //可以用EndInvoke等待非同步的結果
            //// Poll while simulating work.
            //while (result.IsCompleted == false) {
            //    Thread.Sleep(250);
            //    Console.Write(".");
            //}

            //var res = dc.EndInvoke(result);
            #endregion
            return true;
         } catch (Exception ex) {
            Log.Error("SendMailAsync Error", ex);
            return false;
         }
      }
   }

}

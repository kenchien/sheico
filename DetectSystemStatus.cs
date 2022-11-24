using Common.Helper;
using log4net;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaintainReport {
   /// <summary>
   /// 偵測系統運作情況 (console版本)
   /// </summary>
   public class DetectSystemStatus {
      protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

      public DetectSystemStatus() {

         Log.Info($"偵測系統運作情況 (console版本) Version:{Assembly.GetExecutingAssembly().GetName().Version}");
      }


      /// <summary>
      /// 首頁公告_頁面讀取(打api確認NIDRS IIS是否正常運作)
      /// </summary>
      public async Task CheckLogin() {
         string dbconn = "";
         try {
            //1.read setting
            ConfigurationManager.RefreshSection("appSettings");
            dbconn = new ShareFunction().GetConnectionString();//設定資料庫連線(預設) + 設定log4net寫入DB
            string toRecipients = ConfigurationManager.AppSettings["MailTo"]?.ToString();

            string apiurl = ConfigurationManager.AppSettings["CheckUrl"]?.ToString();
            string jsonBody = $@"{{""ClientCode"":""r"",""ActionCode"":""search_ontop_anno"",""RequestData"":{{""ID"":null,""ANNO_TYPE"":null,""TITLE"":null,""CONTNET"":null,""ON_TOP"":true,""PUBLISH_DATE"":null,""PUBLISHER"":null,""PUBLISHER_NAME"":null,""ATTATCH_FILE_INFOS"":null,""DELETED"":null,""CREATE_USER"":null,""CREATE_DATE"":null,""DISABLE_DATE"":null,""ENABLED"":null,""ANNO_STATUS"":null}}}}";
            Log.Info($"準備進行 NIDRS_首頁公告_頁面讀取CheckLogin,Url={apiurl}");

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var httpClient = new HttpClient();
            //var jsonBody = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var httpResponse = await httpClient.PostAsync(apiurl, httpContent);

            if (httpResponse.Content != null) {
               var res = await httpResponse.Content.ReadAsStringAsync();

               if (res.IndexOf("ResponseMSG") > 0 && res.IndexOf("Ok") > 0) {
                  Log.Info("讀取首頁公告--正常");
               } else {
                  Log.Error($@"讀取首頁公告--異常(沒有ResponseMSG和Ok),Msg={res}");
                  SendMail(dbconn, errCode: "1", errMsg: "首頁公告回應訊息有錯誤", content: res);
               }
               return;
            }// if (httpResponse.Content != null) {

            Log.Error("讀取首頁公告--異常(無回應httpResponse.Content)");

            //ken,另一種
            //var client = new RestClient("https://example.com/?urlparam=true");
            //var request = new RestRequest(Method.Post);
            //request.AddHeader("content-type", "application/json");
            //request.AddHeader("cache-control", "no-cache");
            //request.AddJsonBody(jsonBody);
            //var response = client.Execute(request);
            //dynamic jsonResponse = Newtonsoft.Json.Linq.JObject.Parse(result.Content);


         } catch (Exception ex) {
            Log.Error(ex);
            SendMail(dbconn, errCode: "99", errMsg: $@"讀取首頁公告--異常({ex.Message})", content: ex.StackTrace + ex.InnerException?.StackTrace);
         }
      }


      /// <summary>
      /// 發異常告警信件
      /// (設定都是讀取DB,如果DB掛掉,會讀取固定參數做發信)
      /// </summary>
      /// <param name="dbconn"></param>
      /// <param name="mailList"></param>
      /// <param name="extraSubject"></param>
      /// <param name="errCode"></param>
      /// <param name="errMsg"></param>
      /// <param name="content"></param>
      /// <returns></returns>
      public bool SendMail(string dbconn, string mailList = "AlarmMailTo",
                           string extraSubject = "發生異常", string errCode = "", string errMsg = "", string content = "") {
         try {

            //1.get mail list
            string toRecipients = ConfigurationManager.AppSettings["AlarmMailTo"]?.ToString();
            if (string.IsNullOrEmpty(toRecipients)) throw new Exception($@"no toRecipients(appsetting.{mailList})");

            //2.setup subject
            NetworkHelper networkHelper = new NetworkHelper();
            var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
#if DEBUG
            var machineName = "localhost(debug)";
#else
            var machineName = localIp.IndexOf(".31") > 0 ? "測試機" : "正式機";
#endif
            string mailSubject = $@"NIDRS[{localIp}]{extraSubject}({errCode}){errMsg} {DateTime.Now}";


            //3.read mail setting from db smartida_config
            MailHelper mailHelper = new MailHelper(dbconn);


            //4.mailHelper.SendMailAsync(toRecipients, mailSubject, content);
            return mailHelper.SendMail(toRecipients, mailSubject, content);

         } catch (Exception ex) {
            Log.Error(ex);
            return false;
         }
      }

   }
}

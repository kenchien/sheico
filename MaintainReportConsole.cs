using Common.Helper;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;

namespace MaintainReport {
    /// <summary>
    /// CDC_系統指標 (console版本)
    /// 目前作法是直接套範本輸出excel
    /// </summary>
    public class MaintainReportConsole {
        protected readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MaintainReportConsole() {

            Log.Info($"CDC_系統指標 (console版本) Version:{Assembly.GetExecutingAssembly().GetName().Version}");
        }


        /// <summary>
        /// make report to excel
        /// </summary>
        public void ExecuteJob() {
            try {
                //0.設定參數
                string dbconn = new ShareFunction().GetConnectionString();//設定資料庫連線(預設) + 設定log4net寫入DB
                DateTime startDate = DateTime.Today.AddDays(-1).AddHours(17);//.ToString("yyyy-MM-dd HH:mm:ss");
                DateTime endDate = DateTime.Today.AddHours(17);//.ToString("yyyy-MM-dd HH:mm:ss");
                string templateFilename = Path.GetFullPath(ConfigurationManager.AppSettings["TemplatePathName"]?.ToString());
                string oldTemplate = templateFilename.ToLower().Replace(".xlsx", $@"_{startDate:yyyyMMdd}.xlsx");

                NetworkHelper networkHelper = new NetworkHelper();
                var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
                string outputTemp = $"NIDRS系統指標_{localIp.Replace(".", "_")}_{endDate:yyyyMMdd}.xlsx";
                string outputFilename = Path.Combine(ConfigurationManager.AppSettings["OutputPath"]?.ToString(), outputTemp);

                //1.開啟範本檔
                //2.逐一撈取資料並填入Excel sheet
                //3.save
                //4.move template to temp file, and new file replace template
                Report report = new Report();
                var result = report.CreateExcel(dbconn, startDate, endDate, templateFilename, oldTemplate, outputFilename);
                if (string.IsNullOrEmpty(result))
                    return;

                #region //5.send mail
                if (ConfigurationManager.AppSettings["NeedSendMail"]?.ToString() == "1") {
                    if (new ShareFunction().SendMailWithDefaultSetting(dbconn, outputFilename))
                        Log.Info($"五.(Extra)額外將檔案打包發信,發送成功.");
                } else {
                    Log.Info($"五.app.config設定不發信(NeedSendMail != 1).");
                }
                #endregion

                #region //6.(額外發信)問題反映清單,是否發送,1=發信,0=不發
                if (ConfigurationManager.AppSettings["NeedFeedback"]?.ToString() == "1") {
                    int sendCount = new Feedback().FeedbackSendMail(dbconn);
                    if (sendCount <= 0) {
                        Log.Info("六.(Extra)don't have any feedback data to send.");
                    } else {
                        Log.Info($"六.(Extra)額外將問題反映清單發信,成功發送{sendCount}筆.");
                    }

                } else {
                    Log.Info($"六.app.config設定不發信問題反映(NeedFeedback != 1).");
                }
                #endregion
            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 會進來這邊就是console param=feedback,直接發信
        /// </summary>
        public void Feedback() {
            try {
                ConfigurationManager.RefreshSection("appSettings");

                string dbconn = new ShareFunction().GetConnectionString();//設定資料庫連線(預設) + 設定log4net寫入DB

                //不管設定,會進來這邊就是console param=feedback,直接發信
                int sendCount = new Feedback().FeedbackSendMail(dbconn);
                if (sendCount <= 0) {
                    Log.Info("問題反映清單發信,don't have any feedback data to send.");
                } else {
                    Log.Info($"問題反映清單發信,成功發送{sendCount}筆.");
                }

            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 會進來這邊就是console param=alarm,直接發告警信
        /// </summary>
        public void Alarm() {
            try {
                ConfigurationManager.RefreshSection("appSettings");

                string dbconn = new ShareFunction().GetConnectionString();//設定資料庫連線(預設) + 設定log4net寫入DB

                //不管設定,會進來這邊就是console param=feedback,直接發信
                int sendCount = new Alarm().AlarmSendMail(dbconn);
                if (sendCount <= 0) {
                    Log.Info("問題反映清單發信,don't have any feedback data to send.");
                } else {
                    Log.Info($"問題反映清單發信,成功發送{sendCount}筆.");
                }

            } catch (Exception ex) {
                Log.Error(ex);
            }
        }

    }
}

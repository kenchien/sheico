using Common.Helper;
using log4net;
using System;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows.Forms;

namespace MaintainReport {
    /// <summary>
    /// CDC_系統指標 (winform版本)
    /// 目前作法是直接套範本輸出excel
    /// </summary>
    public partial class MaintainReport : Form {
        private readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MaintainReport() {
            InitializeComponent();

            Log.Info($"CDC_系統指標 (winform版本). Version:{Assembly.GetExecutingAssembly().GetName().Version}");

            txtConn.Text = new ShareFunction().GetConnectionString();
            txtStartDate.Text = DateTime.Today.AddDays(-1).AddHours(17).ToString("yyyy-MM-dd HH:mm:ss");
            txtEndDate.Text = DateTime.Today.AddHours(17).ToString("yyyy-MM-dd HH:mm:ss");
            txtFilename.Text = ConfigurationManager.AppSettings["TemplatePathName"]?.ToString();
            txtOutput.Text = ConfigurationManager.AppSettings["OutputPath"]?.ToString();
        }



        /// <summary>
        /// make report to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateExcel_Click(object sender, EventArgs e) {


            try {
                #region //0.檢查和設定參數
                labMsg.Text = "";
                if (string.IsNullOrEmpty(txtConn.Text)) { ShowMsg("連線字串不能為空"); return; }
                if (string.IsNullOrEmpty(txtStartDate.Text)) { ShowMsg("起始日不能為空"); return; }
                if (string.IsNullOrEmpty(txtEndDate.Text)) { ShowMsg("結束日不能為空"); return; }
                if (!File.Exists(txtFilename.Text)) { ShowMsg($"no find template,path={txtFilename.Text}"); return; }

                string dbconn = txtConn.Text;
                DateTime startDate = DateTime.Parse(txtStartDate.Text);
                DateTime endDate = DateTime.Parse(txtEndDate.Text);
                string templateFilename = Path.GetFullPath(txtFilename.Text);
                string oldTemplate = templateFilename.ToLower().Replace(".xlsx", $@"_{startDate:yyyyMMdd}.xlsx");

                NetworkHelper networkHelper = new NetworkHelper();
                var localIp = networkHelper.GetLocalIp(NetworkInterfaceType.Ethernet);
                string outputTemp = $"NIDRS系統指標_{localIp.Replace(".", "_")}_{endDate:yyyyMMdd}.xlsx";
                string outputFilename = Path.Combine(txtOutput.Text, outputTemp);
                #endregion

                //1.開啟範本檔
                //2.逐一撈取資料並填入Excel sheet
                //3.save
                //4.move template to temp file, and new file replace template
                Report report = new Report();
                var result = report.CreateExcel(dbconn, startDate, endDate, templateFilename, oldTemplate, outputFilename);
                if (string.IsNullOrEmpty(result))
                    return;
                ShowMsg($"三.成功產生Excel,路徑={outputFilename}");
                ShowMsg($"四.(Extra)範本檔更新成功,舊範本搬移到{oldTemplate}");


                #region //5.send mail
                if (ConfigurationManager.AppSettings["NeedSendMail"]?.ToString() == "1") {
                    if (new ShareFunction().SendMailWithDefaultSetting(dbconn, outputFilename)) {
                        Log.Info($"五.(Extra)額外將檔案打包發信,發送成功.");
                        ShowMsg($"五.(Extra)額外將檔案打包發信,發送成功.");
                    }
                } else {
                    Log.Info($"五.app.config設定不發信(NeedSendMail != 1).");
                    ShowMsg($"五.app.config設定不發信(NeedSendMail != 1).");
                }
                #endregion

                #region //6.(額外發信)問題反映清單,是否發送,1=發信,0=不發
                if (ConfigurationManager.AppSettings["NeedFeedback"]?.ToString() == "1") {
                    int sendCount = new Feedback().FeedbackSendMail(dbconn);
                    if (sendCount <= 0) {
                        Log.Info("六.(Extra)don't have any feedback data to send.");
                        ShowMsg("六.(Extra)don't have any feedback data to send.");
                    } else {
                        Log.Info($"六.(Extra)額外將問題反映清單發信,成功發送{sendCount}筆.");
                        ShowMsg($"六.(Extra)額外將問題反映清單發信,成功發送{sendCount}筆.");
                    }
                } else {
                    Log.Info($"六.app.config設定不發信問題反映(NeedFeedback != 1).");
                    ShowMsg($"六.app.config設定不發信問題反映(NeedFeedback != 1).");
                }
                #endregion
            } catch (Exception ex) {
                Log.Error(ex);
                MessageBox.Show(ex.ToString());//顯示詳細錯誤訊息
            }
        }

        private void ShowMsg(string msg) {
            //Log.Info(msg);
            labMsg.Text += msg + "\r\n";
        }
    }
}

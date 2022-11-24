using Common.Helper;
using log4net;
using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace MaintainReport {

    /// <summary>
    /// 產生報表
    /// </summary>
    public class Report {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Report() { }


        virtual public string CreateExcel(string dbconn,
                                            DateTime startDate,
                                            DateTime endDate,
                                            string templateFilename,
                                            string oldTemplate,
                                            string outputFilename) {
            ExcelHelper excelHelper = null;
            int maxSheetLength = 10;//從0開始,到10共11個sheet
            try {
                //1.開啟範本檔
                excelHelper = new ExcelHelper(templateFilename);
                excelHelper.PrintHeaders = false;//撈取的data table,輸出時不要輸出欄位名稱
                Log.Info($"一.開啟範本檔{templateFilename},準備寫入{maxSheetLength}個sheet資料");

                //2.逐一撈取資料並填入Excel sheet
                for (int k = 0; k < maxSheetLength + 1; k++) {
                    ISheet sh = null;
                    try {
                        //2.1逐一呼叫對應到的class
                        Type thisType = Type.GetType($"MaintainReport.Sheet{k}");//注意要完整className
                        Object obj = Activator.CreateInstance(thisType);//動態CreateInstance
                        if (obj == null) continue;//沒有這個class,其實會直接跳error,這邊多一手防範
                        sh = (obj as ISheet);
                        sh.ConnectionString = dbconn;

                        //2.2 先搬移前一天的資料(如果同一天,就更新不搬移)
                        //2.3 寫入 今天日期 到指定的標題欄位
                        if (!string.IsNullOrEmpty(sh.DateCell) && sh.OldRange != null && sh.NewRange != null) {
                            if (excelHelper.WBook.Worksheets[k].Cells[sh.DateCell].Value.ToString() != endDate.ToString("MM-dd")) {
                                excelHelper.CopyRange(k, sh.OldRange, sh.NewRange);//先搬移前一天的資料
                                                                                   //} else {
                                                                                   //    excelHelper.ClearFormulaValues(k, sh.DumpRange);//清除左邊一天區塊的資料就好 (只清除值不清除格式)
                            }

                            excelHelper.SetExcelValue(sh.DateCell, endDate.ToString("MM-dd"), k);//寫入左邊一天區塊的標題日期
                        }//if (!string.IsNullOrEmpty(sh.DateCell) && sh.OldRange != null && sh.NewRange != null) {

                        //2.4 do something before write data 這邊要清除昨天留下的異常資料 A3:H503 = {3, 1, 503, 8 }
                        if (sh.ClearRange != null)
                            excelHelper.DeleteRow(k, sh.ClearRange[0], sh.ClearRange[2]);

                        //2.5 塞資料之前,先把欄位清空,避免資料沒有覆蓋成功
                        //excelHelper.Clear(k, sh.DumpRange, true);//清掉全部後,再把格線畫回去
                        excelHelper.ClearFormulaValues(k, sh.DumpRange);//並不會把值清掉

                        //2.6 get data table
                        DataTable dt = sh.GetDataTable(new { startDate, endDate });

                        //2.7 write data table into excel
                        if (dt?.Rows?.Count == 0 && !sh.SkipNoDataError)//必要輸出結果發現無資料時,拋出錯誤
                            throw new Exception("查無資料");
                        if (dt?.Rows?.Count > 0)
                            excelHelper.LoadDataTable(dt, k, sh.DumpStartCell, sh.DrawBorder);

                        //2.8 當DumpStartCell2 != null,則多呼叫GetDataTable2處理第二個data table
                        //先合併處理,所以目前沒有輸出2個data table的情況,之後有需要再說

                        Log.Info($"二.{k}. write {sh.SheetName} data into excel,count={dt?.Rows?.Count}");
                    } catch (Exception ex1) {
                        if (sh != null)
                            Log.Error($"二.{k}. {sh.SheetName} error,msg={ex1.Message}");
                        else
                            Log.Error($"二.{k}. no find sheet{k}");
                    }
                }//for (int k = 1; k < 10; k++) {

                //3.save
                FileInfo fileinfo = new FileInfo(outputFilename);
                excelHelper.SaveExcel(fileinfo);
                Log.Info($"三.成功產生Excel,路徑={outputFilename}");

                //4.move template to temp file, and new file replace template
                File.Copy(templateFilename, oldTemplate, true);
                File.Copy(outputFilename, templateFilename, true);
                Log.Info($"四.(Extra)範本檔更新成功,舊範本搬移到{oldTemplate}");

                return outputFilename;
            } catch (Exception ex) {
                Log.Error(ex);
                throw ex;
            } finally {
                if (excelHelper != null)
                    excelHelper.Dispose();
            }
        }


    }

}

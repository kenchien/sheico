namespace MaintainReport.Models {
    /// <summary>
    /// 輸出報表時,很多設定可以統一寫在db table,改動時候方便(搭配ExcelHelper使用)
    /// </summary>
    public class SettingReportModel {
        public string ReportId { get; set; }
        public string ReportName { get; set; }//no use,just memo
        public string ServiceName { get; set; }//動態切換class
        public string SourceFunc { get; set; }//動態切換function
        public bool IsDataTable { get; set; }//1=取資料的函數回傳型態為dataTable,0=List<T>
        public string TemplateName { get; set; }//excel範本檔,不含路徑

        public int SheetIndex { get; set; }
        public string DataTableStartCell { get; set; }
        public bool PrintHeaders { get; set; }

        public string Header1 { get; set; }
        public string HeaderPos1 { get; set; }
        public string Header2 { get; set; }
        public string HeaderPos2 { get; set; }

        public int DrawBorder { get; set; }
    }

}
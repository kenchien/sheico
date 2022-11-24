using MaintainReport.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Common.Helper {
    /// <summary>
    /// excel 套範本輸出
    /// </summary>
    public class ExcelHelper {
        /// <summary>
        /// 表頭
        /// </summary>
        private List<Title> _titles { get; } = new List<Title>();


        public ExcelPackage ExcelDoc { get; set; } = null;//ken,元件沒寫好之前,先開放外面直接設定
        /// <summary>
        /// 是否輸出資料標題
        /// </summary>
        public bool PrintHeaders { get; set; } = false;
        public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


        public SettingReportModel SettingReport { get; set; }

        public ExcelWorkbook WBook { get { return ExcelDoc.Workbook; } }//ken,元件沒寫好之前,先開放外面直接設定

        public ExcelHelper(string templateName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileInfoXls = new FileInfo(templateName);
            ExcelDoc = new ExcelPackage(fileInfoXls);
        }

        public ExcelHelper(List<Title> titles, string path, bool printDataTitle = true) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fileInfoXls = new FileInfo(path);
            ExcelDoc = new ExcelPackage(fileInfoXls);
            _titles.AddRange(titles);
            PrintHeaders = printDataTitle;
        }



        /// <summary>
        /// 設定表頭
        /// </summary>
        /// <param name="t"></param>
        public void AddTitle(Title t) {
            _titles.Add(t);
        }

        public void SetPrintTitle(bool printDataTitle) {
            PrintHeaders = printDataTitle;
        }

        private ExcelWorksheet GetSheet(int sheetIndex) {
            if (ExcelDoc.Workbook.Worksheets.Count == 0)
                return ExcelDoc.Workbook.Worksheets.Add("sheet1");
            else if (ExcelDoc.Workbook.Worksheets.Count == 1)
                return ExcelDoc.Workbook.Worksheets[0];
            else
                return ExcelDoc.Workbook.Worksheets[sheetIndex];
        }

        /// <summary>
        /// LoadDataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="printCell"></param>
        /// <param name="drawBorder"></param>
        public void LoadDataTable(DataTable dt, int sheetIndex = 0, string printCell = "A1", bool drawBorder = true) {
            var sheet = GetSheet(sheetIndex);

            //輸出表頭
            foreach (var t in _titles) {
                sheet.Cells[t.Cell].Value = t.Text;
            }

            if (dt.Rows.Count == 0)
                throw new Exception("無資料可以輸出");

            //貼上資料
            sheet.Cells[printCell].LoadFromDataTable(dt, PrintHeaders);

            if (drawBorder)
                WriteBorder(sheet, dt.Rows.Count, dt.Columns.Count);
        }

        /// <summary>
        /// LoadDataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheet"></param>
        /// <param name="printCell"></param>
        /// <param name="drawBorder"></param>
        public void LoadDataTable(DataTable dt, ExcelWorksheet sheet, string printCell = "A1", bool drawBorder = true) {
            //ken,EPPLUS extension直接貼資料的函數還有
            //LoadFromCollection<T>(IEnumerable < T > Collection, bool PrintHeaders);
            //LoadFromDataTable(DataTable Table, bool PrintHeaders);
            //LoadFromArrays(IEnumerable<object[]> Data);
            //LoadFromText(string Text, ExcelTextFormat Format, TableStyles TableStyle, bool FirstRowIsHeader);
            //LoadFromText(FileInfo TextFile, ExcelTextFormat Format, TableStyles TableStyle, bool FirstRowIsHeader);
            //LoadFromText(string Text);

            //貼上資料
            sheet.Cells[printCell].LoadFromDataTable(dt, PrintHeaders);

            if (drawBorder)
                WriteBorder(sheet, dt.Rows.Count, dt.Columns.Count);
        }

        /// <summary>
        /// LoadDataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="printCell"></param>
        /// <param name="drawBorder"></param>
        public void LoadFromCollection<T>(List<T> list, int sheetIndex = 0, string printCell = "A1", bool drawBorder = true)
            where T : new() {
            var sheet = GetSheet(sheetIndex);

            //輸出表頭
            foreach (var t in _titles) {
                sheet.Cells[t.Cell].Value = t.Text;
            }

            //貼上資料
            sheet.Cells[printCell].LoadFromCollection(list, PrintHeaders);

            var tCount = list.FirstOrDefault().GetType().GetProperties().Count();

            if (drawBorder)
                WriteBorder(sheet, list.Count, tCount);
        }

        /// <summary>
        /// LoadDataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sheet"></param>
        /// <param name="printCell"></param>
        /// <param name="drawBorder"></param>
        public void LoadFromCollection<T>(List<T> list, ExcelWorksheet sheet, string printCell = "A1", bool drawBorder = true)
            where T : new() {
            //貼上資料
            sheet.Cells[printCell].LoadFromCollection(list, PrintHeaders);

            var tCount = list.FirstOrDefault().GetType().GetProperties().Count();

            if (drawBorder)
                WriteBorder(sheet, list.Count, tCount);

        }


        /// <summary>
        /// 整個表格畫上格線
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="RowCount">dt.Rows.Count</param>
        /// <param name="ColCount">dt.Columns.Count</param>
        /// <param name="StartRowIndex">-1 = Int32.Parse(SettingReport.DataTableStartCell.Substring(1))</param>
        /// <param name="PrintHeaders">-1 = (SettingReport.PrintHeaders ? 0 : 1)</param>
        public void WriteBorder(ExcelWorksheet sheet, int RowCount, int ColCount, int StartRowIndex = -1, int PrintHeaders = -1) {
            int fromX, fromY, toX, toY;
            int _startRowIndex, _printHeaders;

            //整個表格畫上格線
            if (SettingReport != null) {
                _startRowIndex = StartRowIndex == -1 ? Int32.Parse(SettingReport.DataTableStartCell.Substring(1)) : StartRowIndex;
                _printHeaders = PrintHeaders == -1 ? (SettingReport.PrintHeaders ? 0 : 1) : PrintHeaders;
            } else {
                _startRowIndex = StartRowIndex == -1 ? 1 : StartRowIndex;
                _printHeaders = PrintHeaders == -1 ? 1 : PrintHeaders;
            }
            fromX = _startRowIndex - _printHeaders;
            fromY = 1;//A
            toX = fromX + RowCount;
            toY = fromY + ColCount - 1;


            if (fromX > 0 && fromY > 0 && toX > 0 && toY > 0) {
                var modelTable = sheet.Cells[fromX, fromY, toX, toY];
                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            }
        }


        /// <summary>
        ///於指定位置塞入資料
        /// </summary>
        /// <param name="cell">指定格</param>
        /// <param name="cellValue">值</param>
        /// <param name="sheetName">sheet 名稱</param>
        public void SetExcelValue(string cell, string cellValue, int sheetIndex = 0) {
            var currentSheet = GetSheet(sheetIndex);

            currentSheet.Cells[cell].Value = cellValue;
        }

        /// <summary>
        /// 以dataTable 貼上資料
        /// </summary>
        /// <param name="dt">批量資料</param>
        /// <param name="printCell">指定格</param>
        /// <param name="sheetName">指定 sheet</param>
        /// <exception cref="Exception"></exception>
        public void PasteData(DataTable dt, string printCell, int sheetIndex = 0) {
            if (dt.Rows.Count == 0)
                throw new Exception("無資料可以貼上");

            var currentSheet = GetSheet(sheetIndex);

            currentSheet.Cells[printCell].LoadFromDataTable(dt, PrintHeaders);
        }

        /// <summary>
        /// 以 List 貼上資料
        /// </summary>
        /// <param name="pasteData"></param>
        /// <param name="printCell"></param>
        /// <param name="sheetName"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="Exception"></exception>
        public void PasteData<T>(List<T> pasteData, string printCell, int sheetIndex = 0) where T : new() {
            if (pasteData.Count == 0)
                throw new Exception("無資料可以貼上");

            var currentSheet = GetSheet(sheetIndex);

            currentSheet.Cells[printCell].LoadFromCollection(pasteData, PrintHeaders);
        }

        /// <summary>
        /// 將一個區塊的資料複製到另一區塊(包含格式)
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="OldRange"></param>
        /// <param name="NewRange"></param>
        public void CopyRange(int sheetIndex, int[] OldRange, int[] NewRange) {
            var currentSheet = GetSheet(sheetIndex);

            currentSheet.Cells[OldRange[0], OldRange[1], OldRange[2], OldRange[3]]
                .Copy(currentSheet.Cells[NewRange[0], NewRange[1], NewRange[2], NewRange[3]]);

        }

        public void DeleteRow(int sheetIndex, int rowForm, int rows) {
            var currentSheet = GetSheet(sheetIndex);

            currentSheet.DeleteRow(rowForm, rows);
        }

        /// <summary>
        /// 清除一個區塊的資料 (包含值+公式+格式)
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="range"></param>
        public void Clear(int sheetIndex, int[] range, bool drawBorder = false) {
            if (range == null) return;
            var currentSheet = GetSheet(sheetIndex);
            currentSheet.Cells[range[0], range[1], range[2], range[3]].Clear();
            
            if (drawBorder) {
                //WriteBorder(currentSheet, range[2] - range[0] + 1, range[3] - range[1] + 1, range[0]);
                var modelTable = currentSheet.Cells[range[0], range[1], range[2], range[3]];
                modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            }

        }

        /// <summary>
        /// 清除一個區塊的公式 (只清除公式)
        /// </summary>
        /// <param name="sheetIndex"></param>
        /// <param name="range"></param>
        public void ClearFormulaValues(int sheetIndex, int[] range) {
            if (range == null) return;
            var currentSheet = GetSheet(sheetIndex);
            currentSheet.Cells[range[0], range[1], range[2], range[3]].ClearFormulaValues();//清除公式
        }

        public class Title {
            /// <summary>
            /// 表頭輸出欄位
            /// </summary>
            public string Cell { get; set; }

            /// <summary>
            /// 表頭文字
            /// </summary>
            public string Text { get; set; }
        }


        /// <summary>
        /// 直接輸出excel
        /// </summary>
        /// <returns></returns>
        public byte[] ExportExcel() {
            var file = ExcelDoc.GetAsByteArray();
            ExcelDoc.Dispose();

            return file;
        }

        public void SaveExcel(FileInfo filename) {
            ExcelDoc.SaveAs(filename);
            ExcelDoc.Dispose();
        }

        public void Dispose() {
            try {
                if (ExcelDoc != null)
                    ExcelDoc.Dispose();
            } catch { }
        }
    }
}
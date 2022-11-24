namespace MaintainReport {

    /// <summary>
    /// 通報
    /// </summary>
    public class Sheet0 : SheetBase {

        public Sheet0() {
            SheetName = "總表";
            SheetIndex = 0;
            //DumpStartCell = "";
            DrawBorder = false;

            SkipNoDataError = true;//跳過sql

            DateCell = "D1";
            OldRange = new int[] { 4, 4, 7, 9 };
            NewRange = new int[] { 4, 5, 7, 10 };
            DumpRange = new int[] { 4, 4, 7, 4 };//當天填入的範圍(左上右下)

        }

        protected override void SetSql() {

            sql = $@"";

        }
    }

}

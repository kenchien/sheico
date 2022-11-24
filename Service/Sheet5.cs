namespace MaintainReport {

    /// <summary>
    /// 檢驗
    /// </summary>
    public class Sheet5 : SheetBase {


        public Sheet5() {
            SheetName = "檢驗";
            SheetIndex = 5;
            DumpStartCell = "B3";
            DrawBorder = false;

            SkipNoDataError = true;

            DateCell = "";
            //OldRange = new int[] { 2, 2, 6, 13 };
            //NewRange = new int[] { 2, 4, 6, 15 };

            SetSql();
        }

        protected override void SetSql() {

            sql = $@"select '人工比對' from dual
";

        }


    }

}

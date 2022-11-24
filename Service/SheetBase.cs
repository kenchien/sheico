using Common.Helper;
using System.Data;

namespace MaintainReport {

    /// <summary>
    /// 其它介接
    /// </summary>
    abstract public class SheetBase : ISheet {
        public string SheetName { get; set; }
        public string ConnectionString { get; set; }
        public int SheetIndex { get; set; }
        public string DumpStartCell { get; set; }
        public bool DrawBorder { get; set; } = false;
        public string DateCell { get; set; }

        protected string sql;

        public bool SkipNoDataError { get; set; } = false;


        public string DumpStartCell2 { get; set; }
        public int[] OldRange { get; set; }
        public int[] NewRange { get; set; }
        public int[] DumpRange { get; set; }
        public int[] ClearRange { get; set; }


        public SheetBase() { }

        public SheetBase(string cnnectionString, int sheetIndex, string dumpStartCell, bool drawBorder) {
            ConnectionString = cnnectionString;
            SheetIndex = sheetIndex;
            DumpStartCell = dumpStartCell;
            DrawBorder = drawBorder;

            SetSql();
        }

        virtual public DataTable GetDataTable(object param = null) {
            if (string.IsNullOrEmpty(sql)) return null;
            
            DapperBase dapperBase = new DapperBase(ConnectionString);
            var dt = dapperBase.GetDataTable(sql, param);

            //also can direct use dapper ,but need extra to transfer to dataTable
            //var res = cn.Query(sql,new {startDate,endDate});

            return dt;
        }

        abstract protected void SetSql();


        virtual public DataTable GetDataTable2(object param = null) {
            return null;
        }



    }

}

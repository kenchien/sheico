using Common.Helper;
using System.Data;

namespace MaintainReport {
    public interface ISheet {
        string SheetName { get; set; }
        string ConnectionString { get; set; }
        int SheetIndex { get; set; }
        string DumpStartCell { get; set; }
        bool DrawBorder { get; set; }

        bool SkipNoDataError { get; set; }


        string DateCell { get; set; }
        int[] OldRange { get; set; }
        int[] NewRange { get; set; }
        int[] DumpRange { get; set; }
        int[] ClearRange { get; set; }

        DataTable GetDataTable(object param = null);

        //當DumpStartCell2 != null,則多呼叫GetDataTable2處理第二個data table
        string DumpStartCell2 { get; set; }
        DataTable GetDataTable2(object param = null);


    }

}

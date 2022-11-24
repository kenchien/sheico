using System;

namespace MaintainReport.Models {

    public class AlarmModel {

        public DateTime request_time { get; set; }
        public string title { get; set; }
        public string report_id { get; set; }
        public string sample_id { get; set; }
        public string param { get; set; }
        public string error_message { get; set; }

    }

}

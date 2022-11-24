using System;

namespace MaintainReport.Models {

    public class FeedbackModel {

        public DateTime created_date { get; set; }
        public string feedback_type { get; set; }
        public string feedback_content { get; set; }
        public string filled_by_name { get; set; }
        public string filled_by_dept { get; set; }
        public string contact_tel { get; set; }
        public string mobile_phone { get; set; }
        public string email { get; set; }
    }

}

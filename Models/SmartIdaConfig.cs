using System;

namespace MaintainReport.Models {

    public class SmartIdaConfig {
        /// <summary>
        /// Data Key, 因 `KEY` 是 Oracle關鍵字
        /// 不可以爲 null
        /// </summary>
        public string DATA_KEY { get; set; }
        /// <summary>
        /// 生效時間
        /// 不可以爲null
        /// </summary>
        public DateTime? ENABLE_DATE { get; set; }
        /// <summary>
        /// Data Value
        /// 所有『值』都是『字串』形式；若有需要，前端必須轉型到其他豈料型別
        /// </summary>
        public string DATA_VALUE { get; set; }
        /// <summary>
        /// 中文描述
        /// </summary>
        public string DESCRIPTION { get; set; }
        /// <summary>
        /// 建立者
        /// </summary>
        public string CREATE_USER { get; set; }
        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CREATE_DATE { get; set; }
        /// <summary>
        /// 資料類型
        /// IDAConfig研判用
        /// </summary>
        public string DATA_TYPE { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? SORT_ORDER { get; set; }
    }

}

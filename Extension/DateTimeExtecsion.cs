using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MaintainReport.Extension {
    public static class DateTimeExtecsion {
        public static string ToTaiwanDateTime(this DateTime dt, string format) {

            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();

            string result = dt.ToString(format, culture);

            //ken,系統問題,format寫yyMM或是yMM 基本就只會回yyyMM,自己過濾調整吧(先用很笨拙的方式吧)
            string finalResult = result;
            if (format.Substring(0, 2) == "yy" && format.Substring(2, 1) != "y")
                finalResult = result.Substring(1);

            return finalResult;

        }

        public static string ToTaiwanDateTime(this string d, string format) {
            bool re = DateTime.TryParse(d, out DateTime dt);
            if (re)
                return dt.ToTaiwanDateTime(format);
            else
                return null;
        }

        public static string DateTimeSplitSymbol(this string d, char s) {
            string re = string.Empty;
            List<string> words = d.Split(s).ToList();

            foreach (string w in words) {
                re += w;
            }

            return re;
        }

        public static string NoSplit(this string source) {
            if (string.IsNullOrEmpty(source)) return null;

            return source.Replace("/", string.Empty);
        }
    }
}
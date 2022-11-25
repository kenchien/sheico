using System.Linq;

namespace Clothes.Extension {
    public static class DecimalExtension {
        //數字轉換為中文
        public static string ToChinese(this decimal inputNum) {
            string[] intArr = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            string[] strArr = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九", };
            string[] Chinese = { "", "十", "百", "千", "萬", "十", "百", "千", "億" };
            //金額
            //string[] Chinese = { "元", "十", "百", "千", "萬", "十", "百", "千", "億" };
            char[] tmpArr = inputNum.ToString().ToArray();
            string tmpVal = "";
            for (int i = 0; i < tmpArr.Length; i++) {
                tmpVal += strArr[tmpArr[i] - 48];//ASCII編碼 0為48
                tmpVal += Chinese[tmpArr.Length - 1 - i];//根據對應的位數插入對應的單位
            }

            return tmpVal;
        }
    }
}
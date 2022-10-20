// @Author: futianfu
// @Date: 2021/8/4 18:21:56

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace WLCore.Helper
{
    public static class StringHelper
    {
        /// <summary>
        /// 判断字符串是否存在 (Contains )
        /// </summary>
        /// <param name="str"> 要检测的原字符串【例：test.meta】 </param>
        /// <param name="strContains"> 是否存在的部分【例：es】 </param>
        /// <returns> 返回判定结果【例：true】 </returns>
        public static bool Contains(string str, string strContains)
        {
            return str.Contains(strContains);
        }

        /// <summary>
        /// 换名
        /// </summary>
        /// <param name="name"> 原名 </param>
        /// <param name="oldChar"> 要替换掉的部分 </param>
        /// <param name="newChar"> 替换成什么 </param>
        /// <returns> 返回转换结果 </returns>
        public static string Replace(string name, string oldChar, string newChar)
        {
            return name.Replace(oldChar, newChar);
        }

        /// <summary>
        /// 分割字符串 (Split) ,支持根据字符和字符串分割【例：Split("Assets/GameAssets/Common", "/")】
        /// </summary>
        /// <param name="str"> 原路径 </param>
        /// <param name="splitStr"> 用什么内容分割【例： "/"】 </param>
        /// <returns> 分割后的数组【例：{"Assets","GameAssets","Common"}】 </returns>
        public static string[] Split(string str, string splitStr)
        {
            return str.Split(new string[] { splitStr }, System.StringSplitOptions.None);
        }

        /// <summary>
        /// 字符串判空
        /// </summary>
        /// <param name="str"> 要判定的字符串【例：""】 </param>
        /// <returns>  【例：true，为空】 </returns>
        public static bool IsNull(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 数字长度不够则在前面补0，如FixNumber(20, 4) -> 0020
        /// </summary>
        /// <param name="num"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string FixNumber(int num, int length)
        {
            string format = "{0:D" + length + "}";
            return String.Format(format, num);
        }

        /// <summary>
        /// 将数字变为带逗号字符串，如1234->1,234、1234567->1,234,567
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string NumberWithComma(int num)
        {
            return num.ToString("N0");
        }

        /// <summary>
        /// 将字节B大小转换大小，超过MB则显示MB，否则超过kb显示kb，最后显示B；保留一位小数位
        /// </summary>
        /// <param name="byteSize"> 单位字节 </param>
        /// <returns></returns>
        public static string GetSize(long byteSize)
        {
            if (byteSize >= 1048576)  // 1024 x 1024
            {
                var r = byteSize / 1048576.0f;
                var t = (int)(r * 10);
                r = t / 10.0f;
                return r.ToString() + "MB";
            }

            if (byteSize >= 1024)
            {
                var r = byteSize / 1024.0f;
                var t = (int)(r * 10);
                r = t / 10.0f;
                return r.ToString() + "KB";
            }

            return byteSize.ToString() + "B";
        }

        /// <summary>
        /// 字符串src不足长度maxlen则用ap补足字符串长度，如StringAppend("23", 'a', 5) -> 23aaa
        /// </summary>
        /// <param name="src">要处理的字符串</param>
        /// <param name="ap">填充的字符</param>
        /// <param name="maxlen">最终的长度</param>
        /// <returns></returns>
        public static string StringAppend(string src, char ap, int maxlen)
        {
            StringBuilder sb = new StringBuilder(src);
            if (src.Length < maxlen)
            {
                sb.Append(ap, maxlen - src.Length);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将ascii码集合转字符串，如[97, 98, 99]  -> "abc"
        /// </summary>
        /// <param name="ascArr"></param>
        /// <returns></returns>
        public static string AsciiToString(byte[] ascArr)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            return asciiEncoding.GetString(ascArr);
        }

        /// <summary>
        /// 解析url 参数
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Parseurl(string url)
        {
            var res = new Dictionary<string, string>();
            var t1  = url.Split(',');
            t1 = t1[0].Split('?');
            if (t1.Length <= 1)
            {
                return res;
            }

            t1 = t1[1].Split('&');

            for (int i = 0; i < t1.Length; i++)
            {
                var t2 = t1[i].Split('=');
                if (1 < t2.Length)
                {
                    res[t2[0]] = t2[1];
                }
            }

            return res;
        }

        /// <summary>
        /// 格式化富文本信息
        /// </summary>
        public static string FormatRichText(string text)
        {
            MatchCollection matchCollection = Regex.Matches(text, @"\[(.*?)]", RegexOptions.IgnoreCase);
            var             sb              = new StringBuilder();
            foreach (Match mc in matchCollection)
            {
                var tempStr = mc.Value.Substring(1, mc.Value.Length - 2);
                var strs    = tempStr.Split('|');
                //与约定格式不匹配不做转换
                if (strs.Length < 3)
                {
                    WLDebug.LogWarning("FormatRichText length error!");
                    continue;
                }

                sb.Clear();
                sb.AppendFormat("<color={0}><size={1}>{2}</size></color>", strs[0], strs[1], strs[2]);
                text = text.Replace(mc.Value, sb.ToString());
            }

            return text;
        }


        #region 字符串操作示例

        //（1）字符访问（下标访问s[i]）
        //s ="ABCD";
        //Console.WriteLine(s[0]); // 输出"A";

        //（3）截取子串（Substring）
        //s ="ABCD";
        //Console.WriteLine(s.Substring(1)); // 从第2位开始（索引从0开始）截取一直到字符串结束，输出"BCD"

        //（4）匹配索引（IndexOf() 和 Contains()）
        //s ="ABCABCD";
        //Console.WriteLine(s.IndexOf('A')); // 从字符串头部开始搜索第一个匹配字符A的位置索引，输出"0"
        //Console.WriteLine(s.IndexOf("BCD")); // 从字符串头部开始搜索第一个匹配字符串BCD的位置，输出"4"
        //Console.WriteLine(s.LastIndexOf('C')); // 从字符串尾部开始搜索第一个匹配字符C的位置，输出"5"
        //Console.WriteLine(s.LastIndexOf("AB")); // 从字符串尾部开始搜索第一个匹配字符串BCD的位置，输出"3"
        //Console.WriteLine(s.Contains("ABCD")); // 判断字符串中是否存在另一个字符串"ABCD"，输出true

        //（5）大小写转换（ToUpper和ToLower）
        //s ="aBcD";
        //Console.WriteLine(s.ToLower()); // 转化为小写，输出"abcd"
        //Console.WriteLine(s.ToUpper()); // 转化为大写，输出"ABCD"

        //（6）填充对齐（PadLeft和PadRight）
        //s ="ABCD";
        //Console.WriteLine(s.PadLeft(6, '_')); // 使用'_'填充字符串左部，使它扩充到6位总长度，输出"__ABCD"
        //Console.WriteLine(s.PadRight(6, '_')); // 使用'_'填充字符串右部，使它扩充到6位总长度，输出"ABCD__"

        //（7）截头去尾（Trim TrimStart TrimEnd）
        //s ="__AB__CD__";
        //Console.WriteLine(s.Trim('_')); // 移除字符串中头部和尾部的'_'字符，输出"AB__CD"
        //Console.WriteLine(s.TrimStart('_')); // 移除字符串中头部的'_'字符，输出"AB__CD__"
        //Console.WriteLine(s.TrimEnd('_')); // 移除字符串中尾部的'_'字符，输出"__AB__CD"

        //（8）插入和删除（Insert和Remove）
        //s ="ADEF";
        //Console.WriteLine(s.Insert(1, "BC")); // 在字符串的第2位处插入字符串"BC"，输出"ABCDEF"
        //Console.WriteLine(s.Remove(1)); // 从字符串的第2位开始到最后的字符都删除，输出"A"
        //Console.WriteLine(s.Remove(0, 2)); // 从字符串的第1位开始删除2个字符，输出"EF"

        //（9）替换字符（串）（Replace）
        //s ="A_B_C_D";
        //Console.WriteLine(s.Replace('_', '-')); // 把字符串中的'_'字符替换为'-'，输出"A-B-C-D"

        //（11）格式化（静态方法Format）
        //Console.WriteLine(string.Format("{0} + {1} = {2}", 1, 2, 1+2));

        //（12）连接成一个字符串（静态方法Concat、静态方法Join和 实例方法StringBuilder.Append）
        //s ="A,B,C,D";
        //string[] arr3 = s.Split(','); // arr = {"A","B","C","D"}
        //Console.WriteLine(string.Concat(arr3)); // 将一个字符串数组连接成一个字符串，输出"ABCD"
        //Console.WriteLine(string.Join(",", arr3)); // 以","作为分割符号将一个字符串数组连接成一个字符串，输出"A,B,C,D"
        //StringBuilder sb = new StringBuilder(); // 声明一个字符串构造器实例
        //sb.Append("A"); // 使用字符串构造器连接字符串能获得更高的性能
        //sb.Append('B');
        //Console.WriteLine(sb.ToString());// 输出"AB"

        #endregion
    }
}

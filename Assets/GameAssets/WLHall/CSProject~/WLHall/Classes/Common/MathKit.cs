// MathKit.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/7/5
using System;
using UnityEngine;

namespace WLFishingHall
{
    public static class MathKit
    {
        /// <summary>
        /// 有些昵称长度可能过长，统一使用该接口转换
        /// </summary>
        /// <param name="nickName">原始昵称</param>
        /// <returns></returns>
        public static string GetNickName(string nickName)
        {
            // 16个字符（8个中文字符）
            return CutStringLength(nickName, 16);
        }

        /// <summary>
        /// 随机一个值在min到max之间的整数，包含min和max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRange(int min, int max)
        {
            System.Random r = new System.Random();
            return r.Next(min, max + 1);
        }

        /// <summary>
        /// value必定在max, min之内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int MaxMin(int value, int min, int max)
        {
            value = Math.Max(value, min);
            return Math.Min(value, max);
        }

        /// <summary>
        /// 就得两点之间的角度（不是弧度）
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double GetAngle(Vector2 start, Vector2 end)
        {
            if (Math.Abs(start.x - end.x) < 0.001 && Math.Abs(start.y - end.y) < 0.001)
            {
                return 0;
            }

            float diff_x = end.x - start.x,
            diff_y = end.y - start.y;
            return 360 * Math.Atan(diff_y / diff_x) / (2 * Math.PI);
        }

        /// <summary>
        /// 将整形转为字符串钱
        /// </summary>
        /// <param name="qian"></param>
        /// <returns></returns>
        /// 示例：10000 -> 1万
        public static string HongFuQianRequest(float qian)
        {
            // 千万不保留小数点
            if (qian > 10000000)
            {
                return Mathf.Floor(qian / 10000f).ToString() + "万";
            }

            // 百万保留一个小数点
            if (qian > 1000000)
            {
                float num = Mathf.Floor(qian / 1000f);
                return (num / 10f).ToString() + "万";
            }

            // 万保留2个小数点
            if (qian > 10000)
            {
                float num = Mathf.Floor(qian / 100f);
                return (num / 100f).ToString() + "万";
            }

            return qian.ToString();
        }

        /// <summary>
        /// 获得字符串长度，中文2个字符，英文1个字符
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetStringLength(string text)
        {
            var length = 0;
            if (string.IsNullOrEmpty(text))
            {
                return length;
            }

            char[] chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var len = (int)chars[i] > 255 ? 2 : 1;
                length += len;
            }
            return length;
        }

        /// <summary>
        /// 剪切字符串长度，用于有中文字符(中文占2个字符)和英文字符混杂情况
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        /// 示例：cutStringLength("我是da傻b", 7) -> 我是da
        public static string CutStringLength(string text, int maxLen)
        {
            var maxLen2 = GetCutStringIndex(text, maxLen);
            return text.Substring(0, maxLen2);
        }

        public static int GetCutStringIndex(string text, int maxLen)
        {
            int index = 0;
            char[] chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                var len = (int)chars[i] > 255 ? 2 : 1;
                index += len;
                if (index > maxLen)
                {
                    return Math.Max(0, i);
                }
            }
            return chars.Length;
        }

        /// <summary>
        /// 超过maxLen长度的，则用replaceString字符替代
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maxLen"></param>
        /// <param name="replaceString"></param>
        /// <returns></returns>
        public static string ReplaceStringLength(string text, int maxLen, string replaceString)
        {
            var realLength = MathKit.GetStringLength(text);
            if (realLength <= maxLen)
            {
                return text;
            }

            var replaceLen = MathKit.GetStringLength(replaceString);
            int len = maxLen - replaceLen;
            string cutSting = MathKit.CutStringLength(text, maxLen);
            return cutSting + replaceString;
        }

        /// <summary>
        /// 已知总数和每行数量，求共有多少行
        /// </summary>
        /// <param name="curIndex"> curIndex当前数量，起始从0开始 </param>
        /// <param name="rowCount"> rowCount每行数量 </param>
        /// <returns> 索引从0开始 </returns>
        public static int GetLineIndex(int curIndex, int rowCount)
        {
            float index = curIndex / rowCount;
            return (int)Math.Floor(index);
        }

        /// <summary>
        /// 已知当前索引和每行显示个数，求竖行索引
        /// </summary>
        /// <param name="curIndex"> curIndex当前数量，起始从0开始 </param>
        /// <param name="rowCount"> rowCount每行数量 </param>
        /// <returns> 索引从0开始 </returns>
        public static int GetRowIndex(int curIndex, int rowCount)
        {
            return curIndex % rowCount;
        }

        /// <summary>
        /// 字符串是否都是中文字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsChineseChar(string text)
        {
            char[] chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if ((int)chars[i] <= 255)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 根据指定长度插入换行符
        /// </summary>
        /// <param name="text"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        /// lineWrapStringWithLength("快点吧,牌在你手上都下崽子了.", 15) ->"快点吧,牌在你手\n上都下崽子了."
        public static string LineWrapStringWithLength(string text, int length)
        {
            int curLength = 0;
            string result = "";
            char[] chars = text.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (curLength >= length)
                {
                    result += "\n";
                    curLength = 0;
                }
                result += chars[i];
                var len = chars[i] > 255 ? 2 : 1;
                curLength += len;
            }
            return result;
        }

        /*
            * 描述：获取position位置
            丨------------- width ----------------丨
            丨                                    丨
            丨space          space          space 丨
            丨----丨position丨----丨position丨---- 丨
            丨                                    丨
            * 参数：posIndex索引从0开始
        */
        public static float PositionWithSpace(int positionCount, float space, float width, int posIndex)
        {
            float spaceWidth = (positionCount + 1) * space;
            float widthCell = (width - spaceWidth) / positionCount;
            float radiusWidth = widthCell / 2;

            return space * (posIndex + 1) + radiusWidth * (posIndex * 2 + 1);
        }

        /*
            * 描述：获取node位置
            丨------------------------------------丨
            丨                                    丨
            丨half  width    space            half丨
            丨---丨  node  丨------丨  node  丨----丨
            丨                                    丨
            * 参数：posIndex索引从0开始
        */
        public static float PositionWithIndex(int posIndex, float width, float space)
        {
            double spaceWidth = (posIndex + 0.5) * space;
            double nodeWidth = (posIndex + 0.5) * width;
            return (float)(spaceWidth + nodeWidth);
        }

        /*====================== 华丽分割线 ======================*/

        private static string _HongFuQianRequest2(float qian)
        {
            // 百万不保留小数点
            if (qian >= 1000000)
            {
                string value = qian / 10000.0f + "";
                var index = _decimalPointIndex(value);
                var num = value.Substring(0, index);
                return num + "万";
            }

            // 万保留2个小数点
            if (qian > 10000)
            {
                string value = qian / 10000.0f + "";
                int index = value.IndexOf('.');
                if (index == -1)
                {
                    return value + "万";
                }
                var num = value.Substring(0, index + 2);
                return num + "万";
            }

            return qian.ToString();
        }

        private static int _decimalPointIndex(string value)
        {
            int index = value.IndexOf('.');
            return index == -1 ? value.Length : index;
        }
    }
}
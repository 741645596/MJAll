// EncodeHelper.cs
// Author: shihongyang <shihongyang@weile.com>
// Data: 2019/4/25
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Unity.Utility
{
    public static class EncodeHelper
    {

        /// <summary>
        /// unicode转中文
        /// </summary>
        /// <returns></returns>
        public static string RegexUnescape(string str)
        {
            //if (string.IsNullOrWhiteSpace(str))
             //   return str;
            //return str;
            return System.Text.RegularExpressions.Regex.Unescape(str);

            
        }

        /// <summary>  
        /// 将字符串进行 unicode 编码  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string UnicodeEncode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            StringBuilder strResult = new StringBuilder();
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = 0; i < str.Length; i++)
                {
                    strResult.Append("\\u");
                    strResult.Append(((int)str[i]).ToString("x4"));
                }
            }
            return strResult.ToString();
        }


        public static string ByteArrayToString(byte[] bytes)
        {
            string hex = BitConverter.ToString(bytes);
            hex = hex.Replace("-", "");
            return hex.ToLower();
        }

        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static Dictionary<string, string> StringGetPair(string str)
        {
            str = str.Trim(new char[] { '\"', '\'' });
            string[] tokens = str.Split(';');
            var result = new Dictionary<string, string>();
            for (int i = 0; i < tokens.Length; i++)
            {
                var pairStr = tokens[i];
                //WLDebug.Log(pairStr);
                var pairToken = pairStr.Split('=');
                if (pairToken.Length == 2)
                {
                    if (result.ContainsKey(pairToken[0]) == false)
                    {
                        result.Add(pairToken[0], pairToken[1]);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 字符串转Unicode
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>Unicode编码后的字符串</returns>
        public static string String2Unicode(string source)
        {
            var bytes = Encoding.Unicode.GetBytes(source);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i += 2) {
                stringBuilder.AppendFormat("\\u{0:x2}{1:x2}", bytes[i + 1], bytes[i]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>      
        /// Unicode字符串转为正常字符串          
        /// </summary>          
        /// <param name="srcText"></param>      
        public static string UnicodeToString(string srcText)
        { 
            string dst = "";
            string src = srcText;
            int len = srcText.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            return dst;
        }

        public static Dictionary<string, string> TableToObject(string str)
        {
            var endIndex = str.LastIndexOf("}");
            var beginIndex = str.IndexOf("{");
            if (endIndex == -1 || beginIndex == -1)
            {
                return null;
            }

            var kvstr = str.Substring(beginIndex + 1, endIndex- beginIndex-1);
            for (var i = 0; i < kvstr.Length; i++)
            {
                kvstr = kvstr.Replace(" ", "");
                kvstr = kvstr.Replace("\"", "");
            }

            var tokens = kvstr.Split(',');
            var count = tokens.Length;

            var obj = new Dictionary<string, string>();
            for (var i = 0; i < count; i++)
            {
                var strs = tokens[i];
                var pair = strs.Split('=');
                if (pair.Length == 2)
                {
                   if(obj.ContainsKey(pair[0]) == false)
                   {
                    obj.Add(pair[0], pair[1]);
                   }
                }
            }

            return obj;
        }

        public static byte[] HexStringToData(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var dataStr = str.ToLower();
            var strLength = str.Length;

            if (strLength > 2 && dataStr[0] == '0' && dataStr[1] == 'x')
            {
                dataStr = dataStr.Substring(2);
            }

            if (strLength % 2 == 1)
            {
                return null;
            }

            var nRealsize = dataStr.Length / 2;
            byte[] buffArray = new byte[nRealsize];

            for (int i = 0; i < nRealsize; i++)
            {
                byte[] array = System.Text.Encoding.ASCII.GetBytes(dataStr[i * 2].ToString());

                int ch1 = array[0];

                if (ch1 >= 48 && ch1 <= 57) // ascii '0':48, '9':57
                {
                    ch1 -= 48;
                }
                else if (ch1 >= 97 && ch1 <= 102) // ascii 'a':97, 'f':102
                {
                    ch1 -= 87;
                }
                else
                {
                    return null;
                }


                array = System.Text.Encoding.ASCII.GetBytes(dataStr[i * 2 + 1].ToString());
                int ch2 = array[0];
                if (ch2 >= 48 && ch2 <= 57)
                {
                    ch2 -= 48;
                }
                else if (ch2 >= 97 && ch2 <= 102)
                {
                    ch2 -= 87;
                }
                else
                {
                    return null;
                }

                byte ch = (byte)(ch1 << 4 | ch2);
                buffArray[i] = ch;
            }

            return buffArray;
        }


        public static string Utf16ToUtf8(string utf16String)
        {
            string utf8String = String.Empty;

            // Get UTF16 bytes and convert UTF16 bytes to UTF8 bytes
            byte[] utf16Bytes = Encoding.Unicode.GetBytes(utf16String);
            byte[] utf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, utf16Bytes);

            // Fill UTF8 bytes inside UTF8 string
            for (int i = 0; i < utf8Bytes.Length; i++)
            {
                // Because char always saves 2 bytes, fill char with 0
                byte[] utf8Container = new byte[2] { utf8Bytes[i], 0 };
                utf8String += BitConverter.ToChar(utf8Container, 0);
            }

            // Return UTF8
            return utf8String;
        }

        public static string UTF8To16(string str)
        {
            string res;
            int i, len, c;
            int char2, char3;
            res = "";
            len = str.Length;
            i = 0;
            while (i < len)
            {
                c = Convert.ToByte(str[i++]);
                switch (c >> 4)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        // 0xxxxxxx
                        res += str.CharAt(i - 1);
                        break;
                    case 12:
                    case 13:
                        // 110x xxxx 10xx xxxx
                        char2 = Convert.ToByte(str[i++]);
                        res += Convert.ToChar(((c & 0x1F) << 6) | (char2 & 0x3F));
                        break;
                    case 14:
                        // 1110 xxxx 10xx xxxx 10xx xxxx
                        char2 = Convert.ToByte(str[i++]);
                        char3 = Convert.ToByte(str[i++]);
                        res += Convert.ToChar(((c & 0x0F) << 12) |
                            ((char2 & 0x3F) << 6) |
                            ((char3 & 0x3F) << 0));
                        break;
                }
            }
            return res;
        }

        public static string UShortArrayToString(ushort[] array)
        {
            var byteList = new List<byte>();
            for (int j = 0; j < array.Length; j++)
            {
                var bytes = BitConverter.GetBytes(array[j]);
                byteList.AddRange(bytes);
            }
            return Encoding.Unicode.GetString(byteList.ToArray());
        }
    }

    public static class CustomCharAt
    {
        /// <summary>
        /// 返回指定位置字符
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="index">字符索引，长度超出时返回：' '</param>
        /// <returns></returns>
        public static char CharAt(this string str, int index)
        {
            if (index > str.Length)
                return ' ';

            string res = str.Substring(index, 1);
            return Convert.ToChar(res);
        }
    }
}

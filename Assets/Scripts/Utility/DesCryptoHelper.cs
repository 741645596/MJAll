// DesCrypto.cs
// Author: shihongyang <shihongyang@weile.com>
// Data: 2019/4/24

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Utility
{
    public static class DesCryptoHelper
    {
        public static byte[] Encrypt(byte[] bytes, byte[] keydata)
        {
            List<byte> result = new List<byte>();
            if (bytes == null)
                return result.ToArray();

            byte[][] k48s = BuildKey(keydata);

            int nLen = bytes.Length;
            for (int i = 0; nLen > 0; i++, nLen -= 8)
            {
                byte[] cdata = new byte[8];
                Array.ConstrainedCopy(bytes, i * 8, cdata, 0, Math.Min(8, nLen));
                byte[] d = Des(cdata, k48s, true);

                result.AddRange(d.ToList());
            }

            return result.ToArray();
        }

        public static byte[] Decrypt(byte[] bytes, byte[] keydata)
        {
            //List<byte> result = new List<byte>();
            byte[] result = null;
            //byte[] keydata = System.Text.Encoding.Default.GetBytes(key);
            byte[][] k48s = BuildKey(keydata);

            int nLen = bytes.Length;
            for (int i = 0; i < Math.Floor(nLen / 8.0); i++)
            {
                byte[] cdata = new byte[8];
                Array.ConstrainedCopy(bytes, i * 8, cdata, 0, 8);
                byte[] d = Des(cdata, k48s, false);
                if (result == null) result = d;
                else
                {
                    int newLen = result.Length + d.Length;
                    byte[] newData = new byte[newLen];
                    Array.ConstrainedCopy(result, 0, newData, 0, result.Length);
                    Array.ConstrainedCopy(d, 0, newData, result.Length, d.Length);
                    result = newData;
                }
                //result.AddRange(d.ToList());
            }

            return result;//result.ToArray();
        }

        private static byte[] Des(byte[] data, byte[][] k48s, bool v)
        {
            byte[] M64 = Chars2bits(data, 64);
            M64 = TransMatrix(M64, IP_Table);
            if (v)
            {
                for (int i = 0; i < 16; i++)
                {
                    byte[] l32 = new byte[32];
                    Array.Copy(M64, l32, 32);
                    byte[] r32 = new byte[32];
                    Array.Copy(M64, 32, r32, 0, 32);
                    byte[] temp = new byte[32];
                    Array.Copy(M64, 32, temp, 0, 32);

                    r32 = FFunc(r32, k48s[i]);
                    r32 = FXOR(r32, l32);

                    var l = temp.ToList();
                    l.AddRange(r32.ToList());
                    M64 = l.ToArray();
                }
            }
            else
            {
                for (int i = 15; i >= 0; i--)
                {
                    byte[] l32 = new byte[32];
                    Array.Copy(M64, l32, 32);
                    byte[] r32 = new byte[32];
                    Array.Copy(M64, 32, r32, 0, 32);
                    byte[] temp = new byte[32];
                    Array.Copy(M64, temp, 32);

                    l32 = FFunc(l32, k48s[i]);
                    l32 = FXOR(l32, r32);

                    var l = l32.ToList();
                    l.AddRange(temp.ToList());
                    M64 = l.ToArray();
                }
            }
            M64 = TransMatrix(M64, IPR_Table);
            //Info(M64);
            return Bits2chars(M64);
        }

        private static byte[] Bits2chars(byte[] bits)
        {
            byte size = (byte)(Math.Floor(bits.Length / 8.0));
            byte[] result = new byte[size];
            for (int i = 0; i < bits.Length; i++)
            {
                byte index = (byte)(Math.Floor(i / 8.0));
                result[index] |= (byte)(bits[i] << (i & 7));
            }
            return result;
        }

        private static byte[] FXOR(byte[] data, byte[] key)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] ^= key[i];
            }

            return data;
        }

        private static byte[] FFunc(byte[] data, byte[] key)
        {
            byte[] MR = TransMatrix(data, E_Table);
            MR = FXOR(MR, key);

            byte[] res = Sfunc(MR);
            return TransMatrix(res, P_Table);
        }

        private static byte[] Sfunc(byte[] data)
        {
            byte[] result = null;
            for (int i = 0, p = 0; i < 8; i++, p += 6)
            {
                int j = data[p + 0] * 2 + data[p + 5];
                int k = data[p + 1] * 8 + data[p + 2] * 4 + data[p + 3] * 2 + data[p + 4];
                int index = i * 16 * 4 + j * 16 + k;
                byte b = (byte)(S_Box[index] + 48);
                byte[] dst = Chars2bits(new byte[] { b }, 4);
                if (result == null) result = dst;
                else
                {
                    int newLen = result.Length + dst.Length;
                    byte[] newData = new byte[newLen];
                    Array.ConstrainedCopy(result, 0, newData, 0, result.Length);
                    Array.ConstrainedCopy(dst, 0, newData, result.Length, dst.Length);
                    result = newData;
                }

//                 var l = result.ToList();
//                 l.AddRange(dst.ToList());
//                 result = l.ToArray();
            }
            return result;
        }

        private static byte[][] BuildKey(byte[] keydata)
        {
            byte[] k64 = Chars2bits(keydata, 64);
            byte[] k56 = TransMatrix(k64, PC1_Table);

//             byte[] l28 = new byte[28];
//             Array.Copy(k56, l28, 28);
//             byte[] r28 = new byte[28];
//             Array.Copy(k56, 28, r28, 0, 28);

            byte[][] k48s = new byte[16][];
            for (int i = 0; i < 16; i++)
            {
                //                 l28 = RotateL(l28, Rotate_Table[i]);
                //                 r28 = RotateL(r28, Rotate_Table[i]);
                // 
                //                 byte[] c = new byte[56];
                //                 Array.Copy(l28, c, 28);
                //                 Array.Copy(r28, 0, c, 28, 28);

                RotateL(k56, 0, 28, Rotate_Table[i]);
                RotateL(k56, 28, 56, Rotate_Table[i]);
                k48s[i] = TransMatrix(k56, PC2_Table);
            }
            return k48s;
        }

        private static byte[] Chars2bits(byte[] keydata, int len)
        {
            byte[] result = new byte[len];

            for (int i = 0; i < len; ++i)
            {
                byte index = (byte)Math.Floor(i / 8.0);
                result[i] = (byte)((keydata[index] >> (i & 7)) & 1);
            }

            return result;
        }

        private static byte[] TransMatrix(byte[] src, byte[] matrix)
        {
            var length = matrix.Length;
            byte[] result = new byte[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = src[matrix[i] - 1];
            }

            return result;
        }

        private static byte[] RotateL(byte[] src, int start, int end, int n)
        {
            //             int len = src.Length;
            // 
            //             for (int j = 0; j < n; j++)
            //             {
            //                 byte temp = src[0];
            //                 for (int i = 0; i < len - 1; i++)
            //                 {
            //                     src[i] = src[i + 1];
            //                 }
            //                 src[len - 1] = temp;
            //             }

            int dataLen = end - start;
            for(int i = 0; i < n; ++i)
            {
                byte v = src[start];
                Array.ConstrainedCopy(src, start + 1, src, start, dataLen - 1);
                src[end - 1] = v;
            }

            return src;
        }

        public static void Info(byte[] data)
        {
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (i % 8 == 0)
                {
                    str += "\n";
                }

                str += data[i];
                if (i < data.Length - 1)
                {
                    str += ", ";
                }
            }

            WLDebug.LogWarning(str);
        }

        private static byte[] PC1_Table = new byte[]
        {
            13, 2,10,50, 1,28,59,32,    30,46,19,63,33,41,24,52,
            27,42,49,18, 9,48,23,58,    31, 8, 7,12, 6, 5, 3,53,
            55,36,57,40,26, 4,44,17,    62,38,15,14,25,56,64,20,
            21,60,11,34,22,29,43,16
        };

        private static byte[] IP_Table = new byte[]
        {
            13, 2,10,50, 1,28,59,32,    30,46,19,63,33,41,24,52,
            27,42,49,18, 9,48,23,58,    31, 8, 7,12, 6, 5, 3,53,
            55,36,57,40,26, 4,44,17,    62,38,15,14,25,56,64,20,
            21,60,11,34,22,29,43,16,    51,35,37,45,54,39,47,61
        };

        private static byte[] Rotate_Table = new byte[]
        {
            1, 1, 2, 2, 2, 2, 2, 2,     2, 2, 2, 2, 2, 2, 2, 1
        };

        private static byte[] PC2_Table = new byte[]
        {
            43, 1,19,20, 3,56,46, 7,    29,30, 5,38,31,16,14,17,
            53, 6,48, 2,18, 8, 9,49,    11,21,37,45,24,12,44,25,
            27,28,26,40,39,23,33, 4,    52,41,47,36,32,50,22,55
        };

        private static byte[] E_Table = new byte[]
        {
            1,27, 4, 8, 2, 9,13,17,    21,22,26,30,24,32,10,23,
            3,15,28,18, 5,31,19,12,    6,16,14,25, 7,11,29,20,
            28,16,18,14,29,15,27,5,    17,21,30,12,19,11, 8,26
        };

        private static byte[] S_Box = new byte[]
        {
            //Part1
            5, 13, 7, 8, 3,10, 4, 1,    14,11,15,12, 2,16, 6, 9,
            11, 8, 5,12,15,14, 1,10,    13, 3, 2, 7, 4, 6,16, 9,
            10, 5, 6, 7,13, 8,12, 4,    16,11, 3, 1, 2, 9,15,14,
            5,  2,11,15,12, 4,10,13,     7, 8, 3,14, 1, 9, 6,16,
            //Part2
            10, 4, 9, 3,14, 6, 5,12,    16, 2, 7, 1,13,15,11, 8,
             7, 4, 8,12,16, 6,10, 3,    13, 5, 1, 2, 9,14,11,15,
             6, 1,16,14,13,10, 5, 4,     8,12, 7, 9,15,11, 3, 2,
             7,10,12, 5, 8, 4,16,13,     3,11, 2,14,15, 9, 1, 6,
            //Part3
            16, 3, 4, 1,14,10,13, 5,    11,15, 9, 6, 8,12, 7, 2,
            12, 8,15, 7, 4, 5, 1,11,    14, 6, 9,16,10, 2,13, 3,
            10, 7,11,15, 6, 4, 5, 2,     3,16, 9,14, 8,13,12, 1,
             3, 6, 5, 4,13, 8,14, 9,     2,12,16, 1, 7,11,15,10,
            //Part4
            15,12, 3,14,10,11,16, 9,     2,13, 5, 8, 6, 1, 4, 7,
            16,12, 3, 4, 9, 5, 7,10,     6, 2, 1,14,15,13, 8,11,
             4,16,12, 2,13, 6,10, 7,     3, 5,15,11, 1, 9, 8,14,
            14, 7, 5, 3, 8, 6,15,11,    10, 1,12, 9, 4,13,16, 2,
            //Part5
             5, 7,14, 4,13,15,11, 1,     3,16, 6, 2, 8,10, 9,12,
             1, 4,15, 3, 9,10,11, 6,     5, 7,13,12, 2,14,16, 8,
            13, 8, 4,16, 5,15,12,14,     9, 1, 3, 7, 6,10,11, 2,
             2, 4,11, 8,16, 9,14, 1,     5,13, 7,10,12,15, 6, 3,
            //Part6
             3, 2,14,16, 1, 8, 7, 4,    13, 5,15,10, 9,12,11, 6,
            10, 9, 4, 2, 7,15,14, 1,     8,11, 5,12, 3, 6,16,13,
             5,10, 8, 4,11, 1, 2, 6,     3,14,16, 9,13, 7,15,12,
             5,14, 3, 6, 4, 2,16,10,    13, 1, 8,12, 7,15, 9,11,
            //Part7
            13,12, 9, 6, 1,15,16, 7,    11, 2, 4, 8, 3,14,10, 5,
            10,15, 5,16,12, 8, 3, 1,     2,11, 4, 9,14,13, 6, 7,
            10, 9,14, 2, 8, 5,11, 1,     4, 7,13, 3, 6,15,16,12,
            10, 3, 5,13, 7, 2, 1,16,    15, 6, 9,11,14, 4,12, 8,
            //Part8
             7,10, 3, 6, 1,11, 9,16,     4, 2,15, 5,13, 8,14,12,
            16,10,14, 6, 2, 8, 9,15,     4,12,11, 5, 7, 1, 3,13,
            16, 5, 9, 6, 4, 8, 3,13,     1,10, 7,11,15,14, 2,12,
            14, 1, 5,10, 2,13, 9,12,     7,11,16, 3, 6, 4, 8,15
        };

        private static byte[] IPR_Table = new byte[]
        {
            5, 2,31,38,30,29,27,26,    21, 3,51,28, 1,44,43,56,
            40,20,11,48,49,53,23,15,    45,37,17, 6,54, 9,25, 8,
            13,52,58,34,59,42,62,36,    14,18,55,39,60,10,63,22,
            19, 4,57,16,32,61,33,46,    35,24, 7,50,64,41,12,47
        };

        private static byte[] P_Table = new byte[]
        {
            13,24,25,31,17,28, 2,29,    14, 3, 6, 9,21,12, 4,22,
            8, 5, 7,26,20,18,15,23,     1,32,11,10,16,27,30,19
        };
    }
}

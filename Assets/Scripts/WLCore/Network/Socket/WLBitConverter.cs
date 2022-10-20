/// <summary>
/// chenzhiwei@weile.com
/// 字节转化辅助
/// </summary>

namespace WLCore
{
    public class WLBitConverter
    {
        public static object bitConverterLock = new object();       // 对象重利用，须线程同步,所有用到getBytes结果的地方全部需要加锁

        private static byte[] bytes1 = new byte[1];
        private static byte[] bytes2 = new byte[2];
        private static byte[] bytes4 = new byte[4];
        private static byte[] bytes8 = new byte[8];

        public static byte[] GetBytes(long value)
        {
            bytes8[0] = (byte)(value & 0xFF);
            bytes8[1] = (byte)((value >> 8) & 0xFF);
            bytes8[2] = (byte)((value >> 16) & 0xFF);
            bytes8[3] = (byte)((value >> 24) & 0xFF);
            bytes8[4] = (byte)((value >> 32) & 0xFF);
            bytes8[5] = (byte)((value >> 40) & 0xFF);
            bytes8[6] = (byte)((value >> 48) & 0xFF);
            bytes8[7] = (byte)((value >> 56) & 0xFF);
            return bytes8;
        }

        public static byte[] GetBytes(ulong value)
        {
            bytes8[0] = (byte)(value & 0xFF);
            bytes8[1] = (byte)((value >> 8) & 0xFF);
            bytes8[2] = (byte)((value >> 16) & 0xFF);
            bytes8[3] = (byte)((value >> 24) & 0xFF);
            bytes8[4] = (byte)((value >> 32) & 0xFF);
            bytes8[5] = (byte)((value >> 40) & 0xFF);
            bytes8[6] = (byte)((value >> 48) & 0xFF);
            bytes8[7] = (byte)((value >> 56) & 0xFF);
            return bytes8;
        }

        public static byte[] GetBytes(int value)
        {
            bytes4[0] = (byte)(value & 0xFF);
            bytes4[1] = (byte)((value >> 8) & 0xFF);
            bytes4[2] = (byte)((value >> 16) & 0xFF);
            bytes4[3] = (byte)((value >> 24) & 0xFF);
            return bytes4;
        }

        public static byte[] GetBytes(uint value)
        {
            bytes4[0] = (byte)(value & 0xFF);
            bytes4[1] = (byte)((value >> 8) & 0xFF);
            bytes4[2] = (byte)((value >> 16) & 0xFF);
            bytes4[3] = (byte)((value >> 24) & 0xFF);
            return bytes4;
        }

        public static byte[] GetBytes(short value)
        {
            bytes2[0] = (byte)(value & 0xFF);
            bytes2[1] = (byte)((value >> 8) & 0xFF);
            return bytes2;
        }

        public static byte[] GetBytes(ushort value)
        {
            bytes2[0] = (byte)(value & 0xFF);
            bytes2[1] = (byte)((value >> 8) & 0xFF);
            return bytes2;
        }

        public static byte[] GetBytes(byte value)
        {
            bytes1[0] = value;
            return bytes1;
        }
    }
}

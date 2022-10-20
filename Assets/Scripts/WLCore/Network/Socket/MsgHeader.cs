// @Author: tanjinhua
// @Date: 2021/3/15  15:08


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.Utility;

namespace WLCore
{
    public class MsgHeader
    {
        public class Flag
        {
            public const byte None = 0;         //!<发送数据不做任何处理
            public const byte Packet = 0x1;     //!<当前为包文件（一个或者多个连续包），每个包都有个包标记@brief msg_Packet 当所有包首发完毕才能处理
            public const byte Mask = 0x2;       //!<检查掩码,@ref    msg_Header::byMask 将当前包的校验码，当前包也会重组数据
            public const byte Encode = 0x4;     //!<重新编码,使用映射表重新编码数据
            public const byte Compress = 0x8;   //!<压缩数据,数据将启用zlib压缩，该标记仅推荐，如果数据压缩后更大，将忽略该标记
            public const byte Route = 0x10;     //!<路由包,服务器每次中转，@ref    msg_Header::byRouteCount 的值累加1，当到达特定值后，仍然没有到达目标服务器，该包将丢弃
            public const byte DelaySend = 0x20; //!<延迟消息，等待下次非延迟消息合并发送（客户端忽略该标记）
            public const byte Filled = 0x40;    //!<已经填充头部（已经进行掩码、编码、压缩处理)，不能重复处理
            public const byte Offset = 0x80;    //!<指定消息头偏移，掩码、编码、压缩处理将从消息头开始偏移@ref    msg_Header::byHeaderOffset 个字节开始
        }

        public enum eEndian
        {
            BIG_ENDIAN = 1,
            LITTLE_ENDIAN = 2,
        }

        protected MemoryStream _stream;

        public ushort len = 0;
        public ushort messageID = 0;
        public byte byMask = 0;
        public byte byFlag = 0;
        public byte byRouteCount = 0;
        public byte byHeaderOffset = 0;
        public long position { get => _stream.Position; set => _stream.Position = value; }
        public long streamLength { get { return _stream.Length; } }
        public MemoryStream stream { get { return _stream; } }

        public static readonly int headerSize = 8;                              // 消息头大小
        private List<byte> _tmpBuffer = new List<byte>(50);                     // 读取字符串时避免每次都去new对象，暂时放在这，有必要再放到统一缓冲池
        private static byte[] _byteCache = new byte[1024];                      // 字节缓冲
        private static readonly uint _maxByte = 1024 * 1024;                    // 超出则提示网络包过大
        private static UnicodeEncoding _unicode = new UnicodeEncoding();
        private static readonly eEndian _endian = eEndian.LITTLE_ENDIAN;        // 默认小端, 正常网络字节序为大端, 这里是为了兼容原有棋牌的网络处理
        private static object _lock = new object();

        public MsgHeader()
        {
            _stream = new MemoryStream();
        }

        public MsgHeader(byte[] buffer)
        {
            _stream = new MemoryStream(buffer);
            position = 0;
        }

        public void Dispose()
        {
            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
                _stream = null;
            }
        }

        public byte[] GetBuffer()
        {
            // 消息头部写入
            WriteHeader();
            return ToArray();
        }

        public byte[] GetSteamBuffer()
        {
            return _stream == null ? null : _stream.GetBuffer();
        }

        /// <summary>
        /// 回收重置变量
        /// </summary>
        public virtual void Reset()
        {
            len = 0;
            messageID = 0;
            byMask = 0;
            byFlag = 0;
            byRouteCount = 0;
            byHeaderOffset = 0;

            _stream.Position = 0;
            _stream.SetLength(0);
        }

        public void ReadHeader()
        {
            _stream.Position = 0;
            len = ReadUint16();
            messageID = ReadUint16();
            byMask = ReadByte();
            byFlag = ReadByte();
            byRouteCount = ReadByte();
            byHeaderOffset = ReadByte();

            _stream.Position = byHeaderOffset;
        }

        public void WriteHeader()
        {
            len = (ushort)_stream.Position;
            _stream.Position = 0;

            WriteUint16(len);
            WriteUint16(messageID);
            WriteByte(byMask);
            WriteByte(byFlag);
            WriteByte(byRouteCount);
            WriteByte(byHeaderOffset);

            _stream.Position = len;
        }

        public byte[] ToArray()
        {
            return _stream.ToArray();
        }

        public void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// 读取指定字节数
        /// </summary>
        /// <param name="c">需要读取的字节数</param>
        /// <returns></returns>
        private byte[] ReadBytes(uint c)
        {
            byte[] a = GetByteCache((int)c);
            if (_endian == eEndian.BIG_ENDIAN)
            {
                for (uint i = 0; i < c; ++i)
                {
                    a[c - 1 - i] = (byte)_stream.ReadByte();
                }
            }
            else
            {
                _stream.Read(a, 0, (int)c);
            }

            return a;
        }

        /// <summary>
        /// 写入字节数组
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void WriteBytes(byte[] bytes, int offset = 0, int len = 0)
        {
            int byteLen = (bytes == null ? 0 : bytes.Length);
            if (byteLen <= 0)
                return;

            if (len == 0)
                len = byteLen;

            _stream.Write(bytes, offset, len);
        }

        /// <summary>
        /// 根据字节序写入字节数组
        /// </summary>
        /// <param name="bytes"></param>
        public void WriteEndianBytes(byte[] bytes)
        {
            if (bytes == null)
                return;

            if (_endian == eEndian.BIG_ENDIAN)
            {
                for (int i = bytes.Length - 1; i >= 0; --i)
                    WriteByte(bytes[i]);
            }
            else
            {
                _stream.Write(bytes, 0, bytes.Length);
            }
        }

        public byte ReadByte()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadByte();
            return (byte)_stream.ReadByte();
        }

        public bool ReadBoolean()
        {
            return _stream.ReadByte() == 1;
        }

        public void WriteByte(byte value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();
            _stream.WriteByte(value);
        }

        public double ReadDouble()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadDouble();
            byte[] bytes = ReadBytes(8);
            double value = System.BitConverter.ToDouble(bytes, 0);
            return value;
        }

        public void WriteDouble(double value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();
            byte[] bytes = System.BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        public void WriteStringW(string str)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             for (int i = 0; i < str.ToCharArray().Length; i++)
            //             {
            //                 ushort v = str.ToArray()[i];
            //                 writer.Write(v);
            //             }
            //             writer.Write((ushort)0);
            //             writer.Flush();
            lock (_lock)
            {
                byte[] data = _unicode.GetBytes(str);
                for (int i = 0; i < data.Length; ++i)
                    WriteByte(data[i]);
            }

            WriteUint16(0);
        }

        public string ReadStringW()
        {
            if (_stream.Position + 2 <= _stream.Length)
            {
                //List<byte> buffer = new List<byte>();
                _tmpBuffer.Clear();

                while (true)
                {
                    if (_stream.Position + 2 > _stream.Length)
                    {
                        return string.Empty;
                    }
                    ushort b = ReadUint16();
                    if (b != 0)
                    {
                        byte[] bytes = WLBitConverter.GetBytes(b);
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            //buffer.Add(bytes[i]);
                            _tmpBuffer.Add(bytes[i]);
                        }
                        continue;
                    }

                    return Encoding.Unicode.GetString(_tmpBuffer.ToArray());
                }
            }

            return string.Empty;
        }

        public string ReadStringA()
        {
            if (_stream.Position + 1 <= _stream.Length)
            {
                //                 byte b = (byte)ReadByte();
                //                 List<byte> buffer = new List<byte>();
                byte b = (byte)_stream.ReadByte();
                _tmpBuffer.Clear();

                while (b != 0)
                {
                    _tmpBuffer.Add(b);
                    //b = (byte)ReadByte();
                    b = (byte)_stream.ReadByte();
                }

                return Encoding.ASCII.GetString(_tmpBuffer.ToArray());
            }

            return string.Empty;
        }

        public void WriteStringA(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            _stream.Write(bytes, 0, bytes.Length);
            _stream.WriteByte(0);
        }

        public void WriteUint32(uint value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();

            lock (WLBitConverter.bitConverterLock)
            {
                byte[] bytes = WLBitConverter.GetBytes(value);
                WriteEndianBytes(bytes);
            }
        }

        public uint ReadUint32()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadUInt32();
            byte[] bytes = ReadBytes(4);
            uint value = System.BitConverter.ToUInt32(bytes, 0);
            return value;
        }

        public void WriteUint16(ushort value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();

            lock (WLBitConverter.bitConverterLock)
            {
                byte[] bytes = WLBitConverter.GetBytes(value);
                WriteEndianBytes(bytes);
            }
        }

        public ushort ReadUint16()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadUInt16();
            byte[] bytes = ReadBytes(2);
            ushort value = System.BitConverter.ToUInt16(bytes, 0);
            return value;
        }

        public void WriteInt32(int value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();

            lock (WLBitConverter.bitConverterLock)
            {
                byte[] bytes = WLBitConverter.GetBytes(value);
                WriteEndianBytes(bytes);
            }
        }

        public int ReadInt32()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadInt32();
            byte[] bytes = ReadBytes(4);
            int value = System.BitConverter.ToInt32(bytes, 0);
            return value;
        }

        public void WriteInt64(long value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();

            lock (WLBitConverter.bitConverterLock)
            {
                byte[] bytes = WLBitConverter.GetBytes(value);
                WriteEndianBytes(bytes);
            }
        }

        public long ReadInt64()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadInt64();
            byte[] bytes = ReadBytes(8);
            long value = System.BitConverter.ToInt64(bytes, 0);
            return value;
        }

        public float ReadFloat()
        {
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             return reader.ReadSingle();
            byte[] bytes = ReadBytes(4);
            float value = System.BitConverter.ToSingle(bytes, 0);
            return value;
        }

        public void WriteFloat(float value)
        {
            //             BinaryWriter writer = new BinaryWriter(m_stream);
            //             writer.Write(value);
            //             writer.Flush();
            byte[] bytes = System.BitConverter.GetBytes(value);
            WriteEndianBytes(bytes);
        }

        public string ReadTCharString(int length)
        {
            uint count = (uint)length * 2;
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             byte[] bytes = reader.ReadBytes(count);
            byte[] bytes = ReadBytes(count);

            return Encoding.Unicode.GetString(bytes).Replace("\u0000", "");
        }

        public string ReadCharString(int length)
        {
            uint count = (uint)length;
            //             BinaryReader reader = new BinaryReader(m_stream);
            //             byte[] bytes = reader.ReadBytes(count);
            byte[] bytes = ReadBytes(count);
            return Encoding.ASCII.GetString(bytes);
        }

        public ushort ReadWord()
        {
            return ReadUint16();
        }
        public void WriteWord(ushort value)
        {
            WriteUint16(value);
        }

        public uint ReadDword()
        {
            return ReadUint32();
        }

        public void WriteDword(uint value)
        {
            WriteUint32(value);
        }

        private byte[] GetByteCache(int byteLen)
        {
            //if (byteLen > _maxByte)
            //    WLDebugTrace.Trace("[MsgHeader] GetByteCache warning, byteLen=" + byteLen + ",stack=" + Utilities.GetStackTrace());

            if (byteLen > _byteCache.Length)
            {
                //WLDebugTrace.Trace("[MsgHeader] GetByteCache, byte len=" + byteLen + ",stack=" + Utilities.GetStackTrace());
                int capacity = byteLen + 512;
                _byteCache = new byte[capacity];
            }

            return _byteCache;
        }

        public override string ToString()
        {
            string info = "";
            info += "Len=" + len;
            info += ",MessageID=" + messageID;
            info += ",Mask=" + byMask;
            info += ",Flag=" + byFlag;
            info += ",RouteCount=" + byRouteCount;
            info += ",HeaderOffset=" + byHeaderOffset;
            info += ",Position=" + _stream.Position;

            info += ",flag=";
            if ((byFlag & Flag.Compress) > 0) info += "compressed,";
            if ((byFlag & Flag.Encode) > 0) info += "encode,";
            if ((byFlag & Flag.Mask) > 0) info += "mask,";
            if ((byFlag & Flag.Filled) > 0) info += "filled";
            return info;
        }
    }
}

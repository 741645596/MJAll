

using System.Collections;
using System.Linq;
using System.Text;
using WLCore;

namespace WLHall
{
    public static class MsgParser
    {
	    /**
         * 解析消息，风格延续自Lua
         * format[0] 为类型字段，不区分大小写
         * format[1] 为key字段
         * format[2] 长度字段 可以为空 默认为1
         * format[3] 二维数组的一维长度 可以为空
         *
         *      示例：
         *      ArrayList msgFormat = new ArrayList
         *      {
         *          new ArrayList {"int", "code"},
         *          new ArrayList {"byte", "cards", 4}
         *      };
         *      var data = MsgParse.Parse(msg, msgFormat);
         */
	    public static MsgData Parse(MsgHeader msg, ArrayList format)
        {
            var obj = new Hashtable();

            for (var index = 0; index < format.Count; index++)
            {
                ArrayList data = (ArrayList)format[index];
                string dataType = (string)data[0];
                string key = (string)data[1];
                int length1 = data.Count <= 2 ? 1 : (int)data[2];
                int length2 = data.Count <= 3 ? 1 : (int)data[3];
                string typeKey = dataType.ToLower();

                for (var i = 0; i < length1; i++)
                {
                    if (length1 == 1 && length2 == 1)
                    {
                        var value = ReadMsg(msg, typeKey);
                        if (data.Count == 4)
                        {
                            obj.Add(key, new ArrayList { new ArrayList { value } });
                        }
                        else if (data.Count == 3)
                        {
                            obj.Add(key, new ArrayList { value });
                        }
                        else
                        {
                            obj.Add(key, value);
                        }
                    }
                    else
                    {
                        ArrayList list = obj.ContainsKey(key) ? obj[key] as ArrayList : new ArrayList();

                        if (!obj.ContainsKey(key))
                        {
                            obj.Add(key, list);
                        }

                        for (var j = 0; j < length2; j++)
                        {
                            var v = ReadMsg(msg, typeKey);

                            if (length2 == 1)
                            {
                                list.Add(v);
                            }
                            else
                            {
                                ArrayList subList = i <= (list.Count - 1) ? list[i] as ArrayList : new ArrayList();
                                if (i > (list.Count - 1))
                                {
                                    list.Add(subList);
                                }
                                subList.Add(v);
                            }
                        }
                    }
                }
            }

            return new MsgData(obj, format);
        }

        /**
         * 打包游戏房间内消息, 返回 GameMsgHeader，只提供给游戏使用 示例：
         *         var a = 10;
         *         var b = 20;
         *         var c = [11,12,13,14];
         *
         *          ArrayList msgFormat = new ArrayList
         *          {
         *             new ArrayList { "int", b },
         *             new ArrayList { "byte", a },
         *             new ArrayList { "byte", c, 4 }
         *          };
         *          var msgId = 50;
         *          var msg = MsgParse.Pack(msgId, msgFormat);
         */
        public static MsgHeader Pack(ushort msgId, ArrayList format = null)
        {
//             var msg = new GameMsgHeader
//             {
//                 m_messageID = msgId
//             };

            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = msgId;

            if (format == null)
            {
                return msg;
            }

            for (var index = 0; index < format.Count; index++)
            {
                ArrayList data = format[index] as ArrayList;
                string dataType = (string)data[0];
                var value = data[1];

                string typeKey = dataType.ToLower();

                if (data.Count == 3)
                {
                    int length = (int)data[2];
                    var list = value as ArrayList;
                    for (var i = 0; i < length; i++)
                    {
                        var v = list[i];
                        WriteMsg(msg, typeKey, v);
                    }
                }
                else
                {
                    WriteMsg(msg, typeKey, value);
                }
            }

            return msg;
        }

        private static void WriteMsg(MsgHeader msg, string typeKey, object value)
        {
            if (typeKey == "int")
            {
                msg.WriteInt32((int)value);
            }
            else if (typeKey == "long")
            {
                msg.WriteInt32((int)value);
            }
            else if (typeKey == "actiontype")
            {
                msg.WriteInt32((int)value);
            }
            else if (typeKey == "url")
            {
                msg.WriteStringA((string)value);
            }
            else if (typeKey == "eventtype")
            {
                msg.WriteUint32((uint)value);
            }
            else if (typeKey == "dword")
            {
                msg.WriteUint32((uint)value);
            }
            else if (typeKey == "unsignedint")
            {
                msg.WriteUint32((uint)value);
            }
            else if (typeKey == "byte")
            {
                msg.WriteByte((byte)value);
            }
            else if (typeKey == "card")
            {
                msg.WriteByte((byte)value);
            }
            else if (typeKey == "char")
            {
                msg.WriteByte((byte)value);
            }
            else if (typeKey == "bool")
            {
                msg.WriteByte((byte)value);
            }
            else if (typeKey == "word")
            {
                msg.WriteUint16((ushort)value);
            }
            else if (typeKey == "shortint")
            {
                msg.WriteUint16((ushort)value);
            }
            else if (typeKey == "tchar")
            {
                msg.WriteUint16((ushort)value);
            }
            else if (typeKey == "longlong")
            {
                msg.WriteInt64((long)value);
            }
            else if (typeKey == "float")
            {
                msg.WriteFloat((float)value);
            }
            else if (typeKey == "double")
            {
                msg.WriteDouble((double)value);
            }
            else
            {
                WLDebug.LogWarning("不支持的消息类型" + typeKey);
            }
        }

        private static object ReadMsg(MsgHeader msg, string typeKey)
        {
            if (typeKey == "int")
            {
                return msg.ReadInt32();
            }
            else if (typeKey == "url")
            {
                return msg.ReadStringA();
            }
            else if (typeKey == "word")
            {
                return msg.ReadUint16();
            }

            else if (typeKey == "tchar")
            {
                return msg.ReadUint16();
            }

            else if (typeKey == "wchar")
            {
                return msg.ReadUint16();
            }
            else if (typeKey == "card")
            {
                return msg.ReadByte();
            }

            else if (typeKey == "byte")
            {
                return msg.ReadByte();
            }

            else if (typeKey == "char")
            {
                return (byte)msg.ReadByte();
            }

            else if (typeKey == "bool")
            {
                return msg.ReadByte() == 1;
            }
            else if (typeKey == "longlong")
            {
                return msg.ReadInt64();
            }

            else if (typeKey == "actiontype")
            {
                return msg.ReadInt64();
            }
            //else if(typeKey == "shortint")
            //{
                //    return msg.ReadInt16();
            //}
            else if (typeKey == "dword")
            {
                return msg.ReadUint32();
            }

            else if (typeKey == "unsignedint")
            {
                return msg.ReadUint32();
            }

            else if (typeKey == "eventtype")
            {
                return msg.ReadUint32();
            }

            else if (typeKey == "long")
            {
                return msg.ReadUint32();
            }
            else if (typeKey == "float")
            {
                return msg.ReadFloat();
            }
            else if (typeKey == "double")
            {
                return msg.ReadDouble();
            }
            else if (typeKey == "fuzi")
            {
                return FuziData.From(msg);
            }
            else if (typeKey == "systemtime")
            {
                return MsgSystemTime.From(msg);
            }
            else
            {
                WLDebug.LogError("不支持读取的消息类型" + typeKey);
                return null;
            }
        }

        /**
         * 根据字节数组返回真实的字符串（去除占位的空字符）
         *
         * @param {any} buffer
         * @returns
         */
        public static string GetStringFromBuffer(byte[] buffer)
        {
            var realLength = buffer.Length;
            for (var index = 0; index < buffer.Length; index++)
            {
                if (buffer[index] == 0)
                {
                    realLength = index;
                    break;
                }
            }

            return Encoding.Default.GetString(buffer.Take(realLength).ToArray());
        }

        public static ArrayList NewInt(int value)
        {
            return new ArrayList { "int", value };
        }

        public static ArrayList NewLong(int value)
        {
            return NewInt(value);
        }

        public static ArrayList NewActionype(int value)
        {
            return NewInt(value);
        }

        public static ArrayList NewUrl(string value)
        {
            return new ArrayList { "url", value };
        }

        public static ArrayList NewDword(uint value)
        {
            return new ArrayList { "dword", value };
        }

        public static ArrayList NewEventtype(uint value)
        {
            return NewDword(value);
        }

        public static ArrayList NewUnsignedint(uint value)
        {
            return NewDword(value);
        }

        public static ArrayList NewByte(byte value)
        {
            return new ArrayList { "byte", value };
        }

        public static ArrayList NewCard(byte value)
        {
            return NewByte(value);
        }

        public static ArrayList NewChar(byte value)
        {
            return NewByte(value);
        }

        public static ArrayList NewBool(bool value)
        {
            byte b = 0;
            if (value)
            {
                b = 1;
            }
            return NewByte(b);
        }

        public static ArrayList NewWord(ushort value)
        {
            return new ArrayList { "word", value };
        }

        public static ArrayList NewShortint(ushort value)
        {
            return NewWord(value);
        }

        public static ArrayList NewTchar(ushort value)
        {
            return NewWord(value);
        }

        public static ArrayList NewLonglong(long value)
        {
            return new ArrayList { "longlong", value };
        }

        public static ArrayList NewFloat(float value)
        {
            return new ArrayList { "float", value };
        }

        public static ArrayList NewDouble(double value)
        {
            return new ArrayList { "double", value };
        }
    }
}

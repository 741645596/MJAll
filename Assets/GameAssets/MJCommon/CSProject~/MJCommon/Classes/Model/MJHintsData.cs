// @Author: tanjinhua
// @Date: 2021/4/20  19:09


using System.Collections.Generic;
using WLCore;

namespace MJCommon
{

    /// <summary>
    /// 胡牌(听牌)提示数据(三种类型合一)
    /// </summary>
    public class MJHintsData
    {
        /// <summary>
        /// 提示类型
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// 普通类型，只显示剩余几张
            /// </summary>
            Normal,
            /// <summary>
            /// 显示番(倍)数与剩余张数
            /// </summary>
            Multiple,
            /// <summary>
            /// 显示胡牌类型与剩余张数
            /// </summary>
            Descriptive
        }


        /// <summary>
        /// 类型
        /// </summary>
        public Type type;

        /// <summary>
        /// 牌值列表
        /// </summary>
        public List<int> cardValues;

        /// <summary>
        /// 番(倍)数列表
        /// </summary>
        public List<int> multiplies;

        /// <summary>
        /// 胡牌类型描述列表
        /// </summary>
        public List<string> descriptions;


        public static MJHintsData From(MsgHeader msg, Type type, int readCount)
        {
            MJHintsData data = new MJHintsData
            {
                type = type
            };

            data.cardValues = new List<int>();

            for (int i = 0; i < readCount; i++)
            {
                data.cardValues.Add(msg.ReadByte());
            }

            switch (type)
            {
                case Type.Multiple:
                    data.multiplies = new List<int>();
                    for (int i = 0; i < readCount; i++)
                    {
                        data.multiplies.Add(msg.ReadInt32());
                    }
                    break;

                case Type.Descriptive:
                    data.descriptions = new List<string>();
                    for (int i = 0; i < readCount; i++)
                    {
                        data.descriptions.Add(msg.ReadStringW());
                    }
                    break;

                default:
                    break;
            }

            return data;
        }
    }
}

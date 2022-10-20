// @ UserInfo.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/31 星期三 17:47:11
// @ Description :

using System.Text;
using WLCore;

namespace WLHall
{
    public class UserInfo
    {
        public long money { get; internal set; }    //!<用户金钱
        public long bankMoney;                      //!<用户存款
        public uint id;                             //!<用户ID
        public uint xzMoney;                        //!<鑫泽币（吉祥币）
        public uint lottery;                        //!<奖励券（元宝）
        public uint dwPrestige;                     //!<声望值
        public byte userType;                       //!<用户类型,为@ref USER_TYPE 中的一个值
        public byte userFrom;                       //!<用户来源，为@ref USER_FROM 中的一个或多个值
        public ushort clientType;                   //!<登陆类型，为@ref CLIENT_TYPE 中的一个值
        public uint avatarId;                       //!<头像ID,为UINT_MAX(值为0XFFFFffff)则为自定义头像
        public uint prestige;                       //!<服装ID，为保证新旧版本的数据兼容，这个字段用于保存玩家的荣誉值。
        public uint vipValue;                       //!<VIP累积值
        public int chatLimit;                       //!<聊天限制,为@ref CHAT_LIMIT 的一个或多个值的组合
        public uint tireTime;                       //!<疲劳时间，为UINT_MAX则不计沉迷时间
        public int sex;                             //!<性别，1：男；0：女
        public string szNickName;                   //!<昵称
        public uint dwReserved;                     //!<该值保留，必须为0
        public int[] nExt;                          //!<24个扩展属性（服务器实际只存了四个，后面的全部作废），0 贵族月卡 1 星耀月卡 2 朋友场局数
        public string szAvatarImgPath;              //!<头像地址
        public string nickName;
        public string avatarPath;                   

        public uint lastLoginIp;                    // 对应js版本的 HallManager 下的this._dwLastLoginIP
        public bool hasSecPSW;                      // 对应js版本的 HallManager 下的this._bHasSecPSW

        public MsgSystemTime tmVIPEnd;
        public MsgSystemTime tmLastLogout;
        public uint dwCityID; 
        public string strRealName;
        public string phone;
        public string IDCard;
        public uint clothID;

  
        public static UserInfo From(MsgHeader msg)
        {
            UserInfo info = new UserInfo();
            info.money = msg.ReadInt64();
            info.bankMoney = msg.ReadInt64();
            info.id = msg.ReadUint32();
            info.xzMoney = msg.ReadUint32();
            info.lottery = msg.ReadUint32();
            info.dwPrestige = msg.ReadUint32();
            info.userType = (byte)msg.ReadByte();
            info.userFrom = (byte)msg.ReadByte();
            info.clientType = msg.ReadUint16();
            info.avatarId = msg.ReadUint32();
            info.prestige = msg.ReadUint32();
            info.vipValue = msg.ReadUint32();
            info.chatLimit = msg.ReadInt32();
            info.tireTime = msg.ReadUint32();
            info.sex = msg.ReadInt32();
            info.szNickName = msg.ReadTCharString(Constants.USER_NAME_LEN);

            info.dwReserved = msg.ReadUint32();
            info.nExt = new int[Constants.USER_EXT_COUNT];
            for (int i = 0; i < Constants.USER_EXT_COUNT; i++)
            {
               info.nExt[i] = msg.ReadInt32();
            }
            info.szAvatarImgPath = msg.ReadCharString(Constants.AVATAR_IMG_PATH_LEN);

            // TODO
            info.nickName = MsgParser.GetStringFromBuffer(Encoding.Default.GetBytes(info.szNickName));
            info.avatarPath = MsgParser.GetStringFromBuffer(Encoding.Default.GetBytes(info.szAvatarImgPath));
            
            return info;
        }

        public static UserInfo FromEx(MsgHeader msg) {
            UserInfo info = From(msg);
            info.lastLoginIp = msg.ReadUint32();
            info.hasSecPSW = msg.ReadUint32() != 0;

            info.tmVIPEnd = MsgSystemTime.From(msg);
            info.tmLastLogout = MsgSystemTime.From(msg);
            info.dwCityID = msg.ReadDword();
            info.strRealName = msg.ReadStringW();
            info.phone = msg.ReadStringA();
            info.IDCard = msg.ReadCharString(Constants.ID_CARD_LEN);
            return info;
        }
    }
}

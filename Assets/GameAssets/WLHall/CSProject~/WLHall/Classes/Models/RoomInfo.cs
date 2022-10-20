// @ RoomInfo.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/31 星期三 11:09:51
// @ Description :

using System.Collections.Generic;
using Unity.Utility;
using WLCore;

namespace WLHall
{
    public class RoomInfo
    {
        public uint id;                             //!<房间ID
        public uint type;                           //!<房间类型
        public uint serverId;                       //!<服务器ID；
        public uint gameId;                         //!<游戏ID
        public uint baseMoney;                      //!<房间底注,如果房间属性为@ref ROOM_TYPE_BASEMONEY 有效
        public float tax;                           //!<税率.房价属性为 @ref ROOM_TYPE_RATETAX 则该参数为扣税系数，否则为扣税值
        public float taxVip;                        //!<VIP税率.房价属性为@ref ROOM_TYPE_RATETAX 则该参数为扣税系数，否则为扣税值
        public int minMoney;                        //!<最小金钱或者积分限制
        public int maxMoney;                        //!<最大金钱或者积分限制
        public uint joinRight;                      //!<加入权限
        public ushort criticalPlayers;              //!<临界玩家数
        public ushort maxPlayers;                   //!<最大玩家数
        public ushort deskCount;                    //!<桌子数量
        public ushort playersPerDesk;               //!<每桌玩家数
        public ushort leastPlayers;                 //!<至少多少个玩家才能开始
        public ushort clientType;                   //!<支持的客户端类型，为@ref CLIENT_TYPE 中的一个值或多个值
        public string szRoomName;                   //!<房间名
        public string szRoomCommand;                //!<房间命令行
        public ushort wPlayers;                     //!<房间人数
        public ushort wShowPlayers;
        public Dictionary<string, string> cmd;      //!<房间命令行
        public string name;                         //!<房间名

        public static RoomInfo From(MsgHeader msg)
        {
            RoomInfo info = new RoomInfo()
            {
                id = msg.ReadUint32(),
                type = msg.ReadUint32(),
                serverId = msg.ReadUint32(),
                gameId = msg.ReadUint32(),
                baseMoney = msg.ReadUint32(),
                tax = msg.ReadFloat(),
                taxVip = msg.ReadFloat(),
                minMoney = msg.ReadInt32(),
                maxMoney = msg.ReadInt32(),
                joinRight = msg.ReadUint32(),
                criticalPlayers = msg.ReadUint16(),
                maxPlayers = msg.ReadUint16(),
                deskCount = msg.ReadUint16(),
                playersPerDesk = msg.ReadUint16(),
                leastPlayers = msg.ReadUint16(),
                clientType = msg.ReadUint16(),
                szRoomName = msg.ReadTCharString(Constants.ROOM_NAME_LEN),
                szRoomCommand = msg.ReadTCharString(Constants.COMMAND_LEN),
                wPlayers = msg.ReadUint16()
            };
            msg.position += 2;
            info.cmd = EncodeHelper.StringGetPair(info.szRoomCommand);
            info.name = info.szRoomName;
            return info;
        }
    }
}

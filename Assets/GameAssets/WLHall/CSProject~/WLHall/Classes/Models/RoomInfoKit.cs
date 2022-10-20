// @ RoomInfoKit.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/31 星期三 11:08:13
// @ Description :

using System.Collections.Generic;
using WLCore;

namespace WLHall
{
    
    public static class RoomInfoKit
    {
        private static Dictionary<uint, RoomInfo> _dicRoomInfos = new Dictionary<uint, RoomInfo>();
        private static Dictionary<uint, RelationInfo> _dicRelationInfos = new Dictionary<uint, RelationInfo>();

        /// <summary>
        /// 反序列化游戏房间
        /// </summary>
        /// <param name="msg"></param>
        public static void DeserializeRoomInfo(MsgHeader msg)
        {
            _dicRoomInfos.Clear();
            uint count = msg.ReadUint32();
            for (int i = 0; i < count; i++)
            {
                RoomInfo roomInfo = RoomInfo.From(msg);
                if (WLDebugTrace.traceEnable)
                    WLDebugTrace.Trace("[RoomInfoKit] DeserializeRoomInfo, room id=" + roomInfo.id);
                if (!_dicRoomInfos.ContainsKey(roomInfo.id))
                {
                    _dicRoomInfos.Add(roomInfo.id, roomInfo);
                }
            }
        }

        /// <summary>
        /// 反序列化游戏节点
        /// </summary>
        /// <param name="msg"></param>
        public static void DeserializeRelationInfo(MsgHeader msg)
        {
            uint count = msg.ReadUint32();
            for (int i = 0; i < count; i++)
            {
                RelationInfo info = RelationInfo.From(msg);
                //WLDebug.Info(info.ToString());
                if (!_dicRelationInfos.ContainsKey(info.dwRelationID))
                {
                    _dicRelationInfos.Add(info.dwRelationID, info);
                }
            }
        }

        public static RoomInfo GetRoomInfoByID(uint roomID)
        {
            if (_dicRoomInfos.ContainsKey(roomID)) {
                return _dicRoomInfos[roomID];
            }
            return null;
        }
    }
}

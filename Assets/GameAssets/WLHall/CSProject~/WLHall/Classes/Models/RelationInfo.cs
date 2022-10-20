// @ RelationInfo.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/31 星期三 14:00:49
// @ Description :

using WLCore;

namespace WLHall
{
    public class RelationInfo
    {
        public uint dwRelationID;                   //!<关系节点ID
        public uint dwIcon;                         //!<图标索引
        public uint dwParentType;                   //!<父节点类型
        public uint dwParentID;                     //!<父节点ID
        public uint dwObjectType;                   //!<对象类型
        public uint dwObjectID;                     //!<对象ID
        public uint dwSortIndex;                    //!<排序序号
        public uint dwClientType;                   //!<支持的客户端类型
        public string szName;                       //!<节点名,如果为空，则使用默认对象名
        public string szCommand;                    //!<命令行

        public static RelationInfo From(MsgHeader msg)
        {
            return new RelationInfo()
            {
                dwRelationID = msg.ReadUint32(),
                dwIcon = msg.ReadUint32(),
                dwParentType = msg.ReadUint32(),
                dwParentID = msg.ReadUint32(),
                dwObjectType = msg.ReadUint32(),
                dwObjectID = msg.ReadUint32(),
                dwSortIndex = msg.ReadUint32(),
                dwClientType = msg.ReadUint32(),
                szName = msg.ReadTCharString(Constants.USER_NAME_LEN),
                szCommand = msg.ReadTCharString(Constants.COMMAND_LEN)
            };
        }
    }

}

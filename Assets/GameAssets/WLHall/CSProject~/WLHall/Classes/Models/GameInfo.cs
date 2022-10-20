// @ GameInfo.cs
// @ Email : 415546748@qq.com
// @ Author： chenfaxin
// @ Date : 2021/3/30 星期二 17:06:40
// @ Description : individual gameInfo

using System.Collections.Generic;
using Unity.Utility;
using WLCore;

namespace WLHall
{
    public class GameInfo
    {
        public long version;                    //!<当前版本
        public long supportVersion;             //!<支持的版本
        public uint id;                         //!<游戏ID
        public uint type;                       //!<游戏类型,为@ref GAME_GROUP 中的一个值或多个值
        public ushort clientSupport;            //!<游戏客户端支持,为@ref CLIENT_TYPE 中的一个值或多个值
        public byte disable;                    //!<是否禁用游戏（是否客户端显示)
        public byte reserved;                   //!<保留
        public string szGameName;               //!<游戏名
        public string szShortName;              //!<游戏缩写名，最多4字符
        public string szShellFile;              //!<游戏命令行
        public string szCommand;                //!<执行文件名
        public Dictionary<string, string> cmd;  //!<游戏命令行字典
        public string name;                     //!<游戏名
        public string shortName;                //!<游戏短名
        public string shell;                    //!<游戏命令行

        public static GameInfo From(MsgHeader msg)
        {
            GameInfo info = new GameInfo();
            info.version = msg.ReadInt64();
            info.supportVersion = msg.ReadInt64();
            info.id = msg.ReadUint32();
            info.type = msg.ReadUint32();
            info.clientSupport = msg.ReadUint16();
            info.disable = (byte)msg.ReadByte();
            info.reserved = (byte)msg.ReadByte();
            info.szGameName = msg.ReadTCharString(Constants.GAME_NAME_LEN);
            info.szShortName = msg.ReadTCharString(Constants.GAME_SHORT_NAME_LEN);
            info.szShellFile = msg.ReadTCharString(Constants.GAME_SHELL_FILE_LEN);
            info.szCommand = msg.ReadTCharString(Constants.COMMAND_LEN);
            msg.position += 2;
            info.name = info.szGameName;
            info.shortName = info.szShortName;
            info.shell = info.szShellFile;
            info.cmd = EncodeHelper.StringGetPair(info.szCommand);
            return info;
        }

        public override string ToString()
        {
            return $"[id: {id}, name: {name}, shortName: {shortName}, cmd: {LitJson.JsonMapper.ToJson(cmd)}]";
        }
    }
}

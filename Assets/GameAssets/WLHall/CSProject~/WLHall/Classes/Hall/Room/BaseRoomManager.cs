
using System;
using WLCore;
using WLCore.Stage;

namespace WLHall
{
    /// <summary>
    /// 房间管理器基类
    /// </summary>
    public abstract class BaseRoomManager : MessageManager
    {
        /// <summary>
        /// 房间信息
        /// </summary>
        public RoomInfo roomInfo => _roomInfo;

        /// <summary>
        /// 游戏信息
        /// </summary>
        public GameInfo gameInfo => _gameInfo;

        /// <summary>
        /// 房间ID
        /// </summary>
        public uint roomId => _roomInfo.id;

        /// <summary>
        /// 用户自己的玩家对象
        /// </summary>
        public BasePlayer playerSelf => _playerSelf;

        /// <summary>
        /// 收到加入房间答应事件
        /// </summary>
        public Action onInfoReply;

        protected HallManager _hallManager;
        protected BaseGameStage _gameStage;
        protected RoomInfo _roomInfo;
        protected GameInfo _gameInfo;
        protected BasePlayer _playerSelf;
        protected ushort _deskID;


        public BaseRoomManager()
        {
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_USE_PROP_REPLY, OnMsgUsePropReply);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_USE_PROP_FAILED, OnMsgUsePropFailed);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_SYSTEM_MSG_NOTIFY, OnRoomSystemMsgNotify);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_SAVE_MONEY_REPLY, OnRoomExchangeMoneyReply);
        }

        /// <summary>
        /// 启动游戏客户端
        /// </summary>
        /// <returns></returns>
        public bool StartGame(Action onload = null)
        {
            if (_hallManager == null)
            {
                return false;
            }

            if (_gameStage != null)
            {
                _gameStage.Exit();

                _gameStage = null;
            }

            string upper = gameInfo.shortName.ToUpper();

            string typeFullName = $"NS_{upper}.Client{upper}";

            _gameStage = StageManager.RunStage(typeFullName, this) as BaseGameStage;

            _gameStage.sceneLoaded += () =>
            {
                onload?.Invoke();

                StartProcessWaitingQueue();
            };

            return true;
        }

        /// <summary>
        /// 设置房间信息
        /// </summary>
        /// <param name="dwRoomID"></param>
        public void SetRoomInfo(uint dwRoomID)
        {
            _roomInfo = RoomInfoKit.GetRoomInfoByID(dwRoomID);

            _gameInfo = GameInfoKit.GetGameByGameID(_roomInfo.gameId);
        }

        /// <summary>
        /// 设置大厅管理器
        /// </summary>
        /// <param name="hallManager"></param>
        public void SetHallManger(HallManager hallManager)
        {
            _hallManager = hallManager;
        }

        /// <summary>
        /// 换桌
        /// </summary>
        public void ChangeDesk()
        {
            MsgHeader msg1 = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg1.messageID = RoomMsgDefine.MSG_ROOM_STAND_UP;
            SendData(msg1);

            MsgHeader msg2 = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg2.messageID = RoomMsgDefine.MSG_ROOM_SIT_DOWN;
            SendData(msg2);
        }

        /// <summary>
        /// 起立
        /// </summary>
        public void StandUp()
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = RoomMsgDefine.MSG_ROOM_STAND_UP;
            SendData(msg);

            // 解开引用
            _hallManager?.ExitRoom();
        }

        #region Messages
        private void OnMsgUsePropReply(MsgHeader msg)
        {
            // todo xzh
        }

        private void OnMsgUsePropFailed(MsgHeader msg)
        {
            // todo xzh
        }

        private void OnRoomSystemMsgNotify(MsgHeader msg)
        {
            // todo xzh
        }

        private void OnRoomExchangeMoneyReply(MsgHeader msg)
        {
            // todo xzh
        }
        #endregion


        #region Inherit
        public override void OnProcessMessage(MsgHeader msg)
        {
            int offset = Constants.GetMsgOffset(msg.messageID);
            WLDebugTrace.Trace("[BaseRoomManager] OnProcessMessage, msgid=" + msg.messageID + ",submsgid=" + offset);
            if (offset == Constants.MSG_OFFSET_BASE_LOGIC)
            {
                if (_gameStage != null)
                {
                    _gameStage.OnProcessMessage(msg);
                }
                else
                {
                    WLDebug.LogWarning($"BaseRoomManager.OnProcessMessage: 收到游戏消息[{msg.messageID}]，但是游戏Client为空");
                }

                return;
            }

            base.OnProcessMessage(msg);
        }

        public override void SendData(MsgHeader msg, byte flag = MsgHeader.Flag.None)
        {
            // socket异常关闭触发重连过程中会清除_hallManager的引用
            if (_hallManager == null)
            {
                return;
            }

            if ((flag & MsgHeader.Flag.Filled) == 0 && UserInfoKit.UserInfo != null)
            {
                WriteGameMsgHeader(msg, ref flag);
            }

            _hallManager.SendData(msg, flag);
        }

        public override void Dispose()
        {
            base.Dispose();

            _hallManager = null;
            _gameStage = null;
            _roomInfo = null;
            _gameInfo = null;
            _playerSelf = null;

        }
        #endregion

        private void WriteGameMsgHeader(MsgHeader msg, ref byte flag)
        {
            long tmp = msg.position;

            msg.position = MsgHeader.headerSize;
            msg.WriteUint32(_roomInfo.serverId);
            msg.WriteUint32(0);
            msg.WriteUint32(UserInfoKit.GetSelfID());
            msg.WriteUint32(_roomInfo.id);

            msg.position = tmp;

            if ((flag & MsgHeader.Flag.Offset) != 0)
            {
                msg.byHeaderOffset += 16;
            }
            else
            {
                msg.byHeaderOffset = 24;
                flag |= MsgHeader.Flag.Offset;
            }
        }
    }
}

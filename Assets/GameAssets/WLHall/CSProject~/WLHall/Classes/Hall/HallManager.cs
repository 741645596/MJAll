// @Author: tanjinhua
// @Date: 2021/8/13  16:35

using System;
using System.Collections.Generic;
using Unity.Utility;
using WLCore;

namespace WLHall
{
    /// <summary>
    /// 大厅服务器消息管理器
    /// </summary>
    public class HallManager : MessageManager
    {
        private string _session;
        private uint _userId;
        private string _hallServerUrl;
        private ushort _hallServerId;
        private int _hallServerPort;
        private BaseRoomManager _roomManager;

        internal HallManager()
        {
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_USER_JOIN_REPLY, OnMsgUserJoinReply);
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_JOIN_GAME_REPLY, OnMsgJoinGameReply);
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_CLIENT_CHANGE_USERINFO_REPLY, OnMsgChangeUserInfoReply);
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_UPDATE_USER_DATA, OnMsgUpdateUserData);                             // 更新玩家数据
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_EXCHANGE_MONEY_REPLY, OnMsgExchangeMoneyReply);                     // 大厅存取豆兑换应答
            RegisterMessageProcessor(HallMsgDefine.MSG_CLIENT_HALL_CHANGE_PASSWORD_REPLY, OnQueryChangePasswordReply);          // 修改密码的回复
            RegisterMessageProcessor(HallMsgDefine.MSG_CLIENT_HALL_GET_CAPTCHA_REPLY, OnGetCaptchaReply);                       // 获取验证码的回复
            RegisterMessageProcessor(HallMsgDefine.MSG_CLIENT_HALL_BIND_ACCOUNT_REPLY, OnClientHallBindAccountReply);           // 账号激活/辅助账号 绑定 与 解绑 应答
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_SYSTEM_MESSAGE, OnMsgSystemMessage);                                // 系统消息通知
            RegisterMessageProcessor(HallMsgDefine.MSG_HALL_QUERY_ROOMKEY_REPLY, OnMsgQueryRoomKeyReply);                       // 查询房间ID应答 (8257)
        }


        /// <summary>
        /// 连接大厅服务器
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userId"></param>
        /// <param name="serverUrl"></param>
        public void Connect(string session, uint userId, string serverUrl, int port)
        {
            WLDebugTrace.Trace("[HallManager] StartConnect, url=" + serverUrl);
            if (string.IsNullOrEmpty(serverUrl))
            {
                WLDebug.LogWarning("HallManager.StartConnect: 连接失败，服务器地址为空");
                return;
            }

            _session = session;
            _userId = userId;
            _hallServerUrl = serverUrl;
            _hallServerPort = port;

            _cs?.Dispose();
            _cs = new ClientSocket(this);
            _cs.Connect(serverUrl, port);
        }


        /// <summary>
        /// 重连
        /// </summary>
        public void Reconnect()
        {
            var userId = UserInfoKit.GetSelfID();
            if (userId != 0 && !string.IsNullOrEmpty(_session) && !string.IsNullOrEmpty(_hallServerUrl))
            {
                GameAppStage.Instance.ConnectHallServer(_session, userId, _hallServerUrl, _hallServerPort);
            }
        }


        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="roomKey"></param>
        public void JoinRoom(uint roomID, string roomKey = "", Action onInfoReply = null)
        {
            RoomInfo roomInfo = RoomInfoKit.GetRoomInfoByID(roomID);
            if (roomInfo == null)
            {
                // todo 查询卡房间
                WLDebug.LogWarning("HallManager.JoinRoom: 房间不存在！");
                return;
            }

            GameInfo gameInfo = GameInfoKit.GetGameByGameID(roomInfo.gameId);
            if (gameInfo == null)
            {
                WLDebug.LogWarning("HallManager.JoinRoom: 游戏不存在");
                return;
            }

            if (FunctionEx.GetRoomMode(roomInfo.type) == Constants.ROOM_TYPE_ALLOCSIT)
            {
                var userBean = UserInfoKit.GetMoney();
                if (userBean < roomInfo.minMoney || userBean > roomInfo.maxMoney)
                {
                    WLDebug.LogWarning("加入房间失败，携带豆豆低于房间下限后高于房间上限");
                    return;
                }
            }

            FunctionEx.LoadGameAssemblies(gameInfo, successed =>
            {
                if (!successed)
                {
                    WLDebug.LogWarning("HallManager.JoinRoom: 加载DLL失败");
                    return;
                }
                // todo xzh 先直接加入
                // 创建房间管理器
                _roomManager = CreateRoomManager(roomInfo);
                _roomManager.onInfoReply = () => onInfoReply?.Invoke();
                _roomManager.SetHallManger(this);
                _roomManager.SetRoomInfo(roomID);

                MsgHeader msg = NetMsgFactory.GetNetMsgOut();
                msg.messageID = HallMsgDefine.MSG_HALL_JOIN_GAME;
                msg.WriteUint32(roomID);
                SendData(msg);
            });
        }


        /// <summary>
        /// 退出房间
        /// </summary>
        public void ExitRoom()
        {
            if (_roomManager == null)
            {
                return;
            }

            MsgHeader msg = NetMsgFactory.GetNetMsgOut();
            msg.messageID = HallMsgDefine.MSG_HALL_LEAVE_GAME;

            msg.WriteUint32(_roomManager.roomId);
            SendData(msg, MsgHeader.Flag.None);
            _roomManager.Dispose();
            _roomManager = null;
        }


        /// <summary>
        /// 创建朋友场
        /// </summary>
        /// <param name="shortName"></param>
        /// <param name="roomId"></param>
        /// <param name="rules"></param>
        public void CreateFriendRoom(string shortName, uint roomId, List<byte> rules)
        {
            uint gameId = GameInfoKit.GetGameIDByShortName(shortName);
            if (gameId == uint.MaxValue)
            {
                WLDebug.LogWarning("创建房间失败!");
                return;
            }

            JoinRoom(roomId, "", () => (_roomManager as CustomRoomManager).CreateGroup(rules));
        }


        /// <summary>
        /// 加入朋友场
        /// </summary>
        /// <param name="roomKey"></param>
        public void JoinFriendRoom(string roomKey)
        {
            MsgHeader msg = NetMsgFactory.GetNetMsgOut();
            msg.messageID = HallMsgDefine.MSG_HALL_QUERY_ROOMKEY;
            msg.WriteUint32(uint.Parse(roomKey));

            SendData(msg, Constants.CRYPT_MSG);
        }


        /// <summary>
        /// 关闭消息管理器
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            _roomManager?.Dispose();

            _roomManager = null;
        }


        /// <summary>
        /// 每帧执行，由GameAppStage调用
        /// </summary>
        public override void Update()
        {
            base.Update();

            _roomManager?.Update();
        }


        /// <summary>
        /// 消息处理及分发
        /// </summary>
        /// <param name="msg"></param>
        public override void OnProcessMessage(MsgHeader msg)
        {
            if ((msg.byFlag & Constants.MSG_HEADER_FLAG_PACKET) != 0)
                WLDebugTrace.Trace("[HallManager] OnProcessMessage, byflag set");

                //-----------------------------------------亲友圈修改逻辑-----------------------------
                //             ushort wMsgOff = (ushort)(msg.messageID >> 12); // 获取高4位
                // 
                //             if (msg.messageID == Constants.MSG_KEEPALIVE || wMsgOff == Constants.MSG_OFFSET_LOBBY_TO_HALLSERVER)
                //             {
                //                 if ((msg.byFlag & Constants.MSG_HEADER_FLAG_PACKET) != 0)
                //                 {
                //                     msg = this._cs.getAddPacktMsg(new Uint8Array(msg.getBuffer()));
                //                 }
                //                 base.OnProcessMessage(msg);
                //                 return;
                //             }

                /*====================== 段位赛消息开始 ======================*/
                //             if (msg.messageID == HallMsgDefine.MSG_RANK_PLAYER_GET_RANK_INFO_REPLY
                //                 || msg.messageID == HallMsgDefine.MSG_RANK_PLAYER_GET_PLAYER_LEVEL_REPLY
                //                 || msg.messageID == HallMsgDefine.MSG_RANK_PLAYER_REWARD_REPLY
                //                 || msg.messageID == HallMsgDefine.MSG_RANK_PLAYER_GET_PLAYER_INFO_REPLY
                //                 || msg.messageID == HallMsgDefine.MSG_RANK_PLAYER_GET_PLAYER_STAR_CHANGE
                //                 || msg.messageID == HallMsgDefine.MSG_RANK_SEND_PLAYER_SCORE_CHANGE
                //                 )
                //             {
                //                 base.OnProcessMessage(msg);
                //                 return;
                //             }
                // 
                //             if ((msg.messageID >= HallMsgDefine.MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY && msg.messageID <= HallMsgDefine.MSG_FRIEND_GROUP_GET_GROUP_TAGS_INFO_REPLY)
                //             || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_FLUSH_PROP_UPDATE || msg.messageID == HallMsgDefine.MSG_FRIEND_CHECK_CLUB_REPLY
                //             || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_PLAYER_RECHARGE_REPLY
                //             || (msg.messageID >= HallMsgDefine.MGS_FRIEND_GROUP_PLAYER_DISBAND_REPLY && msg.messageID <= HallMsgDefine.MSG_FRIEND_GAME_GROUP_CHANGE_ROOM_NAME
                //                 || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_VERSION_LIST || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_CHANGE_ROOM_NAME_REPLY)
                //             || msg.messageID == HallMsgDefine.MGS_FRIEND_GET_CLUB_INFO_REPLY_EX
                //             || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_GAME_TAGS_REPLY
                //             || msg.messageID == HallMsgDefine.MSG_FRIEND_GROUP_UPDATE_GAME_RULE_TAGS)
                //             {
                //                 base.OnProcessMessage(msg);
                //                 return;
                //             }

            ushort msgId = msg.messageID;
            WLDebugTrace.Trace("[HallManager] OnProcessMessage, msgid=" + msgId);
            // 如果是房间消息
            if (msgId <= 4096 * 2)
            {
                _roomManager?.OnSocketMessage(msg);
                return;
            }

            base.OnProcessMessage(msg);
        }


        #region Socket events
        public override void OnSocketOpen()
        {
            WLDebug.Info($"连接大厅服务器成功，URL = {_hallServerUrl}");

            WLDebugTrace.TraceInfo("[HallManager] OnSocketOpen");

            SendHallUserJoin();
        }

        public override void OnSocketClose(int errorCode)
        {
            // 非正常关闭
            if (errorCode != 0)
            {
                WLDebug.Log($"<color=green>HallManager.OnSocketClose: code = {errorCode}, try reconnect!</color>");

                Reconnect();
            }
        }
        #endregion


        #region Receiving
        private void OnMsgUserJoinReply(MsgHeader msg)
        {
            int code = msg.ReadByte();
            WLDebugTrace.Trace("[HallManager] OnMsgUserJoinReply, code=" + code);
            if (code != 0)
            {
                WLDebug.LogWarning("HallManager.OnMsgUserJoinReply: 进入大厅失败", code);
                _cs?.Dispose();
                _cs = null;
                return;
            }

            _hallServerId = msg.ReadWord();
            GameInfoKit.DeserializeGameInfo(msg);
            RoomInfoKit.DeserializeRoomInfo(msg);
            RoomInfoKit.DeserializeRelationInfo(msg);
            UserInfoKit.DeserializeUserInfo(msg);

            for (int i = 0; i < Constants.EFFORT_DATA_COUNT; i++)
            {
                EffortDataKit.SetEffortData(i, msg.ReadInt64());
            }
            for (int i = 0; i < Constants.EFFORT_DATA_COUNT; i++)
            {
                EffortDataKit.SetDailyData(i, msg.ReadInt64());
            }

            uint lastRoomID = 0;
            if (msg.ReadUint32() != 0)
            {
                lastRoomID = msg.ReadUint32();
            }

            WLDebug.Info($"进入大厅成功。code = {code}, lastRoomId = {lastRoomID}");
            //GameApp.UpdateReconnectState(false, true);

            //DoGetWearClothes();
            //DoGetBagClothes();

            if (lastRoomID > 0)
            {
                JoinRoom(lastRoomID);
            }
            else if (QuickConfig.autoJoinRoom)
            {
                JoinRoom(QuickConfig.autoJoinRoomId);
            }
            else
            {
                // 进入大厅场景
                HallStage.EnterHallStage();
            }

            // 开始心跳
            _cs?.StartKeepAlive();
        }

        private void OnMsgJoinGameReply(MsgHeader msg)
        {

        }

        private void OnMsgChangeUserInfoReply(MsgHeader msg)
        {

        }

        private void OnMsgUpdateUserData(MsgHeader msg)
        {

        }

        private void OnMsgExchangeMoneyReply(MsgHeader msg)
        {

        }

        private void OnQueryChangePasswordReply(MsgHeader msg)
        {

        }

        private void OnGetCaptchaReply(MsgHeader msg)
        {

        }

        private void OnClientHallBindAccountReply(MsgHeader msg)
        {

        }

        private void OnMsgSystemMessage(MsgHeader msg)
        {

        }

        private void OnMsgQueryRoomKeyReply(MsgHeader msg)
        {
            uint dwRoomKey = msg.ReadUint32();
            uint roomId = msg.ReadUint32();

            if (roomId > 0)
            {
                string key = dwRoomKey.ToString().PadLeft(6, '0');
                JoinRoom(roomId, key, () => (_roomManager as CustomRoomManager).JoinGroup(key));
            }
            else
            {
                //NotificationCenter.PostNotification(WLEvent.HALL_QUERY_ROOMKEY_FAILED);
                //ToastMessageManager.PushMessage("房间不存在,请重新输入！");
                WLDebug.LogWarning("房间不存在,请重新输入！");
            }
        }
        #endregion


        #region Sending
        private void SendHallUserJoin()
        {
            // TODO: replace hard coded
            uint loadFlag = 0xFFFFffff;
            uint gameId = 0;
            string deviceCode = "3626EF4D4BF53AB0F8C72DE74456961D14E99B85";

            MsgHeader msg = NetMsgFactory.GetNetMsgOut();
            msg.messageID = HallMsgDefine.MSG_HALL_USER_JOIN;

            byte[] sessionBytes = EncodeHelper.HexStringToData(_session);
            msg.WriteBytes(sessionBytes);
            msg.WriteUint32(_userId);
            msg.WriteUint32(loadFlag);
            msg.WriteUint32(gameId);

            for (int i = 0; i < Constants.DEVICE_CODE_LEN; i++)
            {
                if (i < deviceCode.Length)
                {
                    msg.WriteByte((byte)deviceCode[i]);
                }
                else
                {
                    msg.WriteByte(0);
                }
            }

            SendData(msg, Constants.CRYPT_MSG);
        }
        #endregion


        /// <summary>
        /// 创建房间管理器
        /// </summary>
        /// <returns></returns>
        private BaseRoomManager CreateRoomManager(RoomInfo roomInfo)
        {
            uint roomType = roomInfo.type;
            int nRoomType = FunctionEx.GetRoomMode(roomType);
            if (nRoomType == Constants.ROOM_TYPE_ALLOCSIT) // 金币场
            {
                return new AllocSitRoomManager();
            }
            else if (nRoomType == Constants.ROOM_TYPE_FRIEND_TEAM) // 朋友场
            {
                return new CustomRoomManager();
            }
            return null;
        }

        protected override bool NeedsWaiting(MsgHeader msg)
        {
            return false;
        }
    }
}

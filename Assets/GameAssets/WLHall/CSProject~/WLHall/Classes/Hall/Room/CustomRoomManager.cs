// @Author: xuzhihu
// @Date: 2021/5/10 15:03:53 
// @Description:

using System.Collections.Generic;
using Unity.Utility;
using WLCore;

namespace WLHall
{
    public class CustomRoomManager : BaseRoomManager
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public string roomKey { get; private set; }

        /// <summary>
        /// 房主用户ID
        /// </summary>
        public uint roomOwnerId { get; private set; }

        /// <summary>
        /// 是否亲友圈
        /// </summary>
        public bool isQinYouQuan { get; private set; }


        private bool _gpsEnable;
        private string _clubId;

        public CustomRoomManager()
        {
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ROOMINFO_REPLY, OnMsgInfoReply);                    // 4097
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_TEAM_CREATE_NOTIFY, OnRoomTeamCreateNotify);        // 创建队伍应答（4128）
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ALLOC_SIT_DESK_INFO, OnMsgAllocSitDeskInfo);        // 桌子信息 （4125）
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ALLOC_SIT, OnMsgAllocSit);                          // 其他玩家坐下 （4124）
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_READY_NOTIFY, OnRoomPlayerReady);                   // 收到玩家准备消息 （4105）
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_SIT_DOWN_NOTIFY, OnRoomSitDownNotify);              // 玩家坐下广播 服务器发送   （4101）
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_STAND_UP_NOTIFY, OnMsgStandUp);                     // 玩家起立消息 4104
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_TEAM_CREATE_FAILED, OnRoomTeamCreateFailed);        // 创建队伍失败
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_SIT_DOWN_FAILED, OnRoomSitDownFailed);              // 落座失败
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_DESK_KICK_PLAYER, OnRoomDeskKickPlayer);            //服务器桌子踢人
            //RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_UPDATE_PLAYER_DATA_NOTIFY, OnMsgUpdatePlayerData);
        }


        /// <summary>
        /// 创建朋友场
        /// </summary>
        /// <param name="rules"></param>
        public void CreateGroup(List<byte> rules)
        {
            if (_playerSelf == null)
            {
                return;
            }

            if (ReturnGroup())
            {
                return;
            }

            if (rules.Count <= 0)
            {
                WLDebug.LogWarning("CustomRoomManager.CreateCustomFriendRoom: 选择规则错误！！！");
                return;
            }

            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = RoomMsgDefine.MSG_ROOM_TEAM_CREATE;

            foreach (byte value in rules)
            {
                msg.WriteByte(value);
            }
            SendData(msg);

            // todo xzh 保存再来一局信息

        }


        /// <summary>
        /// 加入朋友场
        /// </summary>
        /// <param name="roomkey"></param>
        /// <param name="gpsEnable"></param>
        /// <param name="chairID"></param>
        public void JoinGroup(string roomkey, bool gpsEnable = false, ushort chairID = Constants.INVALID_CHAIR)
        {
            roomkey = string.IsNullOrEmpty(roomkey) ? roomKey : roomkey;

            if (string.IsNullOrEmpty(roomkey))
            {
                return;
            }

            roomKey = roomkey;
            
            // todo
            //if (gpsenable == 0)
            //{
            //    gpsenable = (byte)GPS.GetGpsEnable();
            //}
            //if (!GPS.IsSupportGps())
            //{
            //    gpsenable = 1;
            //}

            MsgHeader msg = NetMsgFactory.GetNetMsgOut(Constants.GAME_MSG_HEADER_SIZE);
            msg.messageID = RoomMsgDefine.MSG_ROOM_SIT_DOWN;
            msg.WriteUint16(chairID);
            msg.WriteStringA(roomkey);
            msg.WriteByte((byte)(gpsEnable ? 1 : 0));

            // TODO: 好友圈 黑名单
            ushort count = 0;
            msg.WriteUint16(count);
            msg.WriteUint16(count);

            SendData(msg);
        }


        /// <summary>
        /// 返回朋友场
        /// </summary>
        /// <returns></returns>
        public bool ReturnGroup()
        {
            if (playerSelf != null && _playerSelf.chairId != Constants.INVALID_CHAIR)
            {
                JoinGroup(roomKey, _gpsEnable, playerSelf.chairId);

                //NotificationCenter.PostNotification(WLEvent.ROOM_JOIN_NOTIFY, null);

                return true;
            }

            return false;
        }


        #region Receiving
        private void OnMsgInfoReply(MsgHeader msg)
        {
            MsgRoomInfoReply data = new MsgRoomInfoReply(msg);

            _playerSelf = data.player;

            if (!string.IsNullOrEmpty(data.strRoomKey))
            {
                roomKey = data.strRoomKey;
            }

            int isGaming = 0;
            UserGameInfo userGameInfo = data.player.userGameInfo;
            if (userGameInfo.wChairID != Constants.INVALID_CHAIR) // 有桌位，直接开始
            {
                roomKey = msg.ReadStringA();
                roomOwnerId = msg.ReadUint32();
                isGaming = msg.ReadByte();
                _gpsEnable = msg.ReadByte() == 1;
                isQinYouQuan = msg.ReadByte() == 1;
            }

            onInfoReply?.Invoke();

            if (!string.IsNullOrEmpty(roomKey) && isGaming == 0)
            {
                ReturnGroup();
            }
            else
            {
                //if (string.IsNullOrEmpty(_roomKey) && GameApp.IsReconnecting())
                //{
                //    NotificationCenter.PostNotification(WLEvent.HALL_ON_JOIN_HALL, 2);
                //    GameApp.UpdateReconnectState(false, true);
                //    _hallManager?.ExitRoom();
                //}
                //else
                //{
                //    //房间初始化通知
                //    NotificationCenter.PostNotification(WLEvent.ROOM_JOIN_ROOM_REPLY, this);
                //}
            }
        }

        private void OnRoomTeamCreateNotify(MsgHeader msg)
        {
            roomKey = msg.ReadStringA();

            _gpsEnable = msg.ReadByte() == 1;

            isQinYouQuan = msg.ReadByte() == 1;

            if (_playerSelf != null)
            {
                roomOwnerId = playerSelf.GetID();
            }
            //NotificationCenter.PostNotification(WLEvent.ROOM_JOIN_NOTIFY);
        }

        private void OnMsgAllocSitDeskInfo(MsgHeader msg)
        {
            if (_playerSelf == null)
            {
                return;
            }

            ushort chairId = msg.ReadUint16();
            ushort otherPlayerCount = msg.ReadUint16();

            _playerSelf.chairId = chairId;
            _playerSelf.deskId = 0;

            var otherUserInfos = new List<UserInfo>();
            var otherUserGameInfos = new List<UserGameInfo>();
            for (int i = 0; i < otherPlayerCount; i++)
            {
                otherUserInfos.Add(UserInfo.From(msg));
                otherUserGameInfos.Add(UserGameInfo.From(msg));
            }
            _deskID = msg.ReadUint16();

            if (_gameStage == null)
            {
                if (!StartGame(() => OnGameSceneLoaded(otherPlayerCount, otherUserInfos, otherUserGameInfos)))
                {
                    WLDebug.LogWarning("启动游戏失败！");
                    return;
                }
            }
            else
            {
                // todo xzh 通知游戏玩家重置
            }
        }

        private void OnGameSceneLoaded(int otherPlayerCount, List<UserInfo> otherUserInfos, List<UserGameInfo> otherUserGameInfos)
        {
            BasePlayer gamePlayerSelf = _gameStage.OnCreatePlayer();
            gamePlayerSelf.userInfo = _playerSelf.userInfo;
            gamePlayerSelf.userGameInfo = _playerSelf.userGameInfo;
            _gameStage.SetPlayer(gamePlayerSelf);
            _gameStage.OnPlayerJoin(gamePlayerSelf, true);

            for (int i = 0; i < otherPlayerCount; i++)
            {
                var player = _gameStage.OnCreatePlayer();
                player.userInfo = otherUserInfos[i];
                player.userGameInfo = otherUserGameInfos[i];

                _gameStage.SetPlayer(player);
                _gameStage.OnPlayerJoin(player, false);
            }

            _gameStage.clubId = _clubId;
        }

        private void OnRoomPlayerReady(MsgHeader msg)
        {
            uint playerID = msg.ReadDword();
            int ready = msg.ReadByte();
            // todo xzh 通知游戏？ 有收到33,是不是可以不用处理
        }

        private void OnMsgAllocSit(MsgHeader msg)
        {
            var body = MsgRoomAllocSit.From(msg);

            if (_gameStage == null)
            {
                return;
            }

            var pPlayer = _gameStage.OnCreatePlayer();
            pPlayer.SetUserInfo(body.userInfo);
            pPlayer.SetUserGameInfo(body.userGameInfo);
            _gameStage.SetPlayer(pPlayer);
            _gameStage.OnPlayerJoin(pPlayer, false);
        }

        private void OnRoomSitDownNotify(MsgHeader msg)
        {
            roomOwnerId = msg.ReadUint32();

            ushort playerCount = msg.ReadUint16();

            _gpsEnable = msg.ReadByte() == 1;

            if (playerCount > 0)
            {
                //NotificationCenter.postNotification(WLEvent.ROOM_JOIN_NOTIFY);
                //LoadingLayerManager.removeLayer();

                var playerInfos = new List<SelectSeatView.PlayerInfo>();
                for (var i = 0; i < playerCount; i++)
                {
                    var playerInfo = new SelectSeatView.PlayerInfo
                    {
                        chairId = msg.ReadWord(),
                        userId = msg.ReadDword(),
                        gender = msg.ReadInt32(),
                        nickname = msg.ReadStringW(),
                        avatarId = msg.ReadDword(),
                        avatarUrl = ""
                    };
                    if (playerInfo.avatarId >= 0x7FFFFFFF)
                    {
                        playerInfo.avatarUrl = msg.ReadStringA();
                    }
                    playerInfos.Add(playerInfo);
                }

                if (!_roomInfo.cmd.ContainsKey("resit")) // 如果不是自由换座
                {
                    //var selectSeatView = WDirector.GetRootLayer().GetChildByTag(TagDefine.SELECT_SEAT_VIEW);
                    //// 显示选座界面
                    //if (selectSeatView != null)
                    //{
                    //    // 更新选座界面人数
                    //    (selectSeatView as SelectSeatView).UpdateSeats(playerInfos);
                    //}
                    //else
                    //{
                    //    // 获取游戏类型
                    //    var isPuke = true;
                    //    if ((gameInfo.type & Constants.GAME_GROUP_MAHJONG) != 0)
                    //    {
                    //        isPuke = false;
                    //    }

                    //    // 选座界面
                    //    SelectSeatView.Display(playerInfos, roomInfo.playersPerDesk, roomKey, isPuke);
                    //}


                    // 方便调试，先跳过选座，直接加入
                    for (ushort chairId = 0; chairId < roomInfo.playersPerDesk; chairId++)
                    {
                        bool chairEmpty = true;
                        foreach (var pinfo in playerInfos)
                        {
                            if (pinfo.chairId == chairId || chairId == playerSelf.chairId)
                            {
                                chairEmpty = false;
                                break;
                            }
                        }
                        if (chairEmpty)
                        {
                            JoinGroup(roomKey, false, chairId);

                            break;
                        }
                    }

                }
            }
            else
            {
                //NotificationCenter.postNotification(WLEvent.CLOSE_SELECT_SEAT_NOTIFY, false);
            }

            isQinYouQuan = msg.ReadByte() == 1;

            if (msg.position < msg.len)
            {
                _clubId = msg.ReadStringA();

                if (_gameStage != null)
                {
                    _gameStage.clubId = _clubId;
                }
            }
        }

        /// <summary>
        /// 玩家起立 （4104）
        /// </summary>
        /// <param name="msg"></param>
        private void OnMsgStandUp(MsgHeader msg)
        {
            uint userId = msg.ReadUint32();

            if (_playerSelf != null && userId == _playerSelf.GetID())
            {
                _playerSelf.deskId = Constants.INVALID_DESK;
                _playerSelf.chairId = Constants.INVALID_CHAIR;
            }
            else if (_gameStage != null)
            {
                BasePlayer player = _gameStage.GetPlayerByUserId(userId);

                if (player != null)
                {
                    // 标记玩家已经离开
                    player.left = true;

                    // 通知游戏玩家离开
                    _gameStage.OnPlayerLeave(player);
                }
            }
        }

        private void OnRoomTeamCreateFailed(MsgHeader msg)
        {
            string result = msg.ReadStringW();
            // todo xzh
            //DialogLayerUtils.showOK(wmsg);
            _hallManager?.ExitRoom();

            WLDebug.LogWarning("创建朋友场失败,", result);
        }

        /// <summary>
        /// 落座失败
        /// </summary>
        /// <param name="msg"></param>
        private void OnRoomSitDownFailed(MsgHeader msg)
        {
            string wmsg = msg.ReadStringW();

            Dictionary<string, string> tipsDic = EncodeHelper.TableToObject(wmsg);

            int code = int.Parse(tipsDic["code"]);

            // 朋友场自由选座功能相关，不退出游戏
            if (_roomInfo.cmd.ContainsKey("resit") && code >= 11 && code <= 17)
            {
                //ToastMessageManager.PushMessage(tipsDic["msg"]);
                return;
            }

            //if (code == 1)
            //{
            //    GPS.OpenAlertView((bAuth) =>
            //    {
            //        HallManager hallManager = GameApp.GetHallManager();
            //        if (bAuth && hallManager != null && !string.IsNullOrEmpty(RoomInfoUtils.GetLastTryJoinRoomKey()))
            //        {
            //            hallManager.JoinFriendRoomByRoomKey(RoomInfoUtils.GetLastTryJoinRoomKey());
            //            // 防止授权服务端和客户端授权状态判断不一样，导致重复加入
            //            RoomInfoUtils.ClearLastTryJoinRoomKey();
            //        }
            //        else
            //        {
            //            DialogLayerUtils.ShowOK(tipsDic["msg"] ?? wmsg);
            //        }
            //    }, "当前房间是GPS场，是否开启GPS加入该房间。（不开启无法加入该房间）");
            //}
            //else
            //{
            //    DialogLayerUtils.ShowOK(tipsDic["msg"] ?? wmsg);
            //}

            _hallManager?.ExitRoom();
            //NotificationCenter.PostNotification(WLEvent.CLOSE_SELECT_SEAT_NOTIFY, true);
            //NotificationCenter.PostNotification(WLEvent.ROOM_SITEDOWN_FAILED, code);
            
        }

        private void OnRoomDeskKickPlayer(MsgHeader msg)
        {
            if (msg.len != msg.position)
            {
                string amsg = msg.ReadStringW();
                if (string.IsNullOrEmpty(amsg))
                {
                    amsg = "您已被请出";
                }
                // todo xzh
                //DialogLayerUtils.showOK(amsg);
            }

            _gameStage?.Exit(); // TODO: 放在提示弹窗确定事件中

            _hallManager?.ExitRoom();
        }
        #endregion

        protected override bool NeedsWaiting(MsgHeader msg)
        {
            int offset = Constants.GetMsgOffset(msg.messageID);

            if (offset == Constants.MSG_OFFSET_BASE_LOGIC ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_ALLOC_SIT ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_DESK_KICK_PLAYER ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_UPDATE_PLAYER_DATA_NOTIFY ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_READY_NOTIFY)
            {
                return true;
            }

            return false;
        }
    }
}

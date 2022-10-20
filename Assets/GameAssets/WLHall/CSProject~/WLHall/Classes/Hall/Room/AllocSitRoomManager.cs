
using System.Collections.Generic;
using WLCore;

namespace WLHall
{

    /// <summary>
    /// 系统分配座位房间管理器
    /// </summary>
    public class AllocSitRoomManager : BaseRoomManager
    {

        public AllocSitRoomManager()
        {
            // 注册房间事件
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ROOMINFO_REPLY, OnMsgInfoReply);  //4097
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ALLOC_SIT, OnMsgAllocSit);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_ALLOC_SIT_DESK_INFO, OnMsgAllocSitDeskInfo);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_STAND_UP_NOTIFY, OnMsgStandup);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_DESK_KICK_PLAYER, OnMsgDeskKickPlayer);
            RegisterMessageProcessor(RoomMsgDefine.MSG_ROOM_UPDATE_PLAYER_DATA_NOTIFY, OnMsgUpdatePlayerData);
        }


        #region Receiving
        private void OnMsgInfoReply(MsgHeader msg)
        {
            MsgRoomInfoReply reply = new MsgRoomInfoReply(msg);

            if (reply.player != null)
            {
                _playerSelf = reply.player;
            }

            if (!StartGame())
            {
                return;
            }

            // 调用换桌
            ChangeDesk();
        }

        private void OnMsgAllocSit(MsgHeader msg)
        {
            if (_gameStage == null)
            {
                return;
            }

            MsgRoomAllocSit data = MsgRoomAllocSit.From(msg);
            BasePlayer gamePlayer = _gameStage.OnCreatePlayer();
            gamePlayer.userInfo = data.userInfo;
            gamePlayer.userGameInfo = data.userGameInfo;
            _gameStage.SetPlayer(gamePlayer);
            _gameStage.OnPlayerJoin(gamePlayer, false);
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

            BasePlayer gamePlayerSelf = _gameStage.OnCreatePlayer();
            gamePlayerSelf.userInfo = _playerSelf.userInfo;
            gamePlayerSelf.userGameInfo = _playerSelf.userGameInfo;
            _gameStage.SetPlayer(gamePlayerSelf);
            _gameStage.OnPlayerJoin(gamePlayerSelf, true);

            for (int i = 0; i < otherPlayerCount; i++)
            {
                BasePlayer gamePlayer = _gameStage.OnCreatePlayer();
                gamePlayer.userInfo = UserInfo.From(msg);
                gamePlayer.userGameInfo = UserGameInfo.From(msg);
                _gameStage.SetPlayer(gamePlayer);
                _gameStage.OnPlayerJoin(gamePlayer, false);
            }

            _deskID = msg.ReadUint16();
        }

        private void OnMsgStandup(MsgHeader msg)
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

        private void OnMsgDeskKickPlayer(MsgHeader msg)
        {
            string reason = "";

            if (msg.len != msg.position)
            {
                reason = msg.ReadStringW();
            }

            _hallManager.ExitRoom();

            _gameStage?.Exit();

            // TODO: 弹窗提示
        }

        private void OnMsgUpdatePlayerData(MsgHeader msg)
        {
            MsgRoomUpdatePlayerDataNotify body = MsgRoomUpdatePlayerDataNotify.From(msg);

            if (_playerSelf != null && _playerSelf.GetID() == body.dwUserID)
            {
                UpdatePlaySelf(body);

                if (body.bNotifyClient)
                {
                    UpdateGamePlayerSelf();
                }
            }
            else if (_gameStage != null)
            {
                UpdateOtherGamePlayer(body);
            }
        }

        private void UpdatePlaySelf(MsgRoomUpdatePlayerDataNotify data)
        {
            var info = _playerSelf.GetUserGameInfo();
            info.llScore = data.llScore;
            info.dwWinCount = data.dwWinCount;
            info.dwLostCount = data.dwLostCount;
            info.dwDrawCount = data.dwDrawCount;
            info.dwFleeCount = data.dwFleeCount;
            info.nGameData1 = data.nGameData1;
            info.nGameData2 = data.nGameData2;
            info.dwPKAwardCount = data.dwPKAwardCount;
            info.dwPKKempCount = data.dwPKKempCount;
            info.cbTaskData = data.cbTaskData;
            info.cbMatchData = data.cbMatchData;
            info.dwCustom = data.dwCustom;
            info.wReserved = data.wReserved;
            _playerSelf.GetUserInfo().money = data.llMoney;
            _playerSelf.GetUserInfo().dwPrestige = data.dwPrestige;
        }

        private void UpdateGamePlayerSelf()
        {
            //同步给客户端
            if (_gameStage == null)
            {
                return;
            }

            var gamePlayerSelf = _gameStage.GetPlayerByUserId(_playerSelf.GetID());
            if (gamePlayerSelf == null)
            {
                return;
            }

            gamePlayerSelf.GetUserInfo().money = _playerSelf.GetMoney();
            gamePlayerSelf.GetUserInfo().bankMoney = _playerSelf.GetUserInfo().bankMoney;

            var pInfo = gamePlayerSelf.GetUserGameInfo();
            var roomplayerinfo = _playerSelf.GetUserGameInfo();
            pInfo.llScore = roomplayerinfo.llScore;
            pInfo.dwWinCount = roomplayerinfo.dwWinCount;
            pInfo.dwLostCount = roomplayerinfo.dwLostCount;
            pInfo.dwDrawCount = roomplayerinfo.dwDrawCount;
            pInfo.dwFleeCount = roomplayerinfo.dwFleeCount;
            pInfo.nGameData1 = roomplayerinfo.nGameData1;
            pInfo.nGameData2 = roomplayerinfo.nGameData2;
            pInfo.dwPKAwardCount = roomplayerinfo.dwPKAwardCount;
            pInfo.dwPKKempCount = roomplayerinfo.dwPKKempCount;
            pInfo.cbTaskData = new List<byte>(roomplayerinfo.cbTaskData).ToArray();
            pInfo.cbMatchData = new List<byte>(roomplayerinfo.cbMatchData).ToArray();
            pInfo.dwCustom = roomplayerinfo.dwCustom;
            pInfo.wReserved = roomplayerinfo.wReserved;
            gamePlayerSelf.SetUserGameInfo(pInfo);
            _gameStage.OnUpdatePlayerInfo(gamePlayerSelf);
        }

        private void UpdateOtherGamePlayer(MsgRoomUpdatePlayerDataNotify data)
        {
            var otherGamePlayer = _gameStage.GetPlayerByUserId(data.dwUserID);

            if (otherGamePlayer == null)
            {
                return;
            }

            var pInfo = otherGamePlayer.GetUserGameInfo();

            pInfo.llScore = data.llScore;
            pInfo.dwWinCount = data.dwWinCount;
            pInfo.dwLostCount = data.dwLostCount;
            pInfo.dwDrawCount = data.dwDrawCount;
            pInfo.dwFleeCount = data.dwFleeCount;
            pInfo.nGameData1 = data.nGameData1;
            pInfo.nGameData2 = data.nGameData2;
            pInfo.dwPKAwardCount = data.dwPKAwardCount;
            pInfo.dwPKKempCount = data.dwPKKempCount;
            pInfo.cbTaskData = data.cbTaskData;
            pInfo.cbMatchData = data.cbMatchData;
            pInfo.dwCustom = data.dwCustom;
            pInfo.wReserved = data.wReserved;

            otherGamePlayer.GetUserInfo().money = data.llMoney;
            otherGamePlayer.GetUserInfo().dwPrestige = data.dwPrestige;

            if (data.bNotifyClient)
            {
                //同步给客户端
                _gameStage.OnUpdatePlayerInfo(otherGamePlayer);
            }
        }

        #endregion


        #region Inherit
        protected override bool NeedsWaiting(MsgHeader msg)
        {
            int offset = Constants.GetMsgOffset(msg.messageID);

            if (offset == Constants.MSG_OFFSET_BASE_LOGIC ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_ALLOC_SIT ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_ALLOC_SIT_DESK_INFO ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_DESK_KICK_PLAYER ||
                msg.messageID == RoomMsgDefine.MSG_ROOM_UPDATE_PLAYER_DATA_NOTIFY)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}

// @Author: tanjinhua
// @Date: 2021/8/16  15:39

using System;
using WLCore;
using WLCore.Stage;

namespace WLHall
{
    public abstract class BaseGameStage : BaseStage
    {
        /// <summary>
        /// 场景加载完成事件
        /// </summary>
        internal Action sceneLoaded;

        /// <summary>
        /// 房间管理器
        /// </summary>
        public BaseRoomManager roomManager => _roomManager;

        /// <summary>
        /// 房间信息
        /// </summary>
        public RoomInfo roomInfo => _roomInfo;

        /// <summary>
        /// 游戏信息
        /// </summary>
        public GameInfo gameInfo => _gameInfo;

        /// <summary>
        /// 亲友圈id
        /// </summary>
        public string clubId { get; internal set; }

        /// <summary>
        /// 是否是朋友场
        /// </summary>
        public virtual bool isFriendRoom => _roomInfo.type >> 24 == 4;

        /// <summary>
        /// 是否是亲友圈
        /// </summary>
        public virtual bool isQinYouQuan => isFriendRoom && (_roomManager as CustomRoomManager).isQinYouQuan;

        /// <summary>
        /// 用户自己的玩家对象
        /// </summary>
        public virtual BasePlayer playerSelf => GetPlayerByUserId(_roomManager.playerSelf.userId);

        /// <summary>
        /// 最大玩家数量
        /// </summary>
        public virtual int maxPlayerCount => _roomInfo.playersPerDesk;

        /// <summary>
        /// 房间内当前玩家数量
        /// </summary>
        public virtual int currentPlayerCount
        {
            get
            {
                int result = 0;
                for (int i = 0; i < maxPlayerCount; i++)
                {
                    if (_players[i] != null && !_players[i].left)
                    {
                        result++;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 是否有玩家离开了
        /// </summary>
        public virtual bool anyPlayerLeft => !isFriendRoom && currentPlayerCount < maxPlayerCount;

        /// <summary>
        /// 玩家自己是否是房主
        /// </summary>
        public virtual bool selfIsRoomOwner => isFriendRoom && (_roomManager as CustomRoomManager).roomOwnerId == playerSelf.userId;



        private BaseRoomManager _roomManager;
        private RoomInfo _roomInfo;
        private GameInfo _gameInfo;
        private BasePlayer[] _players;
        private GameMessageManager _messageMgr;

        public BaseGameStage(BaseRoomManager roomMgr)
        {
            _roomManager = roomMgr;
            _roomInfo = _roomManager.roomInfo;
            _gameInfo = _roomManager.gameInfo;
            _players = new BasePlayer[maxPlayerCount];
            _messageMgr = new GameMessageManager(roomManager);
        }

        /// <summary>
        /// 初始化，实例化游戏Stage时触发
        /// </summary>
        public override void OnInitialize()
        {
            LoadScene(GetBuildSceneName(), () => HandleSceneLoaded());
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void OnUpdate(float deltaTime)
        {
            _messageMgr?.Update();
        }

        /// <summary>
        /// 场景加载完毕事件
        /// </summary>
        protected virtual void OnSceneLoaded()
        {
        }

        /// <summary>
        /// 获取BuildSetting中对应场景名称
        /// </summary>
        /// <returns></returns>
        protected abstract string GetBuildSceneName();

        /// <summary>
        /// 注册游戏消息监听方法
        /// </summary>
        /// <param name="msgId"></param>
        /// <param name="processor"></param>
        public void RegisterMessageProcessor(ushort msgId, Action<MsgHeader> processor)
        {
            _messageMgr?.RegisterMessageProcessor(msgId, processor);
        }

        public void OnProcessMessage(MsgHeader msg)
        {
            _messageMgr?.OnProcessMessage(msg);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="flag"></param>
        public void SendData(MsgHeader msg, byte flag = MsgHeader.Flag.None)
        {
            _roomManager?.SendData(msg, flag);
        }

        /// <summary>
        /// 退出游戏场景时触发
        /// </summary>
        public override void OnShutdown()
        {
            _roomManager = null;
            _roomInfo = null;
            _gameInfo = null;
            _messageMgr?.Dispose();
            _messageMgr = null;
            _players = null;
        }

        /// <summary>
        /// 退出游戏，返回大厅场景
        /// </summary>
        public virtual void Exit()
        {
            _roomManager?.StandUp();

            HallStage.EnterHallStage();
        }

        #region Player Management
        /// <summary>
        /// 创建玩家对象
        /// </summary>
        /// <returns></returns>
        public virtual BasePlayer OnCreatePlayer()
        {
            return new BasePlayer();
        }


        /// <summary>
        /// 设置玩家对象
        /// </summary>
        /// <param name="player"></param>
        public void SetPlayer(BasePlayer player)
        {
            _players[player.chairId] = player;
        }


        /// <summary>
        /// 根据用户ID获取玩家对象
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public BasePlayer GetPlayerByUserId(uint userId)
        {
            for (int i = 0; i < _players.Length; i++)
            {
                BasePlayer player = _players[i];
                if (player != null && player.GetID() == userId)
                {
                    return player;
                }
            }

            return null;
        }


        /// <summary>
        /// 根据服务器座位号获取玩家对象
        /// </summary>
        /// <param name="chairId"></param>
        /// <returns></returns>
        public BasePlayer GetPlayerByChairId(int chairId)
        {
            return _players[chairId];
        }
        #endregion


        #region Notices
        /// <summary>
        /// 玩家加入通知
        /// </summary>
        /// <param name="player"></param>
        public abstract void OnPlayerJoin(BasePlayer player, bool isSelf);


        /// <summary>
        /// 玩家离开通知
        /// </summary>
        /// <param name="player"></param>
        public abstract void OnPlayerLeave(BasePlayer player);


        /// <summary>
        /// 使用道具成功通知
        /// </summary>
        /// <param name="fromUserId"></param>
        /// <param name="toUserId"></param>
        /// <param name="propId"></param>
        /// <param name="usedCount"></param>
        public abstract void OnUsePropSuccessed(uint fromUserId, uint toUserId, uint propId, int usedCount);


        /// <summary>
        /// 使用道具失败通知
        /// </summary>
        /// <param name="errMsg"></param>
        public abstract void OnUsePropFailed(string errMsg);


        /// <summary>
        /// 玩家豆豆等信息发生改变通知
        /// </summary>
        /// <param name="player"></param>
        public abstract void OnUpdatePlayerInfo(BasePlayer player);
        #endregion



        #region Utils
        /// <summary>
        /// 服务器座位号转换成以relativeChairId视角为准的视图座位号
        /// </summary>
        /// <param name="serverChairId"></param>
        /// <param name="relativeChairId"></param>
        /// <returns></returns>
        public virtual int ToViewChairIdRelative(int serverChairId, int relativeChairId)
        {
            int playersPerDesk = maxPlayerCount;

            return (serverChairId + playersPerDesk - relativeChairId) % playersPerDesk;
        }


        /// <summary>
        /// 服务器座位号转换成视图座位号
        /// </summary>
        /// <param name="serverChairId"></param>
        /// <returns></returns>
        public virtual int ToViewChairId(int serverChairId)
        {
            int selfChairId = playerSelf.chairId;

            return ToViewChairIdRelative(serverChairId, selfChairId);
        }


        /// <summary>
        /// 视图座位号转换成服务器座位号
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        public virtual int ToServerChairId(int viewChairId)
        {
            int selfChairId = playerSelf.chairId;

            int playersPerDesk = maxPlayerCount;

            return (viewChairId + selfChairId) % playersPerDesk;
        }


        /// <summary>
        /// 玩家是否破产了
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool IsPlayerBankruptcy(BasePlayer player)
        {
            return !isFriendRoom && player.GetMoney() < roomInfo.minMoney;
        }

        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="chairId"></param>
        /// <returns></returns>
        public bool IsPlayerBankruptcy(int chairId)
        {
            var player = GetPlayerByChairId(chairId);

            return player != null && IsPlayerBankruptcy(player);
        }

        /// <summary>
        /// 遍历玩家
        /// </summary>
        /// <param name="handler"></param>
        public void TraversePlayer(Action<BasePlayer> handler)
        {
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                if (_players[i] != null)
                {
                    handler?.Invoke(_players[i]);
                }
            }
        }

        /// <summary>
        /// 同上
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void TraversePlayer<T>(Action<T> handler) where T : BasePlayer
        {
            for (int i = 0; i < maxPlayerCount; ++i)
            {
                if (_players[i] != null)
                {
                    handler?.Invoke(_players[i] as T);
                }
            }
        }
        #endregion

        private void HandleSceneLoaded()
        {
            OnSceneLoaded();

            sceneLoaded?.Invoke();
        }
    }
}

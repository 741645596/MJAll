// @Description :
// @Author： chenfaxin
// @Date : 2021/3/30 星期二 11:05:33

namespace WLHall
{
    public static class RoomMsgDefine
    {
        public const ushort MSG_ROOM_JOIN_NOTIFY = 4096; // 进入房间广播,服务器发送 4096
        public const ushort MSG_ROOM_ROOMINFO_REPLY = MSG_ROOM_JOIN_NOTIFY + 1;     // 房间信息, 服务器发送
        public const ushort MSG_ROOM_UPDATE_PLAYER_DATA_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 2;     //!<更新玩家数据 服务器发送 @see msg_room_update_player_data_notify
        public const ushort MSG_ROOM_PLAYER_STATE_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 3;     //!<更新玩家状态广播 服务器发送 @see msg_room_player_state_notify
        public const ushort MSG_ROOM_SIT_DOWN = MSG_ROOM_JOIN_NOTIFY + 4;     //!<玩家请求坐下 大厅发送 @see msg_room_sit_down
        public const ushort MSG_ROOM_SIT_DOWN_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 5;     //!<玩家坐下广播 服务器发送 @see msg_room_sit_down_notify
        public const ushort MSG_ROOM_SIT_DOWN_FAILED = MSG_ROOM_JOIN_NOTIFY + 6;     //!<玩家坐下失败 服务器发送 @see msg_room_sit_down_failed
        public const ushort MSG_ROOM_STAND_UP = MSG_ROOM_JOIN_NOTIFY + 7;     //!<玩家起立 大厅发送 @see msg_room_stand_up
        public const ushort MSG_ROOM_STAND_UP_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 8;     //!<玩家起立广播 服务器发送 @see msg_room_stand_up_notify
        public const ushort MSG_ROOM_READY_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 9;     //!<玩家准备广播 服务器发送 @see msg_room_ready_notify
        public const ushort MSG_ROOM_PLAYER_LEAVE_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 10;    //!<玩家离开广播 服务器发送 @see msg_room_player_leave_notify
        public const ushort MSG_ROOM_DESK_STATE_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 11;    //!<桌子状态改变广播 服务器发送 @see msg_room_desk_state_notify
        public const ushort MSG_ROOM_USERSETTING = MSG_ROOM_JOIN_NOTIFY + 12;    //!<玩家设置 大厅发送 @see msg_room_usersetting
        public const ushort MSG_ROOM_CHAT = MSG_ROOM_JOIN_NOTIFY + 13;    //!<房间聊天 大厅发送
        public const ushort MSG_ROOM_CHAT_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 14;    //!<房间聊天广播 服务器发送
        public const ushort MSG_ROOM_USE_PROP = MSG_ROOM_JOIN_NOTIFY + 15;    //!<玩家使用道具 大厅发送
        public const ushort MSG_ROOM_USE_PROP_REPLY = MSG_ROOM_JOIN_NOTIFY + 16;    //!<玩家使用道具广播 服务器发送
        public const ushort MSG_ROOM_DESK_KICK_PLAYER = MSG_ROOM_JOIN_NOTIFY + 17;    //!<服务器桌子踢人 （调用 @ref _BaseGameServer::CBaseDesk::KickPlayer 时发送)
        public const ushort MSG_ROOM_PLAYER_KICK_PLAYER = MSG_ROOM_JOIN_NOTIFY + 18;    //!<玩家踢人 大厅发送
        public const ushort MSG_ROOM_PLAYER_KICK_PLAYER_REPLY = MSG_ROOM_JOIN_NOTIFY + 19;    //!<玩家踢人转发 服务器发送
        public const ushort MSG_ROOM_INVITE = MSG_ROOM_JOIN_NOTIFY + 20;    //!<邀请玩家 大厅发送
        public const ushort MSG_ROOM_INVITE_REPLY = MSG_ROOM_JOIN_NOTIFY + 21;    //!<服务器转发邀请请求 服务器发送
        public const ushort MSG_ROOM_SYSTEM_MSG_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 22;    //!<房间内系统消息（临时
        public const ushort MSG_ROOM_GM_COMMAND = MSG_ROOM_JOIN_NOTIFY + 23;    //!<GM命令

        //比赛消息
        public const ushort MSG_ROOM_PK_PLAYER_JOIN = MSG_ROOM_JOIN_NOTIFY + 24;    //!<比赛玩家信息列表@see msg_room_pk_player_join
        public const ushort MSG_ROOM_PK_ROUND_OVER = MSG_ROOM_JOIN_NOTIFY + 25;    //!<回合结束
        public const ushort MSG_ROOM_PK_ELIMINATE = MSG_ROOM_JOIN_NOTIFY + 26;    //!<比赛淘汰信息
        public const ushort MSG_ROOM_UPDATE_BASE_MONEY_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 27;    //!<更新桌子底注
        public const ushort MSG_ROOM_ALLOC_SIT = MSG_ROOM_JOIN_NOTIFY + 28;    //!<系统分桌
        public const ushort MSG_ROOM_ALLOC_SIT_DESK_INFO = MSG_ROOM_JOIN_NOTIFY + 29;    //!<系统分桌桌子信息

        //!<请求匹配
        public const ushort MSG_ROOM_TEAM_CREATE = MSG_ROOM_JOIN_NOTIFY + 30;    //创建队伍
        public const ushort MSG_ROOM_TEAM_CREATE_FAILED = MSG_ROOM_JOIN_NOTIFY + 31;    //创建队伍失败
        public const ushort MSG_ROOM_TEAM_CREATE_NOTIFY = MSG_ROOM_JOIN_NOTIFY + 32;    //创建队伍应答

        public const ushort MSG_ROOM_TEAM_JOIN = MSG_ROOM_JOIN_NOTIFY + 33;    //加入队伍
        public const ushort MSG_ROOM_TEAM_JOIN_FAILED = MSG_ROOM_JOIN_NOTIFY + 34;    //加入队伍失败
        public const ushort MSG_ROOM_TEAM_JOIN_REPLY = MSG_ROOM_JOIN_NOTIFY + 35;    //加入队伍应答
        public const ushort MSG_ROOM_TEAM_DISBAND = MSG_ROOM_JOIN_NOTIFY + 36;    //队伍解散
        public const ushort MSG_ROOM_TEAM_LEAVE = MSG_ROOM_JOIN_NOTIFY + 37;    //离开分组
        public const ushort MSG_ROOM_TEAM_START_QUICKLY = MSG_ROOM_JOIN_NOTIFY + 38;    //快速开始
        public const ushort MSG_ROOM_TEAM_START_QUICKLY_REPLY = MSG_ROOM_JOIN_NOTIFY + 39;    //快速开始应答
        public const ushort MSG_ROOM_PK_UPDATE_PLAYERS = MSG_ROOM_JOIN_NOTIFY + 40;    //更新玩家数量
        public const ushort MSG_ROOM_PK_ADD_GROUP = MSG_ROOM_JOIN_NOTIFY + 41;    //创建分组
        public const ushort MSG_ROOM_PK_REMOVE_GROUP = MSG_ROOM_JOIN_NOTIFY + 42;    //移除分组
        public const ushort MSG_ROOM_PK_UPDATE_GROUP = MSG_ROOM_JOIN_NOTIFY + 43;    //更新分组信息
        public const ushort MSG_ROOM_PK_PLAYER_AWARD = MSG_ROOM_JOIN_NOTIFY + 44;    //比赛奖励
        public const ushort MSG_ROOM_PK_PLAYER_JOIN_REPLY = MSG_ROOM_JOIN_NOTIFY + 45;    //加入比赛结果
        public const ushort MSG_ROOM_SAVE_MONEY = MSG_ROOM_JOIN_NOTIFY + 46;    //存钱
        public const ushort MSG_ROOM_SAVE_MONEY_REPLY = MSG_ROOM_JOIN_NOTIFY + 47;    //存钱应答
        public const ushort MSG_ROOM_PK_GROUP_CHANGE_STATE = MSG_ROOM_JOIN_NOTIFY + 48;    //更改分组状态
                                                                                           //  public const ushort MSG_ROOM_PK_RANK_LIST               = MSG_ROOM_JOIN_NOTIFY + 49;    //比赛排名 2019-11-28 改为使用4179
        public const ushort MSG_ROOM_PK_WAIT_DESK_GAME_OVER = MSG_ROOM_JOIN_NOTIFY + 50;    //!<等待其他比赛桌游戏结束
        public const ushort MSG_ROOM_PK_MESSAGE = MSG_ROOM_JOIN_NOTIFY + 51;    //!<比赛消息通知
        public const ushort MSG_ROOM_JOIN_LIMIT = MSG_ROOM_JOIN_NOTIFY + 52;    //!<不符合进入条件(报名费不够)
        public const ushort MSG_ROOM_USE_PROP_FAILED = MSG_ROOM_JOIN_NOTIFY + 53;    //!<使用道具失败
        public const ushort MSG_ROOM_REPORT = MSG_ROOM_JOIN_NOTIFY + 54;    //!<举报
        public const ushort MSG_ROOM_REPORT_REPLY = MSG_ROOM_JOIN_NOTIFY + 55;    //!<举报应答
        public const ushort MSG_ROOM_TASK_LIST = MSG_ROOM_JOIN_NOTIFY + 56;    //!<任务列表
        public const ushort MSG_ROOM_TASK_COMPLETE = MSG_ROOM_JOIN_NOTIFY + 57;    //!<任务提交
        public const ushort MSG_ROOM_TASK_COMPLETE_REPLY = MSG_ROOM_JOIN_NOTIFY + 58;    //!<任务提交应答
        public const ushort MSG_ROOM_TASK_GIVEUP = MSG_ROOM_JOIN_NOTIFY + 59;    //!<任务放弃
        public const ushort MSG_ROOM_TASK_GIVEUP_REPLY = MSG_ROOM_JOIN_NOTIFY + 60;    //!<任务放弃应答
        public const ushort MSG_ROOM_TASK_GOT_NEW = MSG_ROOM_JOIN_NOTIFY + 61;    //!<发送一个新任务
        public const ushort MSG_ROOM_PK_SCORE_RECLAC = MSG_ROOM_JOIN_NOTIFY + 62;    //!<比赛积分调整（4158
        public const ushort MSG_ROOM_PK_ROOM_INFO_REPLY = MSG_ROOM_JOIN_NOTIFY + 63;    //
        public const ushort MSG_ROOM_PK_ROOM_RANK = MSG_ROOM_JOIN_NOTIFY + 64;    //
        //  public const ushort MSG_ROOM_MIXED_ROOM_INFO            = MSG_ROOM_JOIN_NOTIFY + 65;    //!<混房间房间消息(4161)
        //  public const ushort MSG_ROOM_MIXED_ROOM_INFO_REPLY      = MSG_ROOM_JOIN_NOTIFY + 66;    //!<混房间房间消息回包(4162)
        //  public const ushort MSG_ROOM_MIXED_ROOM_INFO_REQUEST    = MSG_ROOM_JOIN_NOTIFY + 67;    //!<混房间房间消息回包(4163)
        //  public const ushort MSG_ROOM_MIXED_ROOM_RECONNECT       = MSG_ROOM_JOIN_NOTIFY + 68;    //!<混房间断线重连(4164)
        public const ushort MSG_ROOM_PK_ROOM_ERROR = MSG_ROOM_JOIN_NOTIFY + 69;    //!<预报名比赛开赛失败(4165)
        public const ushort MSG_ROOM_TEAM_READY = MSG_ROOM_JOIN_NOTIFY + 70;    //!<组队队员准备(4166)
        public const ushort MSG_ROOM_TEAM_READY_REPLY = MSG_ROOM_JOIN_NOTIFY + 71;    //!<组队队员准备消息回包(4167)
        public const ushort MSG_ROOM_TEAM_START = MSG_ROOM_JOIN_NOTIFY + 72;    //!<组队队员开始匹配消息(4168)
        public const ushort MSG_ROOM_TEAM_START_REPLY = MSG_ROOM_JOIN_NOTIFY + 73;    //!<组队队员开始匹配消息回包(4169)
        public const ushort MSG_ROOM_TEAM_KICK = MSG_ROOM_JOIN_NOTIFY + 74;    //!<组队踢人消息(4170)
        public const ushort MSG_ROOM_TEAM_KICK_REPLY = MSG_ROOM_JOIN_NOTIFY + 75;    //!<组队踢人消息回包(4171)
        public const ushort MSG_ROOM_TEAM_DISMISS = MSG_ROOM_JOIN_NOTIFY + 76;    //!<组队解散消息(4172)
        public const ushort MSG_ROOM_TEAM_DISMISS_REPLY = MSG_ROOM_JOIN_NOTIFY + 77;    //!<组队解散消息回包(4173)
        public const ushort MSG_ROOM_TEAM_DISCONNECT_REPLY = MSG_ROOM_JOIN_NOTIFY + 78;    //!<组队成员离线(4174)
        public const ushort MSG_ROOM_TEAM_CONNECT_REPLY = MSG_ROOM_JOIN_NOTIFY + 79;    //!<组队成员离线(4175)
        public const ushort MSG_ROOM_TEAM_QUIT = MSG_ROOM_JOIN_NOTIFY + 80;    //!<组队成员离开(4176)
        public const ushort MSG_ROOM_TEAM_QUIT_REPLY = MSG_ROOM_JOIN_NOTIFY + 81;    //!<组队成员离开(4177)
        public const ushort MSG_ROOM_TEAM_STANDUP_REPLY = MSG_ROOM_JOIN_NOTIFY + 82;    //!<组队成员起立(4178)
        public const ushort MSG_ROOM_PK_RANK_LIST_EX = MSG_ROOM_JOIN_NOTIFY + 83;    //!<比赛排名(4179)
        public const ushort MSG_ROOM_TEAM_CHANGE_DESK = MSG_ROOM_JOIN_NOTIFY + 85;    //!<组队成员换桌(4181)
        public const ushort MSG_ROOM_TEAM_ROOM_INFO = MSG_ROOM_JOIN_NOTIFY + 86;    //!<组队房间额外信息(4182)
        public const ushort MSG_ROOM_TEAM_STOP_MATCHING = MSG_ROOM_JOIN_NOTIFY + 87;    //!<组队停止匹配(4183)
        public const ushort MSG_ROOM_TEAM_CHANGE_DESK_REPLY = MSG_ROOM_JOIN_NOTIFY + 107;    //!<组队队长换桌应答(4203)
        public const ushort MSG_ROOM_TEAM_NOTIFY_QUIT_MATCH = MSG_ROOM_JOIN_NOTIFY + 108;    //!<2人和多人组队(4204) 提示XXX退出匹配
        public const ushort MSG_ROOM_PK1NEW_RANK_LIST = MSG_ROOM_JOIN_NOTIFY + 112;    //!PK1NEW玩家排名信息(4208)
        public const ushort MSG_ROOM_GAME_PK_USER_DATA_REQUEST = MSG_ROOM_JOIN_NOTIFY + 113;    //!请求PK玩家信息列表(4209)
        public const ushort MSG_ROOM_GAME_PK_USER_DATA_REPLY = MSG_ROOM_JOIN_NOTIFY + 114;    //!请求PK玩家信息列表应答(4210)
        public const ushort MSG_ROOM_PK1NEW_ROOMINFO_REPLY = MSG_ROOM_JOIN_NOTIFY + 115;    //!pk1NEW房间信息(4211)
        public const ushort MSG_ROOM_PK4NEW_PLAYER_STATUS = MSG_ROOM_JOIN_NOTIFY + 116;    //!<PK4NEW玩家轮空通知(4212)
        public const ushort MSG_ROOM_PK4NEW_START_NEXT_STATE = MSG_ROOM_JOIN_NOTIFY + 117;    //!<通知玩家晋级，开启下一个阶段(4213)

        public const ushort MSG_ROOM_REPLACE_SIT = MSG_ROOM_JOIN_NOTIFY + 118;    // 换座位 4214
        public const ushort MSG_ROOM_STAND_UP_EX = MSG_ROOM_JOIN_NOTIFY + 119;    //!<朋友场自由换座 起立 4215
        public const ushort MSG_ROOM_BROAD_WAIT_SIT_LIST = MSG_ROOM_JOIN_NOTIFY + 120;    //!< 朋友场自由换座 广播等待列表(4216)
    }
}

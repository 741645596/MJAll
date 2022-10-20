// @Author: tanjinhua
// @Date: 2021/3/23  10:07


namespace WLHall
{
    public static class HallMsgDefine
    {
        public const ushort MSG_HALL_USER_JOIN = 8192; // 8192
        public const ushort MSG_HALL_USER_JOIN_REPLY = MSG_HALL_USER_JOIN + 1;   //进入大厅应答
        public const ushort MSG_HALL_SYSTEM_MESSAGE = MSG_HALL_USER_JOIN + 2;   //系统消息
        public const ushort MSG_HALL_UPDATE_ROOM_PLAYERS = MSG_HALL_USER_JOIN + 3;   //更新玩家人数
        public const ushort MSG_HALL_ADD_ROOM = MSG_HALL_USER_JOIN + 4;   //新房间加入
        public const ushort MSG_HALL_REMOVE_ROOM = MSG_HALL_USER_JOIN + 5;   //房间移除
        public const ushort MSG_HALL_UPDATE_VERSION = MSG_HALL_USER_JOIN + 6;   //更新版本

        public const ushort MSG_HALL_JOIN_GAME = MSG_HALL_USER_JOIN + 7;   //加入游戏
        public const ushort MSG_HALL_JOIN_GAME_REPLY = MSG_HALL_USER_JOIN + 8;   //加入房间失败
        public const ushort MSG_HALL_LEAVE_GAME = MSG_HALL_USER_JOIN + 9;   //离开游戏
        public const ushort MSG_HALL_LEAVE_GAME_REPLY = MSG_HALL_USER_JOIN + 10;  //离开游戏应答

        public const ushort MSG_HALL_EXCHANGE_MONEY = MSG_HALL_USER_JOIN + 11;  //游戏币兑换,8203
        public const ushort MSG_HALL_EXCHANGE_MONEY_REPLY = MSG_HALL_USER_JOIN + 12;  //游戏币兑换应答,8204
        public const ushort MSG_HALL_CHANGE_USER_INFO = MSG_HALL_USER_JOIN + 13;  //更改用户信息,8205
        public const ushort MSG_HALL_CHANGE_USER_INFO_REPLY = MSG_HALL_USER_JOIN + 14;  //更改用户信息应答,8206

        public const ushort MSG_HALL_USE_PROP = MSG_HALL_USER_JOIN + 15;  //使用道具,8207
        public const ushort MSG_HALL_USE_PROP_REPLY = MSG_HALL_USER_JOIN + 16;  //使用道具应答,8208
        public const ushort MSG_HALL_PROP_LIST = MSG_HALL_USER_JOIN + 17;  //道具列表(服务器发送)        8209
        public const ushort MSG_HALL_MSGS_LIST = MSG_HALL_USER_JOIN + 18;  //*离线消息列表    8210
        public const ushort MSG_HALL_MISSION_LIST = MSG_HALL_USER_JOIN + 19;  //任务列表        8211

        public const ushort MSG_HALL_GET_RANK_LIST = MSG_HALL_USER_JOIN + 20;  //排行榜            8212
        public const ushort MSG_HALL_GET_RANK_LIST_REPLY = MSG_HALL_USER_JOIN + 21;  //排行榜应答        8213

        public const ushort MSG_HALL_MISSION_COMPLETE = MSG_HALL_USER_JOIN + 22;  //任务完成        8214
        public const ushort MSG_HALL_MISSION_COMPLETE_REPLY = MSG_HALL_USER_JOIN + 23;  //任务完成应答    8215

        public const ushort MSG_HALL_USER_REPORT = MSG_HALL_USER_JOIN + 24;  //用户签到        8216
        public const ushort MSG_HALL_USER_REPORT_REPLY = MSG_HALL_USER_JOIN + 25;  //用户签到应答    8217

        public const ushort MSG_HALL_GET_SHOPINFO = MSG_HALL_USER_JOIN + 26;  //获得商店信息    8218
        public const ushort MSG_HALL_GET_SHOPINFO_REPLY = MSG_HALL_USER_JOIN + 27;  //获得商店信息应答8219

        public const ushort MSG_HALL_BUY_PROP = MSG_HALL_USER_JOIN + 28;  //购买道具        8220
        public const ushort MSG_HALL_BUY_PROP_REPLY = MSG_HALL_USER_JOIN + 29;  //购买道具应答    8221

        public const ushort MSG_HALL_LOAD_EFFORT_DATA = MSG_HALL_USER_JOIN + 30;  //获得成就数据    8222
        public const ushort MSG_HALL_LOAD_EFFORT_DATA_REPLY = MSG_HALL_USER_JOIN + 31;  //获得成就数据应答8223

        public const ushort MSG_HALL_CHANGE_PASSWORD = MSG_HALL_USER_JOIN + 32;  //修改密码
        public const ushort MSG_HALL_CHANGE_PASSWORD_REPLY = MSG_HALL_USER_JOIN + 33;  //修改密码应答

        public const ushort MSG_HALL_SET_SECOND_PASSWORD = MSG_HALL_USER_JOIN + 34;  //设置二级密码
        public const ushort MSG_HALL_SET_SECOND_PASSWORD_REPLY = MSG_HALL_USER_JOIN + 35;  //设置二级密码应答

        public const ushort MSG_HALL_BIND_MACHINE = MSG_HALL_USER_JOIN + 36;
        public const ushort MSG_HALL_BIND_MACHINE_REPLY = MSG_HALL_USER_JOIN + 37;

        public const ushort MSG_HALL_MISSION_COMPLETE_LIST = MSG_HALL_USER_JOIN + 38;  //完成任务列表
        public const ushort MSG_HALL_UPDATE_USER_DATA = MSG_HALL_USER_JOIN + 39;  //更新用户数据
        public const ushort MSG_HALL_CHANGE_USER_INFO_MOBILE = MSG_HALL_USER_JOIN + 40;  //移动平台更改用户信息
        public const ushort MSG_HALL_EXCHANGE_LOTTERY = MSG_HALL_USER_JOIN + 41;  //元宝兑换
        public const ushort MSG_HALL_EXCHANGE_LOTTERY_REPLY = MSG_HALL_USER_JOIN + 42;
        public const ushort MSG_HALL_EXCHANGE_PROP_CARD = MSG_HALL_USER_JOIN + 43;  //兑换道具卡片
        public const ushort MSG_HALL_EXCHANGE_PROP_CARD_REPLY = MSG_HALL_USER_JOIN + 44;  //兑换道具卡片应答
        public const ushort MSG_HALL_CHECK_SECOND_PASSWORD = MSG_HALL_USER_JOIN + 45;  //检测二级密码
        public const ushort MSG_HALL_CHECK_SECOND_PASSWORD_REPLY = MSG_HALL_USER_JOIN + 46;  //检测二级密码应答
        public const ushort MSG_HALL_UPDATE_GAME_MISSION_DATA = MSG_HALL_USER_JOIN + 47;  //更新游戏关卡数据
        public const ushort MSG_HALL_GET_GAME_MISSION_LIST = MSG_HALL_USER_JOIN + 48;  //获得游戏关卡任务
        public const ushort MSG_HALL_GET_GAME_MISSION_LIST_REPLY = MSG_HALL_USER_JOIN + 49;  //获得游戏关卡任务应答
        public const ushort MSG_HALL_GAME_MISSION_COMPLETE = MSG_HALL_USER_JOIN + 50;  //游戏任务提交
        public const ushort MSG_HALL_GAME_MISSION_COMPLETE_REPLY = MSG_HALL_USER_JOIN + 51;  //游戏任务提交应答
        public const ushort MSG_HALL_SET_USER_IDCARD = MSG_HALL_USER_JOIN + 52;  //设置身份证信息
        public const ushort MSG_HALL_SET_USER_IDCARD_REPLY = MSG_HALL_USER_JOIN + 53;  //设置身份证应答
        public const ushort MSG_HALL_GET_USER_GAMEDATA = MSG_HALL_USER_JOIN + 54;  //拉取用户游戏数据
        public const ushort MSG_HALL_GET_USER_GAMEDATA_REPLY = MSG_HALL_USER_JOIN + 55;  //拉取用户游戏数据应答
        public const ushort MSG_HALL_LOGOUT = MSG_HALL_USER_JOIN + 56;  //用户登出
        public const ushort MSG_HALL_RELOADGAMES = MSG_HALL_USER_JOIN + 57;  //重新加载游戏信息

        public const ushort MSG_HALL_DATE_CHANGE = 8250;                     //服务器时间改变
        public const ushort MSG_HALL_COMMAND = MSG_HALL_DATE_CHANGE + 1;
        public const ushort MSG_HALL_USER_JOIN_EX = MSG_HALL_DATE_CHANGE + 2; //玩家进入
        public const ushort MSG_HALL_ACTIVE_ACCOUNT = MSG_HALL_DATE_CHANGE + 3; //激活帐号
        public const ushort MSG_HALL_ACTIVE_ACCOUNT_REPLY = MSG_HALL_DATE_CHANGE + 4; //激活帐号应答
        public const ushort MSG_HALL_CHANGE_FACE = MSG_HALL_DATE_CHANGE + 5; //改变头像
        public const ushort MSG_HALL_QUERY_ROOMKEY = MSG_HALL_DATE_CHANGE + 6; //查询房间ID
        public const ushort MSG_HALL_QUERY_ROOMKEY_REPLY = MSG_HALL_DATE_CHANGE + 7; //查询房间ID应答
        public const ushort MSG_HALL_QUERY_USER_DESK = MSG_HALL_DATE_CHANGE + 8; //查询用户房间ID列表
        public const ushort MSG_HALL_QUERY_USER_DESK_REPLY = MSG_HALL_DATE_CHANGE + 9; //查询用户房间ID列表应答
        public const ushort MSG_HALL_CREATE_ROOM = MSG_HALL_DATE_CHANGE + 10; //用户创建房间
        public const ushort MSG_HALL_CREATE_ROOM_REPLY = MSG_HALL_DATE_CHANGE + 11; //用户创建房间应答
        public const ushort MSG_HALL_CREATE_ROOM_FAILED = MSG_HALL_DATE_CHANGE + 12; //创建房间失败
        public const ushort MSG_HALL_DISBAND_ROOM = MSG_HALL_DATE_CHANGE + 13; //用户解散房间
        public const ushort MSG_HALL_DISBAND_ROOM_REPLY = MSG_HALL_DATE_CHANGE + 14; //用户解散房间应答
        public const ushort MSG_HALL_QUERY_DESKINFO = MSG_HALL_DATE_CHANGE + 15; //查询桌子状态
        public const ushort MSG_HALL_QUERY_DESKINFO_REPLY = MSG_HALL_DATE_CHANGE + 16; //查询桌子状态应答
        public const ushort MSG_HALL_KICK_PLAYER_BY_OWNER = MSG_HALL_DATE_CHANGE + 17; //踢出玩家(代开桌子上)8267
        public const ushort MSG_HALL_KICK_PLAYER_BY_OWNER_REPLY = MSG_HALL_DATE_CHANGE + 18; //踢出玩家应答(代开桌子上)8268
        public const ushort MSG_HALL_UPDATE_EFFORTDATA = MSG_HALL_DATE_CHANGE + 22; //更新成就数据
        public const ushort MSG_HALL_CLIENT2ROOM_PIPE = MSG_HALL_DATE_CHANGE + 23; //客户端到房间管道
        public const ushort MSG_HALL_ROOM2CLIENT_PIPE = MSG_HALL_DATE_CHANGE + 24; //房间到客户端管道
        public const ushort MSG_HALL_UPLOAD_AVATAR = MSG_HALL_DATE_CHANGE + 25; //上传头像
        public const ushort MSG_HALL_UPLOAD_AVATAR_DATA = MSG_HALL_DATE_CHANGE + 26; //上传头像数据
        public const ushort MSG_HALL_MISSION_COMPLETE2 = MSG_HALL_DATE_CHANGE + 27; //任务完成(扩展)
        public const ushort MSG_HALL_MISSION_COMPLETE_REPLY2 = MSG_HALL_DATE_CHANGE + 28; //任务完成应答

        public const ushort MSG_HALL_LOCK_ROOM = MSG_HALL_DATE_CHANGE + 29; //房间锁定状态更新

        public const ushort MSG_HALL_QUERY_GROUP_DESK = MSG_HALL_DATE_CHANGE + 30; //--//!<查询朋友场组桌子(8280)
        public const ushort MSG_HALL_QUERY_GROUP_DESK_REPLY = MSG_HALL_DATE_CHANGE + 31; //--//!<查询朋友场组桌子应答（8281）

        public const ushort MSG_HALL_QUERY_CONFIG_INFO = MSG_HALL_DATE_CHANGE + 33; // 查询预报名配置信息
        public const ushort MSG_HALL_QUERY_CONFIG_INFO_REPLY = MSG_HALL_DATE_CHANGE + 34; // 查询预报名配置信息应答
        public const ushort MSG_HALL_SIGNUP = MSG_HALL_DATE_CHANGE + 35; // 预报名消息
        public const ushort MSG_HALL_UNSIGNUP = MSG_HALL_DATE_CHANGE + 36; // 取消预报名
        public const ushort MSG_HALL_SIGN_UP_MATCH_REPLY = MSG_HALL_DATE_CHANGE + 37; // 预报名应答
        public const ushort MSG_HALL_SIGN_UP_START_REPLY = MSG_HALL_DATE_CHANGE + 38; // 预报名比赛开始通知
        public const ushort MSG_HALL_SIGN_UP_PLAYERS_UPDATE = MSG_HALL_DATE_CHANGE + 39; // 报名人数变化通知
        public const ushort MSG_HALL_TO_CLIENT_ROOMCANIN = MSG_HALL_DATE_CHANGE + 40; // 提前进入比赛房间通知
        public const ushort MSG_HALL_TO_CLIENT_PK_ERR = MSG_HALL_DATE_CHANGE + 41; // 比赛开赛失败通知
        public const ushort MSG_HALL_CLIENT_GET_MONEY_ROOMID = MSG_HALL_DATE_CHANGE + 42; // 客户端请求玩家是否在普通场
        public const ushort MSG_HALL_NOTIFY_CLIENT_MONEY_ROOMID = MSG_HALL_DATE_CHANGE + 43; // 通知客户端玩家是否在普通场
        public const ushort MSG_CLIENT_TO_HALL_GET_SIGNUP_COUNT = MSG_HALL_DATE_CHANGE + 44; // 拉取比赛人数信息--8294
        public const ushort MSG_HALL_TO_CLIENT_SYN_SIGNUP_COUNT = MSG_HALL_DATE_CHANGE + 45; // 拉取比赛人数信息应答--8295
        public const ushort MSG_HALL_GET_IMG_INFO = MSG_HALL_DATE_CHANGE + 46; // 客户端请求头像等信息--8296
        public const ushort MSG_HALL_GET_IMG_INFO_REPLY = MSG_HALL_DATE_CHANGE + 47; // 客户端请求头像等信息应答--8297
        public const ushort MSG_FRIEND_PLAYER_JOIN = MSG_HALL_DATE_CHANGE + 48; // 客户端上报亲友圈信息--8298
        public const ushort MSG_FRIEND_PLAYER_JOIN_REPLY = MSG_HALL_DATE_CHANGE + 49; // 客户端上报亲友圈信息应答--8299
        public const ushort MSG_FRIEND_PLAYER_LEAVE = MSG_HALL_DATE_CHANGE + 50; // 离开亲友圈--8300
        public const ushort MSG_FRIEND_PLAYER_RECIVE_STATE = MSG_HALL_DATE_CHANGE + 51; // 设置是否接收亲友圈信息--8301 true时回8003
        public const ushort MSG_FRIEND_PLAYER_TO_PLAYER = MSG_HALL_DATE_CHANGE + 52; // 客户端指定发送消息给亲友圈某个玩家--8302

        public const ushort MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY = 8000;                     //!<亲友圈广播通知消息
        public const ushort MSG_FRIEND_GROUP_LEAVE_PLAYER_NOTIFY = MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY + 1; //亲友圈离开玩家广播  8001
        public const ushort MSG_FRIEND_GROUP_OP_TAGS = MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY + 2; //亲友圈增加/移除tag(房间发给亲友圈服务器) 8002
        public const ushort MSG_FRIEND_GROUP_FLUSH_DATA = MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY + 3; //推送亲友圈全部数据 8003
        public const ushort MSG_FRIEND_GROUP_P2P = MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY + 5; //推送亲友圈全部数据 8005
        public const ushort MSG_FRIEND_GROUP_BROAD = MSG_FRIEND_GROUP_JOIN_PLAYER_NOTIFY + 6; //推送亲友圈全部数据 8006






        public const ushort MSG_CL_TO_HALL_GET_OFFLINE_CMP_INFO = MSG_HALL_DATE_CHANGE + 53; // 客户端请求线下比赛信息  8303
        public const ushort MSG_HALL_TO_CL_GET_OFFLINE_CMP_INFO = MSG_HALL_DATE_CHANGE + 54; // 客户端请求线下比赛信息应答 8304

        public const ushort MSG_CLIENT_HALL_CHANGE_NICKNAME = MSG_HALL_DATE_CHANGE + 55; // 客户端请求修改昵称 8305
        public const ushort MSG_CLIENT_HALL_CHANGE_SEX = MSG_HALL_DATE_CHANGE + 56; // 客户端请求修改性别 8306
        public const ushort MSG_CLIENT_HALL_SET_CITY = MSG_HALL_DATE_CHANGE + 57; // 客户端上传地区码 8307
        public const ushort MSG_HALL_CLIENT_CHANGE_USERINFO_REPLY = MSG_HALL_DATE_CHANGE + 58; // 修改用户信息应答 8308
        public const ushort MSG_CLIENT_HALL_CHANGE_PASSWORD = MSG_HALL_DATE_CHANGE + 59; // 客户端请求修改密码 8309
        public const ushort MSG_CLIENT_HALL_CHANGE_PASSWORD_REPLY = MSG_HALL_DATE_CHANGE + 60; // 客户端请求修改密码应答 8310
        public const ushort MSG_CLIENT_HALL_CERTIFICATE = MSG_HALL_DATE_CHANGE + 61; // 客户端实名认证 8311
        public const ushort MSG_CLIENT_HALL_BIND_PHONE = MSG_HALL_DATE_CHANGE + 62; // 绑定手机 8312
        public const ushort MSG_CLIENT_HALL_UNBIND_PHONE = MSG_HALL_DATE_CHANGE + 63; // 解绑手机 8313
        public const ushort MSG_CLIENT_HALL_BIND_OR_UNBIND_PHONE_REPLY = MSG_HALL_DATE_CHANGE + 64; // 绑定或者解绑手机应答 8314
        public const ushort MSG_CLIENT_HALL_TO_WEB = MSG_HALL_DATE_CHANGE + 65; // 通过大厅请求 Web 8315
        public const ushort MSG_CLIENT_HALL_TO_WEB_REPLY = MSG_HALL_DATE_CHANGE + 66; // 通过大厅请求 Web应答 8316
        public const ushort MSG_CLIENT_HALL_EXCHANGE_MONEY = MSG_HALL_DATE_CHANGE + 67; // 客户端即时红包兑换豆豆 8317
        public const ushort MSG_CLIENT_HALL_EXCHANGE_MONEY_REPLY = MSG_HALL_DATE_CHANGE + 68; // 客户端兑即时红包兑换豆豆应答 8318
        public const ushort MSG_CLIENT_HALL_GET_CAPTCHA = MSG_HALL_DATE_CHANGE + 69; // 获取验证码 8319
        public const ushort MSG_CLIENT_HALL_GET_CAPTCHA_REPLY = MSG_HALL_DATE_CHANGE + 70; // 获取验证码应答 8320
        public const ushort MSG_CLIENT_HALL_DEPUTYACCOUNT = MSG_HALL_DATE_CHANGE + 75; // 辅助账号 绑定 与 解绑 8325
        public const ushort MSG_CLIENT_HALL_BIND_ACCOUNT_REPLY = MSG_HALL_DATE_CHANGE + 76; // 账号激活/辅助账号 绑定 与 解绑 应答 8326
        public const ushort MSG_CLIENT_HALL_ACTIVATE_PHONE = MSG_HALL_DATE_CHANGE + 77; // 账号激活(手机激活) 8327
        public const ushort MSG_CLIENT_HALL_SET_ACCOUNT_BY_CHANGE_PHONE = MSG_HALL_DATE_CHANGE + 78; // 辅助账号更换手机号(只限账号登录) 8328



        public const ushort MSG_FRIEND_GROUP_CLIENT_FLUSH_GROUP_DATA = 8007; // 推送亲友圈配置相关的全部数据 8007
        public const ushort MSG_FRIEND_GROUP_CLIENT_GROUP_TAG_NOTIFY = 8008; // 亲友圈(圈内标签)配置修改广播 8008
        public const ushort MSG_FRIEND_GROUP_GET_GROUP_TAGS_INFO_REPLY = 8009; // 客户端获取指定类型的标签应答 8009
        public const ushort MSG_FRIEND_GROUP_FLUSH_PROP_UPDATE = 8016; // 通知客户端道具数量变更
        public const ushort MSG_FRIEND_GROUP_PLAYER_RECHARGE_REPLY = 8017; // 亲友圈充值道具应答
        public const ushort MGS_FRIEND_GROUP_PLAYER_DISBAND_REPLY = 8019; // 解散结果，返回给客户端
        public const ushort MSG_FRINED_GAME_GROUP_SET_REPLY = 8020; // 亲友圈设置应答
        public const ushort MSG_FRIEND_GAME_GROUP_CHANGE_ROOM_NAME = 8021; // 更改包房名字应答
        public const ushort MSG_FRIEND_GROUP_GAME_TAGS_REPLY = 8027; // 客户端获取亲友圈某个楼层游戏房间数量应答
        public const ushort MSG_FRIEND_GROUP_UPDATE_GAME_RULE_TAGS = 8028; // 2020-12-10 原8026消息替换为8028，亲友圈游戏规则更新,发给玩家(黑名单，不能同桌列表，等)

        public const ushort MSG_FRIEND_GROUP_CLIENT_OP_GROUP_TAGS = MSG_HALL_DATE_CHANGE + 80; // 客户端增加/修改/移除亲友圈标签 8330
        public const ushort MGS_FRIEND_GET_CLUB_INFO = 8331; //!<客户端获取亲友圈/大联盟信息--lua中实现
        public const ushort MGS_FRIEND_GET_CLUB_INFO_REPLY = 8332; //!<客户端获取亲友圈/大联盟信息应答--lua中实现
        public const ushort MGS_FRIEND_GET_GROUP_TAGS = 8333; //客户端获取标签
        public const ushort MGS_FRIEND_GROUP_PLAYER_RECHARGE = 8334; //客户端给亲友圈充值道具(房卡)
        public const ushort MGS_FRIEND_GROUP_PLAYER_RECHARGE_REPLY = 8335; //客户端给亲友圈充值道具(房卡)失败应答
        public const ushort MSG_FRIEND_CHECK_CLUB = 8336; //检查玩家是否有亲友圈和大联盟
        public const ushort MSG_FRIEND_CHECK_CLUB_REPLY = 8337; //检查玩家是否有亲友圈和大联盟应答
        public const ushort MGS_FRIEND_GROUP_PLAYER_DISBAND = 8338; //客户端解散亲友圈
        public const ushort MSG_FRIEND_GROUP_SET = 8339; //亲友圈设置
        public const ushort MSG_FRIEND_GROUP_CHANGE_ROOM_NAME = 8340; //更改包房名字
        public const ushort MSG_FRIEND_GROUP_VERSION_LIST = 8342; //下发游戏版本列表（亲友圈删除旧标签使用）
        public const ushort MGS_FRIEND_GET_CLUB_INFO_EX = 8343; // 客户端获取亲友圈/大联盟信息EX
        public const ushort MGS_FRIEND_GET_CLUB_INFO_REPLY_EX = 8344; // 客户端获取亲友圈/大联盟信息应答EX
        public const ushort MGS_FRIEND_GET_GAME_TAGS = 8345; // 客户端获取某个亲友圈对应楼层的桌子数量
        public const ushort MGS_FRIEND_CHANGE_GAME_RULE_TAGS = 8346; // 客户端变更需同步给游戏服务器的标签数据，广播时由 8026 返回数据
        public const ushort MGS_FRIEND_GET_GAME_RULE_TAGS = 8347; // 客户端获取需同步给游戏服务器的标签数据，由 8026 返回数据
        public const ushort MSG_FRIEND_GROUP_CHANGE_ROOM_NAME_REPLY = 8348; //更改包房名字应答
        public const ushort MSG_FRIEND_GET_CLUB_POWER = 8349; //获取自己所在圈自己的权限
        public const ushort MSG_FRIEND_GET_CLUB_POWER_REPLY = 8350; //获取自己所在圈自己的权限应答

        public const ushort MSG_HALL_WEB_REQUEST = 8353; /// web请求
        public const ushort MSG_HALL_WEB_REPLY = 8354; /// web应答

        public const ushort MSG_CLIENT_HALL_TRANSFER_SEND_TO_PLAYER = 8371; //web服务器发送数据给玩家

        public const ushort MSG_HALL_TASK_MISSION_LIST = MSG_HALL_DATE_CHANGE + 150; // 任务列表8400
        public const ushort MSG_HALL_TASK_MISSION_COMPLETE = MSG_HALL_DATE_CHANGE + 151; // 任务完成8401
        public const ushort MSG_HALL_TASK_MISSION_COMPLETE_REPLY = MSG_HALL_DATE_CHANGE + 152; // 任务完成应答8402
        public const ushort MSG_HALL_UPDATE_WX_DATA = MSG_HALL_DATE_CHANGE + 153;
        // 同步微信用户信息 8403

        /*====================== 段位赛消息宏定义开始 ======================*/

        // 请求段位赛基础信息
        public const ushort MSG_RANK_PLAYER_GET_RANK_INFO = 8363;

        // 请求段位赛基础信息应答
        public const ushort MSG_RANK_PLAYER_GET_RANK_INFO_REPLY = 8051;

        // 请求段位奖励配置及个人奖励领取状态
        public const ushort MSG_RANK_PLAYER_GET_PLAYER_LEVEL = 8364;

        // 请求段位奖励配置及个人奖励领取状态应答
        public const ushort MSG_RANK_PLAYER_GET_PLAYER_LEVEL_REPLY = 8054;

        // 领取段位奖励
        public const ushort MSG_RANK_PLAYER_REWARD = 8362;

        // 领取段位奖励应答
        public const ushort MSG_RANK_PLAYER_REWARD_REPLY = 8056;

        // 请求单个玩家个人段位信息
        public const ushort MSG_RANK_PLAYER_GET_PLAYER_INFO = 8365;

        // 请求单个玩家个人段位信息应答
        public const ushort MSG_RANK_PLAYER_GET_PLAYER_INFO_REPLY = 8055;

        // 玩家星位变动通知
        public const ushort MSG_RANK_PLAYER_GET_PLAYER_STAR_CHANGE = 8057;

        // 分数变动通知
        public const ushort MSG_RANK_SEND_PLAYER_SCORE_CHANGE = 8058;

        // 玩家确认已继承当前赛季 --TODO

        // 获取玩家段位其他信息
        public const ushort MSG_RANK_PLAYER_GET_PARAM = 8366;

        // 获取玩家段位其他信息应答
        public const ushort MSG_GAME_RANK_PLAYER_GET_PARAM_REPLY = 8061;

        // 获取玩家段位档案信息
        public const ushort MSG_GAME_RANK_GET_RANK_HISTORY = 8367;

        // 获取玩家段位档案信息应答
        public const ushort MSG_GAME_RANK_GET_RANK_HISTORY_REPLY = 8063;

        // 获取赛季奖励
        public const ushort MSG_RANK_PLAYER_GET_SEASON_REWARD = 8368;

        // 获取赛季奖励应答
        public const ushort MSG_RANK_PLAYER_GET_SEASON_REWARD_REPLY = 8064;

        #region 换装
        //获取平台时装列表(*)
        public const ushort MSG_FASHION_GET_FASHION_LIST = 8404;

        //获取玩家时装列表(*
        public const ushort MSG_FASHION_GET_PLAYER_FASHION_LIST = 8405;

        //获取玩家当前穿戴的时装列表(*)
        public const ushort MSG_FASHION_GET_PLAYER_FASHION_CUR_WEAR = 8406;

        //设置玩家当前穿戴的时装列表(*)
        public const ushort MSG_FASHION_SET_PLAYER_FASHION_CUR_WEAR = 8407;

        //执行失败时，返回给请求失败的应答消息
        public const ushort MSG_HALL_FASHION_PLAYER_FAILED_REPLY = 8409;

        //获取平台时装列表应答(*)
        public const ushort MSG_FASHION_GET_FASHION_LIST_REPLY = 8410;

        //获取玩家时装列表应答
        public const ushort MSG_FASHION_GET_PLAYER_FASHION_LIST_REPLY = 8411;

        //获取玩家当前穿着的时装应答
        public const ushort MSG_FASHION_GET_PLAYER_FASHION_CUR_WEAR_REPLY = 8412;

        //设置玩家当前穿着的时装应答
        public const ushort MSG_FASHION_SET_PLAYER_FASHION_CUR_WEAR_REPLY = 8413;
        #endregion
    }
}

// @Author: tanjinhua
// @Date: 2021/3/19  20:07


using WLCore;

namespace WLHall
{
    public static class Constants
    {

        public static readonly string VER_MODULE_GAME_COMMON = "Common";
        public static readonly string VER_MODULE_MJ_COMMON = "MJCommon";
        public static readonly string VER_MODULE_PK_COMMON = "PKCommon";


        public const int LCVR_OK = 0;
        public const int LCVR_WARNING_NEW_VERSION = 1;
        public const int LCVR_ERROR_NOT_SUPPORT = 2;


        public const int DEVICE_CODE_LEN = 42;
        public const int MD5_PSW_LEN = 16;
        public const int MACADDRESS_LEN = 6;
        public const int GUID_LEN = 16;
        public const int GAME_EXT_DATA_SIZE = 16;
        public const int GAME_MSG_HEADER_SIZE = 24;
        public const int PACKET_STRUCT_SIZE = 28;

        public const byte CRYPT_MSG = MsgHeader.Flag.Compress | MsgHeader.Flag.Encode | MsgHeader.Flag.Mask;
        
        public const int MSG_KEEPALIVE = 0xFFFF;                    //!<心跳机制
        public const int MSG_PACKET_SIZE = 6000;                    //!<消息包大小
        public const int MSG_SUIT_SIZE = 8000;                      //!<消息适当大小

        #region MatchRelative
        //比赛阶段信息
        public const ushort PK_GROUP_STATE_WAITING = 0;             //等待状态(时间没到)
        public const ushort PK_GROUP_STATE_JOIN = 1;                //加入状态
        public const ushort PK_GROUP_STATE_STEP = 2;                //比赛状态阶段1,大于2的阶段都为比赛状态
        #endregion

        #region RoomTypeRelative
        // ROOM_TYPE
        public const uint ROOM_TYPE_MONEY = 0x0001;                 //!<游戏币房间
        public const uint ROOM_TYPE_TEMPLATE = 0X0002;              //!<临时货币房间，进入房间分配金币，退出回收，不影响身上货币
        public const uint ROOM_TYPE_TEMPSCORE = 0x0004;             //!<临时积分，进入房间分配固定积分
        public const uint ROOM_TYPE_RATETAX = 0x0008;               //!<可变税率，房间的tax属性为百分比。如果没有此属性，则房间的tax属性为固定扣税值
        public const uint ROOM_TYPE_HIDDEN = 0x0010;                //!<房间内玩家信息不可见，并且无法在房间和游戏中聊天。

        public const uint ROOM_TYPE_BASEMONEY = 0X0020;             //!<基础底注房间，房间的BaseMoney属性有效。没有该属性，则房间内允许每桌不同底注
        public const uint ROOM_TYPE_NO_PROP = 0x0040;               //!<房间内禁止使用道具
        public const uint ROOM_TYPE_VIP = 0x0080;                   //!<房间为VIP房间，非VIP用户无法进入
        public const uint ROOM_TYPE_VIDEO = 0x0100;                 //!<房间为视频房间。
        public const uint ROOM_TYPE_TEST = 0X0200;                  //!<房间为测试房间，所有输赢属性均不计算
        public const uint ROOM_TYPE_GAMING_JOIN = 0x0400;           //!<房间类型为游戏开始后仍可进入类型
        public const uint ROOM_TYPE_UPGRADE = 0x0800;               //!<闯关场
        public const uint ROOM_TYPE_DESKPK = 0x1000;                //!<单桌比赛
        public const uint ROOM_TYPE_GIFTMONEY = 0x2000;             //!<送豆场
        public const uint ROOM_TYPE_UPGRADE2 = 0X4000;              //!<闯关场（分场闯关)
        public const uint ROOM_TYPE_LOCKED = 0x10000;               //!<房间锁定标记)

        //房间等级类型，共4种,位数22-24位
        public const uint ROOM_TYPE_LEVEL_MASK = 0x00C00000;        //!<房间类型掩码
        public const uint ROOM_TYPE_LEVEL_0 = 0;                    //!<新手房间
        public const uint ROOM_TYPE_LEVEL_1 = 1;                    //!<普通房间
        public const uint ROOM_TYPE_LEVEL_2 = 2;                    //!<精英房间
        public const uint ROOM_TYPE_LEVEL_3 = 3;                    //!<土豪房间
        public const uint ROOM_TYPE_LEVEL_4 = 4;                    //!<至尊房间

        //落座模式，共15种，位数24-28位
        public const int ROOM_TYPE_SIT_MODE_MASK = 0x0F000000;      //!<落座模式掩码（共16种）
        public const int ROOM_TYPE_FREE_MODE = 0 << 24;             //!<自由落座模式
        public const int ROOM_TYPE_ALLOCSIT = 1 << 24;              //!<房间内玩家座位由系统分配，无法自己选择座位
        public const int ROOM_TYPE_ALLOCSIT2 = 2 << 24;             //!<系统分桌模式，随机分配位置
        public const int ROOM_TYPE_TEAM = 3 << 24;                  //!<组队模式房间
        public const int ROOM_TYPE_FRIEND_TEAM = 4 << 24;           //!<朋友场模式房间
        public const int ROOM_TYPE_TEAM_MULTI = 5 << 24;            //!<多人组队模式房间
        public const int ROOM_TYPE_PK_MODE1 = 8 << 24;              //!<比赛模式1
        public const int ROOM_TYPE_PK_MODE2 = 9 << 24;              //!<比赛模式2
        public const int ROOM_TYPE_PK_MODE3 = 10 << 24;             //!<比赛模式3
        public const int ROOM_TYPE_PK_MODE4 = 0xb000000;            //!<比赛模式4
        public const int ROOM_TYPE_PK_MODE5 = 12 << 24;             //!<比赛模式5 新版福利赛在使用

        //开发者自定义,取值范围0~15（或四种组合),位数28-32位
        public const uint ROOM_TYPE_CUSTOM_MASK = 0xF0000000;       //自定义类型掩码
        //给与游戏开发者4种自定义房间类型
        public const uint ROOM_TYPE_CUSTOM1 = 0x10000000;
        public const uint ROOM_TYPE_CUSTOM2 = 0x20000000;
        public const uint ROOM_TYPE_CUSTOM3 = 0x40000000;
        public const uint ROOM_TYPE_CUSTOM4 = 0x80000000;
        #endregion

        #region UserFromRelative
        public const int USER_FROM_UNKOWN = 0;                      //!<未知来源
        public const int USER_FROM_UNLOGIN = 1;                     //!<游客身份
        public const int USER_FROM_PLATFORM = 2;                    //!<帐号来自当前平台
        public const int USER_FROM_TENCENT = 3;                     //!<帐号来自腾讯
        public const int USER_FROM_WEIBO = 4;                       //!<帐号来自新浪微博
        public const int USER_FROM_JIXIANG = 7;                     //!<帐号来自吉祥
        public const int USER_FROM_YSDK_QQ = 8;                     //!<帐号来自应用宝QQ
        public const int USER_FROM_YSDK_WX = 9;                     //!<帐号来自应用宝微信
        public const int USER_FROM_QIHOO = 10;                      //!<账号来自奇虎360
        public const int USER_FROM_OPPO = 11;                       //!<账号来自 OPPO
        public const int USER_FROM_HUAWEI = 12;                     //!<账号来自华为
        public const int USER_FROM_XIAOMI = 13;                     //!<账号来自小米
        public const int USER_FROM_TOUTIAO = 14;                    //!<今日头条渠道用户
        public const int USER_FROM_VIVO = 15;                       //!<VIVO渠道用户
        public const int USER_FROM_SAMSUNG = 16;                    //!<三星渠道用户
        public const int USER_FROM_MEIZU = 17;                      //!<魅族渠道用户
        public const int USER_FROM_MOSTONE = 21;                    //!<默往渠道用户
        public const int USER_FROM_BAIDU = 22;                      //!<百度渠道用户
        public const int USER_FROM_DOUYIN = 24;                     //!<抖音渠道用户
        public const int USER_FROM_WECHAT_JIXIANG = 35;             //!<吉祥 心悦微信账号平台
        public const int USER_FROM_WECHAT_WEILE = 36;               //!微乐微信账号平台 正常微乐平台36 避免微乐代码写死出现问题吉祥改成35
        public const int USER_FROM_QQMINI = 80;                     //!账号来自手机QQ小游戏
        public const int USER_FROM_BYTEDANCE = 112;                 //!账号来自抖音小游戏
        #endregion

        public const int MSG_OFFSET_BASE_LOGIC = 0;                 //!<基础逻辑消息
        public const int MSG_OFFSET_LOBBY_TO_GAMESERVER = 1;        //!<游戏逻辑服务器到客户端大厅
        public const int MSG_OFFSET_LOBBY_TO_HALLSERVER = 2;        //!<大厅到大厅服务器
        public const int MSG_OFFSET_GAMESERVER_TO_HALLSERVER = 3;   //!<游戏服务器到大厅服务器
        public const int MSG_OFFSET_LOGIC_TO_CENTER = 4;            //!<逻辑服务器到中央服务器
        public const int MSG_OFFSET_ROUTE = 5;                      //!<中转消息
        public const int MSG_OFFSET_CUSTOM = 6;                     //!<或者异步调用自己,管道消息，进程之间通讯
        public const int MSG_OFFSET_COMMON = 7;                     //!<通用

        public const int USER_TYPE_ROBOT = 0;                       //!<机器人
        public const int USER_TYPE_NORMAL = 1;                      //!<普通用户
        public const int USER_TYPE_VIP = 2;                         //!<会员
        public const int USER_TYPE_GM = 250;                        //!<2B的管理员
        public const int USER_TYPE_SUPER_GM = 251;                  //!<超级管理员

        // 游戏桌子状态
        public const int DESK_STATE_FREE = 0;                       //!<桌子空闲状态
        public const int DESK_STATE_GAMING = 1;                     //!<桌子游戏状态
        public const int DESK_STATE_LOCKED = 2;                     //!<桌子上锁状态
        public const ushort INVALID_DESK = 0XFFFF;                  //无效的桌子ID
        public const ushort INVALID_CHAIR = 0xFFFF;                 //无效的椅子ID

        public const ushort MSG_MAX_SIZE = 8192;
        public const int USER_NAME_LEN = 36;                        //!<用户名长度
        public const int USER_EXT_COUNT = 24;                       //!<扩展属性长度
        public const int PASS_WORD_LEN = 20;                        //!<普通密码长度
        public const int GAME_SHORT_NAME_LEN = 5;                   //!<游戏缩写名长度
        public const int GAME_SHELL_FILE_LEN = 32;                  //!<游戏可执行文件名长度
        public const int COMMAND_LEN = 260;                         //!<服务器附加命令长度
        public const int AVATAR_IMG_PATH_LEN = 128;                 //!<头像地址长度
        public const int ROOM_NAME_LEN = 40;                        //!<房间名长度
        public const int EFFORT_DATA_LEN = 4000;                    //!<任务数据最大长度
        public const int EFFORT_DATA_COUNT = EFFORT_DATA_LEN / 8;   //!<任务总数据量
        public const int ID_CARD_LEN = 18;                          //!<身份证号
        public const int ROOM_MSG_LEN = 1024;                       //!<房间消息长度
        public const int MAX_USER_NAME_LEN = 36;                    //!<实际用户名最大长度
        public const int MISSION_CONTEXT_LEN = 128;                 //!<任务描述长度
        public const int GAME_EXTEND_DATA_LEN = 128;                //!<游戏附加数据

        public const uint RELATION_TYPE_ROOT = 0;                   // 顶级节点
        public const uint RELATION_TYPE_GAME = 1;                   // 游戏节点
        public const uint RELATION_TYPE_ROOM = 2;                   // 房间节点
        public const uint RELATION_TYPE_RELATION = 3;               // 子节点

        #region LoginManagerRelative
        public const int LOGIN_TYPE_BY_NAME = 1;                    // 用户名登录
        public const int LOGIN_TYPE_BY_UNNAME = 2;                  //!<匿名登陆
        public const int LOGIN_TYPE_GET_ROLE_LIST = 3;              //!<拉取用户列表
        public const int LOGIN_TYPE_ALLOC_USER = 4;                 //!<分配新帐号
        public const int CLIENT_TYPE_UNKOWN = 0x0;                  //!<未知类型
        public const int CLIENT_TYPE_PC = 0x1;                      //!<PC登陆(传统登陆
        public const int CLIENT_TYPE_WEB = 0x2;                     //!<网页登陆
        public const int CLIENT_TYPE_ANDROID = 0x4;                 //!<安卓登陆
        public const int CLIENT_TYPE_ANDROID_PAD = 0x8;             //!<安卓平板
        public const int CLIENT_TYPE_ANDROID_TV = 0x10;             //!<安卓电视
        public const int CLIENT_TYPE_IPHONE = 0x20;                 //!<iphone登陆
        public const int CLIENT_TYPE_IPAD = 0x40;                   //!<IPAD登陆
        public const int CLIENT_TYPE_ITV = 0x80;                    //!<IOS系统电视

        public const int CLIENT_TYPE_SUPPORT_JIXIANG = 1 << 14;     //!支持吉祥平台
        public const int CLIENT_TYPE_SUPPORT_WEILE = 1 << 15;       //!支持微乐平台
        #endregion

        #region MissionRelative
        public const int MISSION_DAILY_GIFT_MONEY = 1;              // 任务ID,每人赠送豆
        public const int MISSION_DAILY_GIFT_XZMONEY = 3;            // 申请钻石救济金的任务 ID。
        public const int MISSION_EFFORT_GIFT_TJZMMONEY = 7;         // 添加桌面
        public const int MISSION_DAILY_GIFT_MIXADMONEY = 8;         // 免费豆里面的看广告的次数小金额任务
        public const int MISSION_DAILY_GIFT_MAXADMONEY = 9;         // 免费豆里面的看广告的次数大金额任务
        public const int MISSION_EFFORT_GIFT_TJYLMONEY = 10;        // 添加有礼(浮窗)
        public const int MISSION_EFFORT_GIFT_MEMORYMONEY = 20;      // 打开存储权限奖励
        public const int MISSION_EFFORT_NEWBIE_MONEY_APP = 21;      // 新手豆豆奖励（APP)
        public const int MISSION_EFFORT_NEWBIE_OTHER_APP = 22;      // 新手其他奖励（APP）
        public const int MISSION_EFFORT_NEWBIE_OTHER_MINI = 23;     // 新手其他奖励（非APP）
        public const int MISSION_DAILY_MATCH_FREE_COUNT = 24;       // 锦标赛免费次数的任务ID, 此次数每日重置
        public const int MISSION_EFFORT_LOGIN_MINI = 25;            // 小游戏登录成就
        public const int MISSION_EFFORT_LOGIN_APP = 26;             // APP登录成就
        public const int MISSION_DAILY_GIFT_SIGNIN = 27;            // 普通签到
        public const int MISSION_DAILY_GIFT_SIGNIN_SUBS = 28;       // 订阅签到
        public const int MISSION_EFFORT_NEWBIE_MONEY_MINI = 29;     // 新手豆豆奖励(非APP)
        public const int MISSION_DAILY_GIFT_MONEY_NO_DOUBLE = 30;   // 任务ID,每人赠送豆(领取后不提示多倍奖励)
        public const int MISSION_VIVO_PRIVILEGE = 31;               // 任务ID,VIVO游戏中心特权奖励
        public const int MISSION_NO_SHUFFLE = 33;                   // 任务ID,是否触发无需做牌 (触发一次后，以后就不用做牌)
        public const int MISSION_SERVICE_COIN = 34;                 // 每日任务 客服领豆
        public const int MISSION_FAVORITE_COIN = 35;                // 收藏送豆，终身一次，添加到我的小程序
        ///任务类型
        public const int MISSION_TYPE_EFFORT = 0;                   //!<成就任务
        public const int MISSION_TYPE_DAILY = 1;                    //!<日常任务
        public const int MISSION_TYPE_TEMP = 2;                     //!<临时任务
        public const int MISSION_TYPE_RANDOM = 3;                   //!<随机任务
        #endregion


        // 日常数据索引
        public const int DAILY_DATA_LOGIN_COUNT = 0;                //!<登陆次数
        public const int DAILY_DATA_ONLINE_TIME = 1;                //!<在线时长
        public const int DAILY_DATA_GAME_COUNT = 2;                 //!<游戏局数
        public const int DAILY_DATA_GAME_TIME_EX = 3;               //游戏时间
        public const int DAILY_DATA_GIFT_COUNT = 4;                 //!<领奖次数
        public const int DAILY_DATA_WIN_MONEY = 5;                  //!<赢的钱数
        public const int DAILY_DATA_LOST_MONEY = 6;                 //!<输的钱数
        public const int DAILY_DATA_REPORT = 7;                     //!<签到
        public const int DAILY_DATA_PAY_MONEY = 8;                  //!<充值（卡面值）总数
        public const int DAILY_DATA_LOTTERY = 9;                    //!<当天获得的元宝数
        public const int DAILY_DATA_MISSION_LOTTERY = 10;           //!<当天任务获得的元宝数
        public const int DAILY_DATA_EXCHANGE_LIMIT = 11;            //!<兑换限制,第一位，300元宝兑换6W开心豆，2位,30元宝兑换2参赛券
        public const int DAILY_DATA_GAME_COUNT_ROOM_LEVEL0 = 12;    //新手场玩的局数
        public const int DAILY_DATA_GAME_COUNT_ROOM_LEVLE1 = 13;    //初级房间玩的局数
        public const int DAILY_DATA_GAME_COUNT_ROOM_LEVEL2 = 14;    //中级房间玩的局数
        public const int DAILY_DATA_ACTIVE_CARD_USED = 15;          //是否使用激活卡
        public const int DAILY_DATA_WIN_COUNT = 16;                 //赢的局数
        public const int DAILY_DATA_LOST_COUNT = 17;
        public const int DAILY_DATA_PRESTIGE = 18;                  //!<游戏时长(声望)
        public const int DAILY_DATA_PROP1 = 19;                     //!<用户属性1
        public const int DAILY_DATA_PROP2 = 21;                     //!<用户属性2
        public const int DAILY_DATA_PROP3 = 22;                     //!<用户属性3
        public const int DAILY_DATA_PROP4 = 23;                     //!<用户属性4
        public const int DAILY_DATA_PROP5 = 24;                     //!<用户属性5
        public const int DAILY_COUNT = 25;
        public const int GAME_NAME_LEN = 20;                        //!<游戏名长度

        #region PropIDRelative
        //通用道具
        public const int PROP_ID_PRESTIGE_ = -8;                    //!<旧的声望
        public const int PROP_ID_PRESTIGE = -7;                     //!<声望
        public const int PROP_ID_VIP_ = -6;                         //!<VIP
        public const int PROP_ID_RMB_ = -5;                         //!<人民币
        public const int PROP_ID_XZMONEY_ = -4;                     //!<吉祥币
        public const int PROP_ID_LOTTERY_ = -3;                     //!<元宝
        public const int PROP_ID_MONEY_ = -2;                       //!<游戏币
        public const int PROP_ID_BANK_ = -1;                        //!<背包存款
        public const int PROP_ID_SPEAKER = 1;                       //!<小喇叭
        public const int PROP_ID_PK_TICKET = 2;                     //!<参赛券    道具相关配置已删除这里只做进入房间消息应答
        public const int PROP_ID_SPREAD_PRESENT = 3;                //!<推广礼盒
        public const int PROP_ID_LOTTERY_CARD = 4;                  //!<抽奖卡
        public const int PROP_ID_ACTIVE_CARD = 5;                   //!<激活卡
        public const int PROP_ID_FLEE_CLEAR = 6;                    //!<逃跑率清零
        public const int PROP_ID_7 = 7;                             //!<拖鞋
        public const int PROP_ID_8 = 8;                             //!<鸡蛋
        public const int PROP_ID_9 = 9;                             //!<发泄
        public const int PROP_ID_10 = 10;                           //!<玫瑰
        public const int PROP_ID_11 = 11;                           //!<吻
        public const int PROP_ID_12 = 12;                           //!<纸巾
        public const int PROP_ID_13 = 13;                           //!<表情
        public const int PROP_ID_BANK = 14;                         //!<别墅
        public const int PROP_ID_MONEY = 15;                        //!<游戏币
        public const int PROP_ID_LOTTERY = 16;                      //!<元宝
        public const int PROP_ID_XZMONEY = 17;                      //!<吉祥币
        public const int PROP_ID_RMB = 18;                          //!<人民币
        public const int PROP_ID_VIP = 19;                          //!<VIP
        public const int PROP_ID_CARD1 = 21;                        //!<卡片套装1
        public const int PROP_ID_CARD2 = 22;
        public const int PROP_ID_CARD3 = 23;
        public const int PROP_ID_CARD4 = 24;
        public const int PROP_ID_CARD5 = 25;
        public const int PROP_ID_CARD6 = 26;
        public const int PROP_ID_CARD7 = 27;
        public const int PROP_ID_CARD8 = 28;
        public const int PROP_ID_CARD9 = 29;
        public const int PROP_ID_TYPE_WITH_TIME = 256;              //!<后续道具跟时间相关
        public const int PROP_ID_WEEK_PK_CARD = 256;                //!<周赛参赛券 道具相关配置已删除这里只做进入房间消息应答
        public const int PROP_ID_MONTH_PK_CARD = 257;               //!<月赛参赛券 道具相关配置已删除这里只做进入房间消息应答
        public const int PROP_ID_TEMP = 258;                        //!<临时道具，比如月饼

        // 魔法道具 --begin
        public const int PROP_ID_MAGIC_PROP_BEGIN = 267;
        public const int PROP_ID_272 = 272;
        public const int PROP_ID_273 = 273;
        public const int PROP_ID_274 = 274;
        public const int PROP_ID_275 = 275;
        public const int PROP_ID_276 = 276;
        public const int PROP_ID_277 = 277;
        public const int PROP_ID_278 = 278;
        public const int PROP_ID_279 = 279;
        public const int PROP_ID_280 = 280;
        public const int PROP_ID_281 = 281;
        public const int PROP_ID_MAGIC_PROP_END = 282;
        // 魔法道具 --end
        #endregion

        public const uint GAME_GROUP_UNKOWN = 0x00000000;           //!<未知类
        public const uint GAME_GROUP_POKER = 0x00000001;            //!<扑克类游戏
        public const uint GAME_GROUP_MAHJONG = 0x00000002;          //!<麻将类游戏
        public const uint GAME_GROUP_CHESS = 0x00000004;            //!<棋类游戏
        public const uint GAME_GROUP_LEISURE = 0x00000008;          //!<休闲类游戏
        public const uint GAME_GROUP_FLASH = 0x00000010;            //!<Flash游戏
        public const uint GAME_GROUP_ALONE = 0x00000020;            //!<单机游戏
        public const uint GAME_PACKET_LEISURE = 1 << 20;            //!<休闲游戏包，共可使用15种包，值不能超过0x01000000
        public const uint GAME_PACKET_POKER = 2 << 20;              //!<扑克包
        public const uint GAME_PACKET_MAHJONG = 3 << 20;            //!<麻将包
        public const uint GAME_PACKET_COMMON = 4 << 20;             //!<通用包
        public const uint GAME_GROUP_LIVE = 0X01000000;             //!<娱乐类
        public const uint GAME_GROUP_PKLIST = 0x02000000;           //!<比赛分组
        public const uint GAME_GROUP_MYGAME = 0x04000000;           //!<我的游戏
        public const uint GAME_GROUP_OTHER = 0x80000000;            //!<其他类型

        public static readonly uint MSG_HEADER_FLAG_NONE        = 0;        //!<发送数据不做任何处理
        public static readonly uint MSG_HEADER_FLAG_PACKET      = 0x1;	    //!<当前为包文件（一个或者多个连续包），每个包都有个包标记@brief msg_Packet 当所有包首发完毕才能处理
        public static readonly uint MSG_HEADER_FLAG_MASK        = 0x2;      //!<检查掩码,@ref	msg_Header::byMask 将当前包的校验码，当前包也会重组数据
        public static readonly uint MSG_HEADER_FLAG_ENCODE      = 0x4;      //!<重新编码,使用映射表重新编码数据
        public static readonly uint MSG_HEADER_FLAG_COMPRESS    = 0x8;      //!<压缩数据,数据将启用zlib压缩，该标记仅推荐，如果数据压缩后更大，将忽略该标记
        public static readonly uint MSG_HEADER_FLAG_ROUTE       = 0x10;     //!<路由包,服务器每次中转，@ref	msg_Header::byRouteCount 的值累加1，当到达特定值后，仍然没有到达目标服务器，该包将丢弃
        public static readonly uint MSG_HEADER_FLAG_DELAYSEND   = 0x20;     //!<延迟消息，等待下次非延迟消息合并发送（客户端忽略该标记）

        public static readonly uint MSG_HEADER_FLAG_FILLED      = 0x40;     //!<已经填充头部（已经进行掩码、编码、压缩处理)，不能重复处理
        public static readonly uint MSG_HEADER_FLAG_OFFSET      = 0x80;	    //!<指定消息头偏移，掩码、编码、压缩处理将从消息头开始偏移@ref	msg_Header::byHeaderOffset 个字节开始

        public static int GetMsgOffset(ushort msgId)
        {
            return msgId >> 12;
        }


    }
}

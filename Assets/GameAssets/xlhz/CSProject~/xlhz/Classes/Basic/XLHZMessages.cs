// @Author: tanjinhua
// @Date: 2021/8/18  18:22

namespace NS_XLHZ
{
    public static class XLHZMessages
    {
        public const ushort MSG_C2S_GAME_CHAT                       =   0;      // 聊天
        public const ushort MSG_S2C_GAME_CHAT_NOTIFY                =   1;      // 聊天应答

        public const ushort MSG_S2C_BASE_SCORE                      =   2;      // 游戏基础分

        public const ushort MSG_C2S_READY_REQUEST                   =   3;      // 准备
        public const ushort MSG_S2C_READY_REQUEST_BROADCAST         =   4;      // 准备应答

        public const ushort MSG_S2C_GAMESTART_BROADCAST             =   5;      // 游戏开始
        public const ushort MSG_S2C_GAMEEND_BROADCAST               =   6;      // 游戏结束

        public const ushort MSG_C2S_CHANGECARD_REQUEST              =   7;      // 换牌请求
        public const ushort MSG_S2C_CHANGECARD_BROADCAST            =   8;      // 换牌广播
        public const ushort MSG_S2C_CHANGETYPE_BROADCAST            =   9;      // 交换方式广播

        public const ushort MSG_C2S_DINGQUE_REQUST                  =   10;     // 定缺请求
        public const ushort MSG_S2C_DINGQUE_BROADCAST               =   11;     // 定缺广播

        public const ushort MSG_S2C_SENDCARD_BROADCAST              =   12;     // 发牌

        public const ushort MSG_C2S_OUTCARD_REQUST                  =   13;     // 出牌请求
        public const ushort MSG_S2C_OUTCARD_BROADCAST               =   14;     // 出牌广播

        public const ushort MSG_C2S_ACTION_REQUST                   =   15;     // 动作请求
        public const ushort MSG_S2C_ACTION_BROADCAST                =   16;     // 动作广播

        public const ushort MSG_S2C_GANG_INFO                       =   17;     // 杠操作信息
        public const ushort MSG_S2C_HU_INFO                         =   18;     // 胡操作信息
        public const ushort MSG_S2C_LIUJU_SCORE                     =   19;     // 留局信息

        public const ushort MSG_S2C_DISCONNECT_BROADCAST            =   20;     // 掉线
        public const ushort MSG_S2C_CURRENT_GAMESTATE               =   21;     // 掉线消息
        public const ushort MSG_S2C_CURRENT_GAMESTATE_END           =   22;     // 游戏结束掉线重入消息

        public const ushort MSG_C2S_AUTOPLAY_REQUESET               =   23;     // 托管请求
        public const ushort MSG_S2C_AUTOPLAY_BROADCAST              =   24;     // 托管广播

        public const ushort MSG_S2C_BIAOQING                        =   25;     // 玩家表情
        public const ushort MSG_C2S_BIAOQING                        =   26;     // 玩家表情

        public const ushort MSG_S2C_CHANGE_DESK                     =   27;     // 换桌

        public const ushort MSG_S2C_TEST_SIGNAL                     =   28;     // 测试信号
        public const ushort MSG_C2S_TEST_SIGNAL                     =   29;
        public const ushort MSG_S2C_SIGNAL_INTENSITY                =   30;     // 广播信号强度

        public const ushort MSG_C2S_DELAY_READY_TIME                =   31;     // 延迟准备

        public const ushort MSG_S2C_HAVE_PLAYER_NO_MONEY            =   32;     // 有人没有钱了
        public const ushort MSG_C2S_DELAY_GAMEING_TIME              =   33;     // 延迟游戏时间
        public const ushort MSG_S2C_GIVE_UP                         =   34;     // 放弃
        public const ushort MSG_S2C_TOPUP_SUCCEED                   =   35;     // 充值成功
        public const ushort MSG_S2C_CONTINUE_GAME                   =   36;     // 继续游戏
        public const ushort MSG_C2S_TOPUP_FAIL                      =   37;     // 失败
        public const ushort MSG_S2C_RECONNECT_GIVEUP                =   38;     // 掉线重入认输

        public const ushort MSG_S2C_FRIENDROOM_DESKINFO             =   39;     // 朋友场信息
        public const ushort MSG_C2S_FRIENDROOM_APPLY_DISSOLUTION    =   40;     // 申请解散
        public const ushort MSG_C2S_FRIENDROOM_DISSOLUTION          =   41;     // 客户端解散操作
        public const ushort MSG_S2C_FRIENDROOM_DISSOLUTION          =   42;     // 服务器广播解散操作
        public const ushort MSG_S2C_FRIENDROOM_DISSOLUTION_RESULT   =   43;     // 解散结果
        public const ushort MSG_S2C_FRIENDROOM_CURRENT_GAMESTATE    =   44;     // 朋友场当前状态
        public const ushort MSG_S2C_FRIENDROOM_END                  =   45;     // 朋友场游戏结束

        public const ushort MSG_S2C_FRIENDROOM_LIXIAN               =   46;     // 朋友场离线状态
        public const ushort MSG_S2C_FRIENDROOM_LIXIAN_CURRENT       =   47;     // 朋友场离线状态断线

        public const ushort MSG_S2S_HUINFO_BROADCAST                =   48;     // 胡牌信息
        public const ushort MsgID_S2C_TingInfo                      =   72;       // 听牌常驻数据
        public const ushort MsgID_S2C_HonorScoreInfo                =   73;
        public const ushort MsgID_C2S_Request_TingInfo              =   74;                      //客户端请求听牌信息

        public const ushort MSG_S2C_VIRTUAL_TOPUP_SUCCEED           =   101;     // 虚拟充值成功

        public const ushort MSG_S2C_DWSGameEndScore                 =   260;     //段位分

        public const ushort MSG_C2S_NewClient                       =   500;    // 标记客户端是否是新的
        public const ushort MSG_C2S_CeJu                            =   501;    // 客户端测距
        public const ushort MSG_S2C_CeJu                            =   502;    // 客户端测距

        public const ushort MSG_C2S_Record                          =   503;    // 客户端上传录音地址
        public const ushort MSG_S2C_Record                          =   504;    // 下发录音地址

        public const ushort MsgID_S2C_KickPlayerCause               =   600;    // 通知客户端踢人原因
        public const ushort MsgID_C2S_KickPlayer                    =   601;    // 客户端通知踢人
        public const ushort MsgID_C2S_FRIEND_LiXian                 =   602;    // 客户端通知踢人

        public const ushort MsgID_S2C_YiPaoDuoXiangCard_Info        =   800;    // 一炮多响信息
        public const ushort Msg_S2C_FriendStartScore                =   801;    //朋友场起始分数
        public const ushort MsgID_S2C_YYY_Money                     =   802;    // 摇一摇随机到豆豆更新分数

        public const ushort MsgID_C2S_errorCard                     =   803;   // 出牌后发现手牌有多牌情况

        public const ushort MsgID_S2C_DeskInf_Normal                =   1001; //(1001)普通场桌子信息
        public const ushort MsgID_S2C_LiuShuiInfo                   =   1002;     //(1002)某玩家接收的流水内容


        public const ushort MsgID_S2C_Tasks                         =   3001;   // 从任务服务器接收多个任务
        public const ushort MsgID_S2C_Task                          =   3002;   // 从任务服务器接收单个任务
        public const ushort MsgID_C2S_TaskReward                    =   3401;   // 打开宝箱
        public const ushort MsgID_S2C_TaskReward                    =   3003;   // 打开宝箱回调

        public const ushort MsgID_S2C_Offline_CountDown             = 4066;    // 玩家离线倒计时
        public const ushort MsgID_S2c_All_Offline_CountDown         = 4067;    // 所有玩家离线倒计时（用于断线重连）

        // 赢豆受限通知
        public const ushort MsgID_S2C_Override_Money_Chairs         = 4068;     // 赢豆受限通知
    }
}

// @Author: tanjinhua
// @Date: 2021/4/27  9:49


namespace Common
{
    public static class GameMsgDefine
    {
        public const ushort MsgID_C2S_GameChat_Request                                  = 0;        // 聊天休息发送
        public const ushort MsgID_S2C_GameChat_Broadcast                                = 1;        // 聊天应答（广播）
        public const ushort MsgID_S2C_BASE_SCORE                                        = 2;        // 房间基础分
        public const ushort MsgID_S2C_LianYing_Info                                     = 10;       // 连赢信息
        public const ushort MsgID_S2C_BiaoQing_Broadcast                                = 13;       // 玩家表情（广播）
        public const ushort MsgID_C2S_BiaoQing_Request                                  = 14;       // 玩家表情发送
        public const ushort MsgID_C2S_Delay_Ready_Time                                  = 15;       // 手机发来充值过程中延长准备时间
        public const ushort MsgID_S2C_DeskInf_FRIEND_TEAM                               = 16;       // 朋友场桌子信息
        public const ushort MsgID_S2C_FRIEND_TEAM_Dissolution_Broadcast                 = 17;       // 朋友场解散申请或意见（广播）
        public const ushort MsgID_S2C_FRIEND_Dissolution_Result                         = 19;       // 朋友场解散最终结果（都同意解散，或有玩家不同意）（广播）
        public const ushort MsgID_S2C_FRIEND_TEAM_CurrentGameState                      = 18;       // 朋友场断线重连消息
        public const ushort MsgID_S2C_FRIEND_LiXian                                     = 21;       // 朋友场离线状态
        public const ushort MsgID_S2C_FRIEND_LiXian_Current                             = 22;       // 朋友场断线重连，更新离线状态
        public const ushort MsgID_C2S_FRIEND_TEAM_DissolutionApply_Request              = 23;       // 朋友场解散申请
        public const ushort MsgID_C2S_FRIEND_TEAM_DissolutionVote_Request               = 24;       // 朋友场解散意见
        public const ushort MsgID_S2C_FRIEND_TEAM_End                                   = 25;       // 朋友场结束
        public const ushort MsgID_C2S_AutoPlay_Request                                  = 27;       // 托管
        public const ushort MsgID_S2C_AutoPlay_Broadcast                                = 28;       // 托管广播
        public const ushort MsgID_S2C_CurrentGameState                                  = 30;       // 当前游戏数据(断线重连及旁观时使用)
        public const ushort MsgID_C2S_Ready_Request                                     = 32;       // 玩家准备请求
        public const ushort MsgID_S2C_Ready_Broadcast                                   = 33;       // 玩家准备广播
        public const ushort MsgID_S2C_GameStart_Broadcast                               = 34;       // 游戏开始
        public const ushort MsgID_S2C_GameEnd_Broadcast                                 = 35;       // 游戏结束
        public const ushort MsgID_C2S_GPS                                               = 41;       // 客户端测距
        public const ushort MsgID_S2C_GPS                                               = 42;       // 客户端测距
        public const ushort MsgID_C2S_Record                                            = 44;       // 发送语音接口
        public const ushort MsgID_S2C_Record                                            = 45;       // 接收语音
        public const ushort MsgID_S2C_ChangeDesk                                        = 46;       // 换桌通知
        public const ushort MsgID_S2C_Friend_CurrentPlayCount                           = 47;       // 朋友场当前局数
        public const ushort MsgID_S2C_KickPlayerCause                                   = 48;       // 收到踢人原因消息
        public const ushort MsgID_C2S_KickPlayer                                        = 49;       // 客户端通知踢人
        public const ushort MsgID_C2S_FRIEND_LiXian                                     = 50;       // 发送玩家离线状态
        public const ushort MsgID_S2C_FriendDissolutionLeftTime                         = 63;       // 朋友场解散框刷新倒计时
        public const ushort MsgID_S2C_HonorScoreInfo                                    = 71;       // 荣誉分信息

        public const ushort MsgID_C2S_NewClient_Request                                 = 500;      // 通知服务器当前是最新版本

        // 以已废弃
        //public const ushort MsgID_S2C_Tasks                                             = 3001;     // 从任务服务器接收多个任务
        //public const ushort MsgID_S2C_Task                                              = 3002;     // 从任务服务器接收单个任务
        //public const ushort MsgID_S2C_TaskReward                                        = 3003;     // 打开宝箱回调
        //public const ushort MsgID_C2S_TaskReward                                        = 3401;     // 打开宝箱
        //public const ushort MsgID_C2S_Chest_Task_Info                                   = 3500;     // 请求宝箱任务
        //public const ushort MsgID_S2C_Chest_Task_Info                                   = 3501;     // 宝箱任务信息
        //public const ushort MsgID_C2S_Chest_Task_Change                                 = 3502;     // 切换任务
        //public const ushort MsgID_S2C_Chest_Task_Change                                 = 3503;     // 切换任务返回
        //public const ushort MsgID_C2S_Chest_Task_Receive                                = 3504;     // 请求领取奖励
        //public const ushort MsgID_S2C_Chest_Task_Receive                                = 3505;     // 奖励返回
        //public const ushort MsgID_S2C_Chest_Task_Progress                               = 3506;     // 刷新进度
        //public const ushort MsgID_S2C_Chest_Task_State                                  = 3507;     // 任务状态
        //public const ushort MsgID_C2S_Chest_Task_Prize_Detail                           = 3508;     // 请求奖励详情
        //public const ushort MsgID_S2C_Chest_Task_Prize_Detail                           = 3509;     // 返回奖励详情
        //public const ushort MsgID_S2C_Chest_Award_Result                                = 3510;     // 任务详情状态


        /*============ 3500 - 4095 id通用专属，其他项目组勿使用 ============*/


        // Ranking match
        public const ushort MsgID_S2C_Ranking_Info                                      = 3520;     // 排位赛信息
        public const ushort MsgID_C2S_Ranking_Info                                      = 3521;     // 请求刷新排位赛信息
        public const ushort MsgID_S2C_Ranking_Honor_Update                              = 3522;     // 排位赛结束荣誉值刷新

        //public const ushort MsgID_S2C_YYYReward                                         = 3539;     // 摇一摇领取奖励
        //public const ushort MsgID_S2C_YYYReward_Current                                 = 3540;     // 摇一摇断线重连

        // 旧的连胜消息，已废弃
        //public const ushort MsgID_S2C_Chest_TaskLS_Info                                 = 4020;     // 连胜任务信息
        //public const ushort MsgID_S2C_Player_LS_Broadcast                               = 4021;     // 广播玩家连胜
        //public const ushort MsgID_S2C_Pop_LSBH_Broadcast                                = 4022;     // 广播玩家弹出连胜保护窗口(需修改)
        //public const ushort MsgID_C2S_AD_Delay_Require                                  = 4023;     // 请求广告延时
        //public const ushort MsgID_C2S_LS_Opearte_Result                                 = 4024;     // 通知保持连胜操作的结果
        //public const ushort MsgID_S2C_LS_Opearte_Result                                 = 4025;     // 广播保持连胜操作的结果
        //public const ushort MsgID_S2C_Close_TaskLS                                      = 4026;     // 关闭连胜任务
        //public const ushort MsgID_S2C_Chest_TaskLS_Update                               = 4027;     // 宝箱连胜任务信息刷新

        // 心悦宝箱任务
        //public const ushort MsgID_S2C_XinYue_Task_Info                                  = 4030;     // 任务信息
        //public const ushort MsgID_S2C_XinYue_Task_Update                                = 4031;     // 刷新任务
        //public const ushort MsgID_C2S_XinYue_Task_Receive                               = 4032;     // 请求任务奖励
        //public const ushort MsgID_S2C_XinYue_Task_Receive                               = 4033;     // 奖励返回
        //public const ushort MsgID_S2C_XinYue_Task_Close                                 = 4034;     // 任务关闭


        public const ushort MsgID_S2C_Game_Again                                        = 4040;     // 再来一局（收到邀请）

        // 祈福(转运)
        public const ushort MsgID_C2S_FriendCfg_ZYQS                                    = 4045;     // 请求转运求神配置
        public const ushort MsgID_S2C_FriendCfg_ZYQS                                    = 4046;     // 广播转运求神配置
        public const ushort MsgID_C2S_Friend_ZYQS                                       = 4047;     // 接收转运求神
        public const ushort MsgID_S2C_Friend_ZYQS                                       = 4048;     // 广播转运求神道具使用

        //public const ushort MsgID_C2S_ShowStaus_QiuShen                                 = 4049;     // 求神状态是否显示到左面请求(已废弃)


        // 胜局宝箱
        public const ushort MsgID_S2C_CoinMarket_Task_Info                              = 4050;     // 金币场任务信息
        public const ushort MsgID_S2C_CoinMarket_Task_Update                            = 4051;     // 刷新任务
        public const ushort MsgID_C2S_CoinMarket_Task_Receive                           = 4052;     // 请求任务奖励
        public const ushort MsgID_S2C_CoinMarket_Task_Receive                           = 4053;     // 奖励返回
        public const ushort MsgID_S2C_CoinMarket_Task_Close                             = 4054;     // 任务关闭
        // 胜局宝箱转盘
        //public const ushort MsgID_S2C_LuckWheel_Config_Broadcast                        = 4055;     // 胜局送豆转盘配置
        //public const ushort MsgID_S2C_LuckWheel_TaskInfo_Broadcast                      = 4056;     // 胜局送豆转盘任务信息
        //public const ushort MsgID_C2S_LuckWheel_Reward_Request                          = 4057;     // 请求胜局送豆转盘奖励
        //public const ushort MsgID_S2C_LuckWheel_Reward_Reply                            = 4058;     // 请求胜局送豆转盘奖励返回

        // 超时解散
        public const ushort MsgID_S2C_OverTime_Auto_Dissolution                         = 4060;     // 超时解散提示框
        public const ushort MsgID_S2C_Cancel_OverTime_Auto_Dissolution                  = 4061;     // 取消超时解散提示框
        public const ushort MsgID_S2C_OverTime_Auto_Dissolution_Chair                   = 4062;     // 玩家超时自动解散
        public const ushort MsgID_S2C_OverTime_Auto_Dissolution_IS_Open                 = 4063;     // 是否开启超时自动解散

        //public const ushort MsgID_C2S_KickPlayerDelay_Request                           = 4064;     // 请求服务器延时踢人(用于胜局宝箱领取奖励等)


        public const ushort MsgID_S2C_InviteRuleInfo                                    = 4065;     // 朋友场邀请好友附带信息


        public const ushort MsgID_S2C_Offline_CountDown                                 = 4066;     // 玩家离线倒计时
        public const ushort MsgID_S2c_All_Offline_CountDown                             = 4067;     // 所有玩家离线倒计时（用于断线重连）

        public const ushort MsgID_S2C_Override_Money_Chairs                             = 4068;     // 0未超过，1:赢豆无法超过携带上限；2:其他玩家破产输豆上限


        // 扣费功能、道具价格表消息（如魔法道具、洗牌等）
        public const ushort MsgID_C2S_CommonPriceList_Request                           = 4072;     // 请求价格表单
        public const ushort MsgID_S2C_CommonPriceList_Reply                             = 4073;     // 下发价格表单


        // 通过游戏服务器使用扣费魔法表情消息
        public const ushort MsgID_C2S_UseMagicProp_Request                              = 4074;     // 使用扣费道具请求
        public const ushort MsgID_S2C_UseMagicProp_Reply                                = 4075;     // 使用道具结果


        // 使用洗牌功能扣费请求
        public const ushort MsgID_C2S_UseShuffle_Request                                = 4076;     // 使用洗牌功能
        public const ushort MsgID_S2C_UseShuffle_Reply                                  = 4077;     // 使用洗牌功能结果返回
        //public const ushort MsgID_S2C_UseShuffle_Ani                                    = 4078;     // 玩家洗牌状态，用于播放动画


        // 规则更新提示消息
        public const ushort MsgID_S2C_NewGameRule                                       = 4079;       // 收到新游戏规则消息，显示弹窗提示


        //public const ushort MsgID_S2C_MiniGame_UserOpenID_Broadcast                     = 4080;     // 小游戏玩家openId与座位号映射表消息，用于实时语音


        public const ushort MsgID_S2C_UserDuanWeiScore_Broadcast                        = 4081;     // 各玩家当前段位分与历史最高分广播

        // 储钱罐
        public const ushort MsgID_S2C_PiggyBankInfo_Notice                              = 4082;     // 储钱罐信息通知
        public const ushort MsgID_S2C_PiggyBankClose_Notice                             = 4083;     // 储钱罐活动关闭通知
        public const ushort MsgID_C2S_PiggyBankExchangeSuccessed                        = 4084;     // 兑换成功通知服务器

        public const ushort MsgID_S2C_OverScoreFlags_Broadcast                          = 4085;     // 结算分标签类型

        // 储钱罐2
        public const ushort MsgID_S2C_PiggyBank2Info_Notice                             = 4086;     // 储钱罐信息通知
        public const ushort MsgID_S2C_PiggyBank2Close_Notice                            = 4087;     // 储钱罐活动关闭通知
        public const ushort MsgID_C2S_PiggyBank2ExchangeSuccessed                       = 4088;     // 兑换成功通知服务器
    }
}

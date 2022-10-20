// @Author: tanjinhua
// @Date: 2021/4/27  17:29


namespace MJCommon
{
    public static class MJMsgDefine
    {
        public const ushort MsgID_S2C_Test_Signal                                   = 6;        // 信号测试通知
        public const ushort MsgID_C2S_Test_Signal                                   = 7;        // 发送信号测试
        public const ushort MsgID_S2C_Signal_Intensity                              = 8;        // 收到信号强度消息
        public const ushort MsgID_S2C_SendCard_Broadcast    						= 36;       // 游戏发牌
        public const ushort MsgID_C2S_OutCard_Request    						    = 37;       // 请求出牌
        public const ushort MsgID_S2C_OutCard_Broadcast    						    = 38;       // 玩家出牌
        public const ushort MsgID_C2S_Action_Request                                = 39;       // 直接动作
        public const ushort MsgID_S2C_ActionResult_Broadcast                        = 40;       // 动作结果
        public const ushort MsgID_S2C_Friend_Task                                   = 51;       // 朋友场任务
        public const ushort MsgID_S2C_Friend_TaskOver                               = 52;       // 朋友场任务完成
        public const ushort MsgID_S2C_ActionResult2_Broadcast                       = 53;       // 抢杠胡这类动作事件
        public const ushort MsgID_S2C_SendCard_FenZang                              = 54;       // 分赃发牌
        public const ushort MsgID_S2C_HuInfo_Broadcast                              = 57;       // 胡牌信息显示，只显示多少张
        public const ushort MsgID_S2C_TingInfo_Broadcast                            = 58;       // 听牌信息显示
        public const ushort MsgID_S2C_HuInfo2_Broadcast                             = 59;       // 胡牌信息显示，显示多少张和多少番
        public const ushort MsgID_S2C_HuInfo3_Broadcast                             = 60;       // 胡牌信息显示，显示多少张和胡牌类型(如平胡、夹胡等)
        public const ushort MsgID_S2C_TingInfo2_Broadcast                           = 61;       // 听牌信息显示，显示多少张和多少番
        public const ushort MsgID_S2C_TingInfo3_Broadcast                           = 62;       // 听牌信息显示，显示多少张和胡牌类型(如平胡、夹胡等)

        //public const ushort MsgID_S2C_Can_HaiDiLaoYue                               = 65;       // 游戏结束之前下发记录
        //public const ushort MsgID_C2S_HaiDiLaoYue                                   = 66;       // 请求海底捞月
        //public const ushort MsgID_S2C_HaiDiLaoYue_Success                           = 67;       // 返回海底捞月抽中结果
        //public const ushort MsgID_S2C_HaiDiLaoYue_Failed                            = 68;       // 返回海底捞月未抽中结果
        //public const ushort MsgID_S2C_HaiDiLaoYue_ResetGame                         = 69;       // 返回海底捞月断线重连
        //public const ushort MsgID_S2C_ExitShareInfo                                 = 70;       // 退出分享信息
        //public const ushort MsgID_S2C_OverTime_Dissolution                          = 72;       // 超时解散提示框
        //public const ushort MsgID_S2C_Cancel_OverTime_Dissolution                   = 73;       // 取消超时解散提示框
        //public const ushort MsgID_S2C_OverTime_Dissolution_Chair                    = 74;       // 玩家超时自动解散




        /*============ 3500 - 4095 id通用专属，其他项目组勿使用 ============*/

        // 部分游戏朋友场使用奖杯替换分数显示
        public const ushort MsgID_S2C_Update_Cup                                    = 3511;     // 奖牌刷新
        public const ushort MsgID_S2C_Not_Enough_Cup                                = 3512;     // 奖牌不足


        //public const ushort MsgID_S2C_CarryMoney_Money                              = 4069;     // 带豆玩家带入场（补充）的豆豆数量
        //public const ushort MsgID_C2S_CarryMoney_Recharge_Request                   = 4070;     // 带豆玩家充值请求
        //public const ushort MsgID_S2C_CarryMoney_Recharge_Result                    = 4071;     // 带豆玩家充值结果
    }
}

// @Author: tanjinhua
// @Date: 2021/4/12  9:36


using System.Collections.Generic;
using Common;
using WLHall.Game;

namespace MJCommon
{
    public class MJGameData : GameData
    {
        #region Configs
        /// <summary>
        /// 是否使用东北方言音效
        /// </summary>
        public virtual bool useNortheastDialect => false;

        /// <summary>
        /// 东北方言音效是否忽略方言设置开关
        /// </summary>
        public virtual bool northeastDialectIgnoresSetting => false;

        /// <summary>
        /// 头像是否显示庄家小图标
        /// </summary>
        public virtual bool showBankerIcon => true;

        /// <summary>
        /// 手牌最多可以有几张牌
        /// </summary>
        public virtual int maxHandCardCount => 14;

        /// <summary>
        /// 牌墙墩数数组，初始化MJSpace时，从配置设值
        /// </summary>
        public virtual List<int> wallStacks => Wall.Stacks136;

        /// <summary>
        /// 牌墙每个牌墩有几张牌，默认2张
        /// </summary>
        public virtual int countPerStack => 2;

        /// <summary>
        /// 从牌墙中拿牌时是否按顺时针方向拿，默认顺时针
        /// </summary>
        public virtual bool takeCardClockwise => true;

        /// <summary>
        /// 使用骰子值数牌墙时是否按顺时针方向数，默认逆时针
        /// </summary>
        public virtual bool diceClockwise => false;

        /// <summary>
        /// 骰子使用情况，默认不播放骰子动画，但是依然使用两个骰子值来确定牌墙开始拿牌位置
        /// </summary>
        public virtual DiceUsage diceUsage => DiceUsage.Hidden;

        /// <summary>
        /// 托管倒计时时间(秒)
        /// </summary>
        public virtual int trustTimeout => 12;

        /// <summary>
        /// 总牌数
        /// </summary>
        public virtual int totalCardCount
        {
            get
            {
                int result = 0;
                for (int i = 0; i < wallStacks.Count; i++)
                {
                    result += wallStacks[i] * countPerStack;
                }
                return result;
            }
        }

        /// <summary>
        /// 剩余牌数
        /// </summary>
        public virtual int remainCardCount => totalCardCount - sendedCardCount - sendedKongCardCount;

        /// <summary>
        /// 剩余牌数显示红色临界值，默认-1，即不显示
        /// </summary>
        public virtual int remainCardCountWarnLimit => -1;
        #endregion



        #region Dynamic Datas
        public int gameState = GameState.Free;

        /// <summary>
        /// 庄家服务器座位号
        /// </summary>
        public int bankerChairId = Chair.Invalid;

        /// <summary>
        /// 骰子值数组，长度为2，使用一个骰子时忽略第二个值
        /// </summary>
        public List<int> diceValues = new List<int> { 1, 1 };


        /// <summary>
        /// 当前打出牌牌值，用于验证、防止多出牌等用途
        /// </summary>
        public int verifyingOutCardValue = Card.Invalid;


        /// <summary>
        /// 服务器已经发出的补杠牌数量
        /// </summary>
        public int sendedKongCardCount = 0;


        /// <summary>
        /// 服务器已经发出的牌数量
        /// </summary>
        public int sendedCardCount = 0;


        /// <summary>
        /// 赖子牌牌值数组
        /// </summary>
        public List<int> jokerCardValues = new List<int>();


        /// <summary>
        /// 赖子牌是和否可打出
        /// </summary>
        public bool jokerDiscardable = false;


        /// <summary>
        /// 当前动作事件数据(克隆)
        /// </summary>
        public List<MJActionData> currentActionDatas
        {
            get
            {
                if (_currentActionDatas == null)
                {
                    return new List<MJActionData>();
                }

                return new List<MJActionData>(_currentActionDatas);
            }

            set => _currentActionDatas = value;
        }


        /// <summary>
        /// 胡牌提示数据
        /// </summary>
        public MsgHuInfo huInfo = null;


        /// <summary>
        /// 听牌提示数据
        /// </summary>
        public MJHintsData tingInfo = null;


        /// <summary>
        /// 动作是否完成
        /// </summary>
        public bool isActionDone = true;
        #endregion



        private List<MJActionData> _currentActionDatas;


        /// <summary>
        /// 初始化，动态数据设置默认值。注：此方法会在每局游戏开始时调用，朋友场需要每局累加的数据不要在这里初始化
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            gameState = GameState.Free;
            bankerChairId = Chair.Invalid;
            diceValues = new List<int> { 1, 1 };
            verifyingOutCardValue = Card.Invalid;
            sendedKongCardCount = 0;
            sendedCardCount = 0;
            jokerCardValues = new List<int>();
            jokerDiscardable = false;
            currentActionDatas = null;
            huInfo = null;
            tingInfo = null;
            isActionDone = true;
        }


        public override BaseGamePlayer OnCreatePlayer() => new MJGamePlayer();


        /// <summary>
        /// 当前是否存在动作事件
        /// </summary>
        /// <returns></returns>
        public bool HasAction()
        {
            return _currentActionDatas != null;
        }


        /// <summary>
        /// 当前是否存在某一showType类型动作事件
        /// </summary>
        /// <param name="showType"></param>
        /// <returns></returns>
        public bool HasAction(int showType)
        {
            if (!HasAction())
            {
                return false;
            }

            foreach (MJActionData data in _currentActionDatas)
            {
                if (data.showType == showType)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 获取指定showType动作数据，找到第一个马上返回
        /// </summary>
        /// <param name="showType"></param>
        /// <returns></returns>
        public MJActionData GetActionData(int showType)
        {
            if (!HasAction(showType))
            {
                WLDebug.LogWarning($"MJGameData.GetActionData: 当前不存在showType为{showType}的动作事件");

                return default;
            }

            foreach (MJActionData data in _currentActionDatas)
            {
                if (data.showType == showType)
                {
                    return data;
                }
            }

            return default;
        }


        /// <summary>
        /// 获取指定showType的所有动作数据
        /// </summary>
        /// <param name="showType"></param>
        /// <returns></returns>
        public List<MJActionData> GetActionDatas(int showType)
        {
            if (!HasAction(showType))
            {
                WLDebug.LogWarning($"MJGameData.GetActionDatas: 当前不存在showType为{showType}的动作事件");

                return null;
            }

            List<MJActionData> result = new List<MJActionData>();

            foreach (MJActionData data in _currentActionDatas)
            {
                if (data.showType == showType)
                {
                    result.Add(data);
                }
            }

            return result;
        }
    }
}

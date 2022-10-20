// MJDefine.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/14

using System.Collections.Generic;

namespace MJCommon
{
    [System.Serializable]
    public enum DiceUsage
    {
        /// <summary>
        /// 使用一个骰子
        /// </summary>
        One,
        /// <summary>
        /// 使用两个骰子
        /// </summary>
        Two,
        /// <summary>
        /// 隐藏(不播放骰子动画)，但还是会用两个骰子值来决定牌墙开始拿牌的位置
        /// </summary>
        Hidden
    }


    /// <summary>
    /// 方位类型
    /// </summary>
    public enum Wind
    {
        South,
        West,
        North,
        East
    }

    /// <summary>
    /// 以自己视角的方位枚举,对应客户端座位号
    /// </summary>
    public enum MJOrientation
    {
        /// <summary>
        /// 自己
        /// </summary>
        Down,
        /// <summary>
        /// 下家
        /// </summary>
        Right,
        /// <summary>
        /// 对家
        /// </summary>
        Up,
        /// <summary>
        /// 上家
        /// </summary>
        Left,
        /// <summary>
        /// 总数
        /// </summary>
        Count
    }

    /// <summary>
    /// 换牌类型定义
    /// </summary>
    public enum ExchangeCardType
    {
        /// <summary>
        /// 顺时针
        /// </summary>
        Clockwise,
        /// <summary>
        /// 逆时针
        /// </summary>
        Anticlockwise,
        /// <summary>
        /// 对家
        /// </summary>
        Cross
    }

    /// <summary>
    /// 游戏状态定义
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// 无
        /// </summary>
        public const int Free = 0;
        /// <summary>
        /// 换牌
        /// </summary>
        public const int Exchange = 1;
        /// <summary>
        /// 定缺
        /// </summary>
        public const int Dingque = 2;
        /// <summary>
        /// 正在打牌
        /// </summary>
        public const int Play = 3;
        /// <summary>
        /// 游戏结束
        /// </summary>
        public const int End = 4;
    }


    /// <summary>
    /// 座位类型
    /// </summary>
    public static class Chair
    {
        public const int Down = 0;
        public const int Right = 1;
        public const int Up = 2;
        public const int Left = 3;
        public const int Invalid = 0xFFFF;   // 无效座位号65535
    }

    /// <summary>
    /// 游戏结束类型定义
    /// </summary>
    public static class EndType
    {
        public const ushort ZiMo = 0;           // 自摸
        public const ushort DianPao = 1;        // 点炮
        public const ushort QiangGangHu = 2;    // 抢杠胡
        public const ushort HuangZhuang = 3;    // 荒庄
        public const ushort TaoPao = 4;         // 逃跑
        public const ushort Bao = 5;            // 宝牌
        public const ushort DuiBao = 6;         // 对宝
        public const ushort BaoZhongBao = 7;    // 宝中宝
        public const ushort KanDuiBao = 8;      // 坎对宝
        public const ushort JieSan = 9;         // 解散
        public const ushort LiuJu = 10;         // 留局
    }

    /// <summary>
    /// 回放记录ID定义
    /// </summary>
    public static class Record
    {
        public const int Initial                = 0;
        public const int SendCard               = 1;
        public const int OutCard                = 2;
        public const int Action                 = 3;    // 废弃，请勿使用该消息记录
        public const int Bao                    = 4;
        public const int Ting                   = 5;
        public const int End                    = 6;    // 游戏结束
        public const int ActionEvent            = 7;    // 显示动作事件按钮以及操作
        public const int ActionResult           = 8;    // 新的动作事件
        public const int ScrambleActionResult   = 9;    // 关联动作事件，如抢杠胡
        public const int Reserve                = 50;   // 50以内为预留通用事件，请勿使用
    }

    /// <summary>
    /// 通用动作事件按钮(特效)显示类型
    /// </summary>
    public static class ActionShowType
    {
        public const int Calcel = -2;
        public const int Guo = -1;
        public const int Chi = 1;
        public const int Peng = 2;
        public const int Gang = 3;
        public const int Ting = 4;
        public const int Hu = 5;
        public const int Zimo = 6;
        public const int Dianpao = 7;
        public const int Buhua = 11;   // 补花
    }

    /// <summary>
    /// 动作类型定义
    /// </summary>
    public static class ActionType
    {
        public const int None                  = 0;    // 无动作
        public const int FangQi                = 1;    // 放弃
        // 吃        
        public const int ChiLeft               = 10;   // 左吃
        public const int ChiCenter             = 11;   // 中吃
        public const int ChiRight              = 12;   // 右吃
        // 碰        
        public const int Peng                  = 30;   // 碰
        // 杠        
        public const int GangMing              = 50;   // 明杠
        public const int GangAn                = 51;   // 暗杠
        public const int GangPuBuGang          = 52;   // 普补杠(由碰补成明杠)
        public const int GangXi                = 53;   // 喜杠
        public const int GangYao               = 54;   // 幺杠
        public const int GangJiu               = 55;   // 九杠        
        public const int Gang3FengQueDong      = 56;   // 三风杠(西南北)
        public const int Gang3FengQueNan       = 57;   // 三风杠(东西北)
        public const int Gang3FengQueXi        = 58;   // 三风杠(东南北)
        public const int Gang3FengQueBei       = 59;   // 三风杠(东南西)
        public const int Gang4Feng             = 60;   // 四风杠
        public const int GangHuiPi             = 61;   // 会皮杠
        public const int GangHui               = 62;   // 会杠
        public const int GangHui1Gang          = 63;   // 会幺杠
        public const int Deprecated            = 64;   // 这个值请废弃不要使用
        public const int GangHuiXiGang         = 65;   // 会喜杠
        public const int GangHuiFengGang       = 66;   // 会风杠
        public const int XiGangZhong           = 67;   // 喜杠带中 
        public const int XiGangFa              = 68;   // 喜杠带发
        public const int XiGangBai             = 69;   // 喜杠带白
        public const int PengBao               = 70;   // 碰宝牌（算明杠）
        public const int GangBao               = 71;   // 杠宝牌（算暗杠）
        public const int QueTuiYJGang          = 72;   // 瘸腿幺九杠
        public const int HuaGang               = 73;   // 花杠
        public const int GangHui9Gang          = 74;   // 会九杠
        public const int GangMingBu            = 75;   // 明杠(补)
        public const int GangAnBu              = 76;   // 暗杠(补)
        public const int GangPuBuGangBu        = 77;   // 普补杠(由碰补成明杠)(补)
        // 听        
        public const int Ting                  = 100;  // 听牌
        public const int ChiLeftTing           = 101;  // 左吃听
        public const int ChiCenterTing         = 102;  // 中吃听
        public const int ChiRightTing          = 103;  // 右吃听
        public const int PengTing              = 104;  // 碰听
        public const int GangTing              = 105;  // 杠听
        // 胡牌       
        public const int Hu                    = 120;  // 胡牌
        public const int HuBaoPai              = 121;  // 宝牌

        public const int TiHui                 = 140;  // 提会
        public const int TiHui1Gang            = 141;  // 提1
        public const int TiHui9Gang            = 142;  // 提9
        public const int TiHuiFengGang         = 143;  // 提风
        public const int TiHuiXiGang           = 144;  // 提喜
                         
        //           
        public const int DingZhang             = 160;  // 定掌
        public const int ChuPai                = 161;  // 选择出牌牌值
        //           
        public const int Invalid               = 255;  // 错误类型
    }

    /// <summary>
    /// 牌值定义
    /// </summary>
    public static class Card
    {
        public const int Invalid = 0;           // 无效
        public const int Rear = 0xFF;           // 牌背

        public const int ColorNone = 0xFF;      // 无效花色
        public const int ColorWan = 0;          // 万
        public const int ColorTiao = 1;         // 条
        public const int ColorBing = 2;         // 饼
        public const int ColorFengzi = 3;       // 风、字
        public const int ColorFlower = 4;       // 花牌

        public const int ColorMask = 0xF0;      // 花色掩码
        public const int ValueMask = 0x0F;      // 数值掩码
        public const int HuiMask = 0x80;        // 会牌掩码
        public const int MaxIndex = 34;         // 手牌索引长度
        public const int MeldCount = 4;         // 副子长度

        // 万
        public const int Wan1 = 1;
        public const int Wan2 = 2;
        public const int Wan3 = 3;
        public const int Wan4 = 4;
        public const int Wan5 = 5;
        public const int Wan6 = 6;
        public const int Wan7 = 7;
        public const int Wan8 = 8;
        public const int Wan9 = 9;

        // 条
        public const int Tiao1 = 17;
        public const int Tiao2 = 18;
        public const int Tiao3 = 19;
        public const int Tiao4 = 20;
        public const int Tiao5 = 21;
        public const int Tiao6 = 22;
        public const int Tiao7 = 23;
        public const int Tiao8 = 24;
        public const int Tiao9 = 25;

        // 筒
        public const int Bing1 = 33;
        public const int Bing2 = 34;
        public const int Bing3 = 35;
        public const int Bing4 = 36;
        public const int Bing5 = 37;
        public const int Bing6 = 38;
        public const int Bing7 = 39;
        public const int Bing8 = 40;
        public const int Bing9 = 41;

        // 风牌（东南西北）
        public const int Dong = 49;
        public const int Nan = 50;
        public const int Xi = 51;
        public const int Bei = 52;

        // 箭牌（中发白）
        public const int Zhong = 53;
        public const int Fa = 54;
        public const int Bai = 55;

        // 花牌（春夏秋冬梅兰竹菊）
        public const int FlowerChun = 65;
        public const int FlowerXia = 66;
        public const int FlowerQiu = 67;
        public const int FlowerDong = 68;
        public const int FlowerMei = 69;
        public const int FlowerLan = 70;
        public const int FlowerZhu = 71;
        public const int FlowerJu = 72;


        /// <summary>
        /// 获取牌花类型值
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public static int GetCardColorValue(int cardValue)
        {
            return cardValue >> 4;
        }
    }

    /// <summary>
    /// 牌墙定义
    /// </summary>
    public static class Wall
    {
        public static List<int> Stacks144 = new List<int> { 18, 18, 18, 18 };  // 万条筒、东南西北、中发白、花
        public static List<int> Stacks136 = new List<int> { 17, 17, 17, 17 };  // 万条筒、东南西北、中发白
        public static List<int> Stacks132 = new List<int> { 17, 16, 17, 16 };  // 万条筒、东南西北、中发
        public static List<int> Stacks128 = new List<int> { 16, 16, 16, 16 };  // 万条同、中发白、花牌
        public static List<int> Stacks120 = new List<int> { 15, 15, 15, 15 };  // 万条筒、中发白
        public static List<int> Stacks112 = new List<int> { 14, 14, 14, 14 };  // 万条筒、中
        public static List<int> Stacks108 = new List<int> { 14, 13, 14, 13 };  // 万条筒
        public static List<int> Stacks72 = new List<int> { 18, 0, 18, 0 };     // 万条
    }

    /// <summary>
    /// 麻将常量定义
    /// </summary>
    public class MJDefine
    {
        /// <summary>
        /// 麻将牌尺寸的x值
        /// </summary>
        public const float MJCardSizeX = 0.04053742f;
        /// <summary>
        /// 麻将牌尺寸的y值
        /// </summary>
        public const float MJCardSizeY = 0.02745409f;
        /// <summary>
        /// 麻将牌尺寸的z值 
        /// </summary>
        public const float MJCardSizeZ = 0.05318443f;
        /// <summary>
        /// 手牌层掩码
        /// </summary>
        public const string MJHandMask = "HandCard";
        /// <summary>
        /// 无效副子索引值
        /// </summary>
        public const int InvaildFuziIndex = 0xFF;


        public static Dictionary<int, MJCardMatOffset> MJCardMatOffsets = new Dictionary<int, MJCardMatOffset>()
        {
            {Card.Tiao1, new MJCardMatOffset(0, 0) },
            {Card.Tiao2, new MJCardMatOffset(1, 0) },
            {Card.Tiao3, new MJCardMatOffset(2, 0) },
            {Card.Tiao4, new MJCardMatOffset(3, 0) },
            {Card.Tiao5, new MJCardMatOffset(4, 0) },
            {Card.Tiao6, new MJCardMatOffset(5, 0) },
            {Card.Tiao7, new MJCardMatOffset(6, 0) },
            {Card.Tiao8, new MJCardMatOffset(7, 0) },
            {Card.Tiao9, new MJCardMatOffset(8, 0) },

            {Card.Wan1, new MJCardMatOffset(0, 1) },
            {Card.Wan2, new MJCardMatOffset(1, 1) },
            {Card.Wan3, new MJCardMatOffset(2, 1) },
            {Card.Wan4, new MJCardMatOffset(3, 1) },
            {Card.Wan5, new MJCardMatOffset(4, 1) },
            {Card.Wan6, new MJCardMatOffset(5, 1) },
            {Card.Wan7, new MJCardMatOffset(6, 1) },
            {Card.Wan8, new MJCardMatOffset(7, 1) },
            {Card.Wan9, new MJCardMatOffset(8, 1) },

            {Card.Bing1, new MJCardMatOffset(0, 2) },
            {Card.Bing2, new MJCardMatOffset(1, 2) },
            {Card.Bing3, new MJCardMatOffset(2, 2) },
            {Card.Bing4, new MJCardMatOffset(3, 2) },
            {Card.Bing5, new MJCardMatOffset(4, 2) },
            {Card.Bing6, new MJCardMatOffset(5, 2) },
            {Card.Bing7, new MJCardMatOffset(6, 2) },
            {Card.Bing8, new MJCardMatOffset(7, 2) },
            {Card.Bing9, new MJCardMatOffset(8, 2) },

            {Card.Dong, new MJCardMatOffset(0, 3) },
            {Card.Nan, new MJCardMatOffset(1, 3) },
            {Card.Xi, new MJCardMatOffset(2, 3) },
            {Card.Bei, new MJCardMatOffset(3, 3) },
            {Card.Zhong, new MJCardMatOffset(4, 3) },
            {Card.Fa, new MJCardMatOffset(5, 3) },
            {Card.Bai, new MJCardMatOffset(6, 3) },

            {Card.FlowerChun, new MJCardMatOffset(7, 3) },
            {Card.FlowerXia, new MJCardMatOffset(8, 3) },
            {Card.FlowerQiu, new MJCardMatOffset(0, 4) },
            {Card.FlowerDong, new MJCardMatOffset(1, 4) },
            {Card.FlowerMei, new MJCardMatOffset(2, 4) },
            {Card.FlowerLan, new MJCardMatOffset(3, 4) },
            {Card.FlowerZhu, new MJCardMatOffset(4, 4) },
            {Card.FlowerJu, new MJCardMatOffset(5, 4) },
        };
    }

    public class MJCardMatOffset
    {
        public MJCardMatOffset(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float x;
        public float y;
    }
}
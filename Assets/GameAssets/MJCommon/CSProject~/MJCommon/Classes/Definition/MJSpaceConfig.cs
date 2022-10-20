// MJSpaceConfig.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/08/04

using System.Collections.Generic;
using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 麻将位置配置
    /// </summary>
    public class MJSpaceConfig
    {
        public MJWallConfig wallConfig;
        public MJDeskCardConfig deskConfig;
        public MJMeldConfig meldConfig;
        public MJHandSetConfig handConfig;
        public MJWinSetConfig winSetConfig;
    }

    [System.Serializable]
    public class MJWinSetConfig
    {
        /// <summary>
        /// 位置
        /// </summary>
        public DVector3[] position;

        /// <summary>
        /// 缩放
        /// </summary>
        public double[] scale;
    }
    /// <summary>
    /// 麻将牌墙位置配置
    /// </summary>
    [System.Serializable]
    public class MJWallConfig
    {
        /// <summary>
        /// 每个单边牌墙的牌墩数量
        /// </summary>
        public List<int> stackCountPerSide;

        public int countPerStack = 2;

        public bool takeCardClockwise = true;

        public bool diceClockwise = false;

        public DiceUsage diceUsage = DiceUsage.Hidden;

        /// <summary>
        /// 中心距离
        /// </summary>
        public double centreDistance;
    }

    /// <summary>
    /// 麻将桌面上牌的位置配置
    /// </summary>
    [System.Serializable]
    public class MJDeskCardConfig
    {
        /// <summary>
        /// 位置
        /// </summary>
        public DVector3[] position;
        /// <summary>
        /// 排列
        /// </summary>
        public int[] arrange;
        /// <summary>
        /// 缩放
        /// </summary>
        public double[] scale;
        /// <summary>
        /// 是否反转对家的牌
        /// </summary>
        public bool invert;
    }

    /// <summary>
    /// 麻将副子位置配置
    /// </summary>
    [System.Serializable]
    public class MJMeldConfig
    {
        /// <summary>
        /// 副子位置
        /// </summary>
        public DVector3[] position;
        /// <summary>
        /// 副子栈方向
        /// </summary>
        public MeldStackDirection direction;
        /// <summary>
        /// 副子间隔
        /// </summary>
        public double interval;
        /// <summary>
        /// 缩放
        /// </summary>
        public double[] scale;
        /// <summary>
        /// 是否反转对家的牌
        /// </summary>
        public bool invert;
    }

    /// <summary>
    /// 手牌配置
    /// </summary>
    [System.Serializable]
    public class MJHandSetConfig
    {
        public DVector3[] position;
        public HandCardAnchor anchor;
        public int maxHandCardCount;
        public double interval;
        public double liftHeight;
    }

    /// <summary>
    /// JsonData不支持float类型，只支持double
    /// 所以定义该类用于序列化
    /// </summary>
    [System.Serializable]
    public class DVector3
    {
        public double x;
        public double y;
        public double z;

        public DVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }
    }

    /// <summary>
    /// 副子栈的方向
    /// </summary>
    public enum MeldStackDirection
    {
        LeftToRight,
        RightToLeft
    }

    /// <summary>
    /// 手牌的锚点
    /// </summary>
    public enum HandCardAnchor
    {
        Left,
        Right
    }
}

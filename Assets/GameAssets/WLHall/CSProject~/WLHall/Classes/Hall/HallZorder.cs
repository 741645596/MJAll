using System;

namespace WLHall
{
    public class HallZorder
    {
        // z坐标之间的间隙
        public const int Gap = 200;

        /// <summary>
        /// 默认WDirector.GetRootLayer为0
        /// </summary>
        public const int Begin = 0;

        // 按钮层
        public const int Btn = Gap;

        public const int Tv = Gap * 2;

        public const int Desk = Gap * 3;

        public const int Sofa = Gap * 4;

        public const int RoomBg = Gap * 5;

        public const int DynBuilding = Gap * 6;

        public const int Building = Gap * 7;

        public const int Cloud = Gap * 8;

        public const int Sky = Gap * 9;

        /// 值越大越靠后显示
        /// <summary>
        /// 背景坐标
        /// </summary>
        public const int Bg = Gap * 10;

        /// <summary>
        /// 大厅最远的z坐标，预留给游戏内
        /// </summary>
        public const int End = Bg;

    }
}

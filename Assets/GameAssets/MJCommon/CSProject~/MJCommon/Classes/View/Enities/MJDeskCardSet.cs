// MJDeskCardSet.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/16

using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 单边桌面上已经打出牌的集合
    /// </summary>
    public class MJDeskCardSet : MJCardSet
    {
        /// <summary>
        /// 每行默认牌的数量
        /// </summary>
        private const int DEFAULT_COUNT = 6;

        /// <summary>
        /// 排列方式，数组表示每行有几张牌，超过以最后一个值排列
        /// </summary>
        protected int[] arrange = new int[] { DEFAULT_COUNT };

        /// <summary>
        /// 是否反转对家的手牌
        /// </summary>
        protected bool isInvert;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="cards">牌值数组</param>
        public MJDeskCardSet()
        {
            gameObject.name = "MJDeskCardSet";
        }

        public void UpdateConfig(MJDeskCardConfig config)
        {
            if(config == null || config.arrange == null || config.arrange.Length == 0)
            {
                return;
            }
            arrange = config.arrange;
            isInvert = config.invert;
            Refresh();
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        public override void Refresh()
        {
            for (int i = 0; i < count; i++)
            {
                MJCard card = GetCard(i);

                CalculatePlacementArgs(i, out int layer, out int row, out int rowCount, out int indexInRow);

                var trs = card.gameObject.transform;
                trs.localPosition = CalculatePlacement(layer, row, rowCount, indexInRow);
                trs.localEulerAngles = Vector3.zero;
                
                if(orientation == MJOrientation.Up && isInvert)
                {
                    card.TowardUpInvert();
                }
                else
                {
                    card.TowardUp();
                }

                if (layer != 0) // 不是最下面一层隐藏阴影
                {
                    card.HideShadow();
                }
            }
        }


        /// <summary>
        /// 计算布局参数
        /// </summary>
        /// <param name="index"></param>
        /// <param name="layer"></param>
        /// <param name="row"></param>
        /// <param name="rowCount"></param>
        /// <param name="indexInRow"></param>
        public void CalculatePlacementArgs(int index, out int layer, out int row, out int rowCount, out int indexInRow)
        {
            int maxPerLayer = 0;
            foreach (int c in arrange)
            {
                maxPerLayer += c;
            }

            int indexInLayer = index % maxPerLayer; // 按单层算的当前索引
            int totalCount = arrange[0];

            layer = index / maxPerLayer;            // 当前在第几层
            row = 0;                                // 当前在第几行
            indexInRow = indexInLayer;              // 按单行算的当前索引
            rowCount = arrange[0];                // 当前行牌数量 

            for (int i = 1; i < arrange.Length; i++)
            {
                if (indexInLayer >= totalCount)
                {
                    row = i;
                    indexInRow -= rowCount;
                    rowCount = arrange[i];
                    totalCount += rowCount;
                }
            }
        }

        /// <summary>
        /// 计算布局位置
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 CalculatePlacement(int index)
        {
            CalculatePlacementArgs(index, out int layer, out int row, out int rowCount, out int indexInRow);

            return CalculatePlacement(layer, row, rowCount, indexInRow);
        }

        /// <summary>
        /// 计算布局
        /// </summary>
        /// <param name="layer">第几层</param>
        /// <param name="row">第几行</param>
        /// <param name="rowCount">该行总共摆几个牌</param>
        /// <param name="indexInRow">在该行中的第几个</param>
        /// <returns></returns>
        public Vector3 CalculatePlacement(int layer, int row, int rowCount, int indexInRow)
        {
            float dx = MJDefine.MJCardSizeX;
            float dy = MJDefine.MJCardSizeY;
            float dz = MJDefine.MJCardSizeZ;

            float rowWidth = rowCount * dx;
            float x = indexInRow * dx - rowWidth * 0.5f + dx * 0.5f;
            float y = (layer + 0.5f) * dy;
            float z = -dz * 0.5f - row * dz;

            return new Vector3(x, y, z);
        }
    }
}
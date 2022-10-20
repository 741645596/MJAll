// MJStack.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2020/07/16

using UnityEngine;

namespace MJCommon
{
    /// <summary>
    /// 牌墩
    /// </summary>
    public class MJStack : MJCardSet
    {

        public bool empty => count == 0;


        public float width => GetSize().x;


        public MJStack()
        {
            gameObject.name = "MJStack";
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="count"></param>
        public void Initialize(int count)
        {
            Reload(count);

            Refresh();
        }

        /// <summary>
        /// 获取牌墩的尺寸
        /// </summary>
        /// <returns></returns>
        public override Vector3 GetSize()
        {
            float x = MJDefine.MJCardSizeX;
            float y = MJDefine.MJCardSizeY * count;
            float z = MJDefine.MJCardSizeZ;

            return new Vector3(x, y, z);
        }

        public override void Refresh()
        {
            for (var i = 0; i < count; i++)
            {
                float dy = MJDefine.MJCardSizeY;

                float y = dy * 0.5f + (count - 1 - i) * dy;

                MJCard card = GetCard(i);

                card.gameObject.transform.localPosition = new Vector3(0, y, 0);

                card.TowardDown();

                if (i != count - 1)
                {
                    card.HideShadow();
                }
            }
        }

        /// <summary>
        /// 根据每墩牌数量重新加载
        /// </summary>
        /// <param name="count">每墩几张牌</param>
        public void Reload(int count)
        {
            int[] values = new int[count];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 0;
            }

            Reload(values);
        }

        /// <summary>
        /// 设置材质
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="card_mat_key"></param>
        /// <param name="back_mat_key"></param>
        public void SetMaterial(string assetName, string card_mat_key, string back_mat_key)
        {
            Traverse((c, i) => c?.SetMaterial(assetName, card_mat_key, back_mat_key));
        }

    }
}
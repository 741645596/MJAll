// MJWall.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2020/07/16

using System;
using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 单边的牌墙
    /// </summary>
    public class MJWall : BaseEntity
    {
        #region Properties
        /// <summary>
        /// 牌墩数量
        /// </summary>
        public int count => _stackCount;

        /// <summary>
        /// 是否是空牌墙
        /// </summary>
        public bool empty => count == 0;

        /// <summary>
        /// 最大宽度
        /// </summary>
        public float maxWidth => _maxWidth;
        #endregion


        protected MJOrientation orientation;

        private float _maxWidth;
        private int _stackCount;
        private List<MJStack> _stacks;
        private int _countPerStack = 2;
        private bool _takeCardClockwise = true;

        public MJWall()
        {
            gameObject.name = "MJWall";

            _stacks = new List<MJStack>(); 
        }


        /// <summary>
        /// 初始化麻将牌墙
        /// </summary>
        /// <param name="stackCount">总共牌墩数量</param>
        /// <param name="countPerStack">每墩几张牌，默认2</param>
        public void Initialize(int stackCount, int countPerStack = 2, bool takeCardClockwise = true)
        {
            _countPerStack = countPerStack;

            _takeCardClockwise = takeCardClockwise;

            Reload(stackCount, countPerStack);

            Refresh();
        }


        /// <summary>
        /// 遍历
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="reverse"></param>
        public void Traverse(Action<MJStack, int> handler, bool reverse = false)
        {
            if (reverse)
            {
                for (int i = _stacks.Count - 1; i >= 0; i--)
                {
                    handler?.Invoke(Get(i), i);
                }

                return;
            }

            for (int i = 0; i < _stacks.Count; i++)
            {
                handler?.Invoke(Get(i), i);
            }
        }


        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="stackCount"></param>
        /// <param name="countPerStack"></param>
        public void Reload(int stackCount, int countPerStack = 2)
        {
            Clear();

            _maxWidth = 0;

            _stackCount = stackCount;

            for (int i = 0; i < stackCount; i++)
            {
                MJStack stack = new MJStack();

                stack.Initialize(countPerStack);

                stack.gameObject.transform.SetParent(gameObject.transform);

                _stacks.Add(stack);

                _maxWidth += stack.width;

                var assetName = "MJCommon/MJ/mj_cards";

                var card_mat_key = $"materials/mj_card_wall_{(int)orientation}.mat";

                var back_mat_key = $"materials/mj_back_wall_{(int)orientation}.mat";

                stack.SetMaterial(assetName, card_mat_key, back_mat_key);

            }
        }


        /// <summary>
        /// 移除所有
        /// </summary>
        public void Clear()
        {
            _stacks.ForEach(stack =>
            {
                stack.Traverse((card, i) => MJCardPool.Recycle(card));
                stack.Destroy();
            });

            _stacks.Clear();
        }


        public Vector3 GetSize()
        {
            float x = maxWidth,
                y = MJDefine.MJCardSizeY * _countPerStack,
                z = MJDefine.MJCardSizeZ;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        public void Refresh()
        {
            for (int i = 0; i < _stacks.Count; i++)
            {
                MJStack stack = Get(i);

                float halfWidth = maxWidth * 0.5f;

                float width = stack.width;

                float increasement = width * (i + 0.5f);

                float x = _takeCardClockwise ? halfWidth - increasement : -halfWidth + increasement;

                stack.gameObject.transform.localPosition = new Vector3(x, 0, 0);

                stack.gameObject.transform.localEulerAngles = Vector3.zero;
            }
        }

        public void SetOrientation(MJOrientation orientation)
        {
            this.orientation = orientation;
            var q = Quaternion.Euler(0, (int)orientation * -90, 0);
            gameObject.transform.localRotation = q;
        }

        /// <summary>
        /// 获取牌墩
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MJStack Get(int index)
        {
            return _stacks[index];
        }
    }
}
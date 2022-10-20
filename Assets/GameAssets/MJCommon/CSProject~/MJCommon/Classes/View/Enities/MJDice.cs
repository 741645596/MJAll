// MJDice.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2020/07/31

using System.Collections.Generic;
using Unity.Core;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 骰子
    /// </summary>
    public class MJDice : BaseEntity
    {
        /// <summary>
        /// 骰子列表，支持多个骰子
        /// </summary>
        private readonly List<GameObject> diceList = new List<GameObject>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="values">骰子值的列表</param>
        public MJDice(List<int> values)
        {
            gameObject = new GameObject("MJDice");
            var r = 0.015f;
            float a = 360 / values.Count;
            float m = 30;

            for (int i = 0; i < values.Count; i++)
            {
                var asset = "MJCommon/MJ/mj_env";
                var key = $"prefabs/mj_dice_{values[i]}.prefab";
                var prefab = AssetsManager.Load<GameObject>(asset, key);
                if (prefab != null)
                {
                    var dice = Object.Instantiate(prefab);
                    dice.transform.SetParent(gameObject.transform);
                    diceList.Add(dice);

                    var angle = Random.Range(a * i + m, a * (i + 1) - m);
                    var z = Mathf.Sin(Mathf.Deg2Rad * angle) * r;
                    var x = Mathf.Cos(Mathf.Deg2Rad * angle) * r;
                    dice.transform.localPosition = new Vector3(x, 0, z);
                }
            }
        }

        /// <summary>
        /// 创建单个骰子
        /// </summary>
        /// <param name="value"></param>
        public MJDice(int value)
        {
            gameObject = new GameObject("MJDice");

            var asset = "MJCommon/MJ/mj_env";
            var key = $"prefabs/mj_dice_{value}.prefab";
            var prefab = AssetsManager.Load<GameObject>(asset, key);

            if (prefab != null)
            {
                var dice = Object.Instantiate(prefab, gameObject.transform);
                diceList.Add(dice);
            }
        }

        /// <summary>
        /// 获取骰子对象列表
        /// </summary>
        /// <returns></returns>
        public List<GameObject> GetDiceList()
        {
            return diceList;
        }

        // 实现抽象方法，创建GameObject的操作在构造方法内完成
        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}

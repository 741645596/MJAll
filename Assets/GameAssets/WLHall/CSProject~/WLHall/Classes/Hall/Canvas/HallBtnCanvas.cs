using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace WLHall
{
    /// <summary>
    /// 大厅首页按钮层
    /// </summary>
    public class HallBtnCanvas : WLayer
    {
        public static new HallBtnCanvas Create()
        {
            return new HallBtnCanvas();
        }

        public HallBtnCanvas()
        {
            InitGameObject("WLHall/Main/hall_f_other", "BtnCanvas.prefab");

            // 这里注册按钮的点击事件
        }
    }
}

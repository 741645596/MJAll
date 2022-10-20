using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace WLHall
{
    /// <summary>
    /// 大厅首页桌子层：地球仪、亲友圈、背包
    /// </summary>
    public class HallDeskCanvas : WLayer
    {
        public static new HallDeskCanvas Create()
        {
            return new HallDeskCanvas();
        }

        public HallDeskCanvas()
        {
            InitGameObject("WLHall/Main/hall_f_other", "DeskCanvas.prefab");

            // 这里注册按钮的点击事件
        }
    }
}

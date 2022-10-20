using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace WLHall
{
    /// <summary>
    /// 大厅Tv面板
    /// </summary>
    public class HallTvCanvas : WLayer
    {
        public static new HallTvCanvas Create()
        {
            return new HallTvCanvas();
        }

        public HallTvCanvas()
        {
            InitGameObject("WLHall/Main/hall_f_other", "TvCanvas.prefab");

            // 这里注册按钮的点击事件
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/9/7  17:06

using Unity.Widget;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

namespace MJCommon
{
    public class MJAvatarPanel : WLayer
    {
        public MJAvatarPanel()
        {
            var obj = CanvasHelper.CreateEmptyCanvas();
            obj.name = "avatar_panel";
            obj.AddComponent<GraphicRaycaster>();
            InitGameObject(obj);
        }
    }
}

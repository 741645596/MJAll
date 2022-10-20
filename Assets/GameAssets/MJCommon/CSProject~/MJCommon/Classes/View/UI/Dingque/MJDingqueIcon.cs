// @Author: tanjinhua
// @Date: 2021/10/20  20:09

using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJDingqueIcon : WNode
    {

        private GameObject _effect;

        public MJDingqueIcon(int colorValue)
        {
            var asset = "MJCommon/MJ/mj_ui_prefabs";
            var key = $"dingque_icon_{colorValue}.prefab";
            InitGameObject(asset, key);

            _effect = FindReference("effect").gameObject;
        }

        public void PlayEffect()
        {
            _effect.gameObject.SetActive(true);
        }
    }
}

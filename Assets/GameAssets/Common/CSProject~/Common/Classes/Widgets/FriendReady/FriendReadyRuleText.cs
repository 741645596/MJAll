// @Author: tanjinhua
// @Date: 2021/9/16  15:36

using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;

namespace Common
{
    public class FriendReadyRuleText : WNode
    {
        public Action<Vector2> onUpdateSize;

        private SKText _text;

        public FriendReadyRuleText()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "friendready_ruletext.prefab");

            _text = gameObject.GetComponent<SKText>();

            _text.onUpdateSize += () => onUpdateSize?.Invoke(rectTransform.rect.size);
        }

        public void SetRules(List<string> rules)
        {
            var str = "";
            for (int i = 0; i < rules.Count; i++)
            {
                str += rules[i] + (i != rules.Count - 1 ? "、" : "");
            }

            _text.text = str;
        }
    }
}

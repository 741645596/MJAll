// @Author: tanjinhua
// @Date: 2021/5/13  23:23


using System;
using System.Collections.Generic;
using Common;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJDingquePanel : WLayer
    {
        public Action<int> onSelect;

        private GameObject _buttonRoot;
        private GameObject _infoLack;
        private GameObject[] _recommandEffects;
        private RectTransform[] _buttons;

        public MJDingquePanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "dingque_panel.prefab");

            _buttonRoot = FindInChildren("btns").gameObject;
            _infoLack = FindInChildren("info_lack").gameObject;
            _recommandEffects = new GameObject[]
            {
                FindInChildren("btns/character/character_recommand").gameObject,
                FindInChildren("btns/banboo/banboo_recommand").gameObject,
                FindInChildren("btns/dot/dot_recommand").gameObject
            };
            _buttons = new RectTransform[3];
            _buttons[0] = FindInChildren<RectTransform>("btns/character");
            _buttons[0].gameObject.GetComponent<PressButton>().onClick.AddListener(() => onSelect?.Invoke(Card.ColorWan));
            _buttons[1] = FindInChildren<RectTransform>("btns/banboo");
            _buttons[1].gameObject.GetComponent<PressButton>().onClick.AddListener(() => onSelect?.Invoke(Card.ColorTiao));
            _buttons[2] = FindInChildren<RectTransform>("btns/dot");
            _buttons[2].gameObject.GetComponent<PressButton>().onClick.AddListener(() => onSelect?.Invoke(Card.ColorBing));
        }

        public void ShowButtons(int recommendColorValue)
        {
            _buttonRoot.SetActive(true);
            _infoLack.SetActive(true);

            for (int i = 0; i < _recommandEffects.Length; i++)
            {
                _recommandEffects[i].SetActive(i == recommendColorValue);
            }
        }

        public void HideButtons()
        {
            _buttonRoot.SetActive(false);
            _infoLack.SetActive(false);
        }

        public Vector2 GetButtonDisplayPos(int colorValue)
        {
            return _buttons[colorValue].GetDisplayPosition();
        }

        public void ShowInfoWaitOthers()
        {
            var keys = new List<string>
            {
                "dingque/info_ddqtwjxz.png",
                "dingque/info_dot_y.png",
                "dingque/info_dot_y.png",
                "dingque/info_dot_y.png"
            };

            var info = CreateInfoAnim(keys, layout.center_bottom, new Vector2(0, 230));

            info.PlayJump(1);

            // TODO：使用新资源
        }


        public void ShowInfoSelecting(int viewChairId, Vector2 pos)
        {
            // TODO: 缺资源
        }


        public void ShowInfoSelected(int viewChairId, Vector2 pos)
        {
            // TODO: 缺资源
        }

        private SequenceAnimation CreateInfoAnim(List<string> keys, Vector2 layout, Vector2 offset)
        {
            var asset = "MJCommon/MJ/mj_ui_atlas";

            var anim = new SequenceAnimation(asset, keys);
            anim.AddTo(this);
            anim.SetAnchor(new Vector2(0.5f, 0.5f));
            anim.Layout(layout, offset);
        
            return anim;
        }
    }
}

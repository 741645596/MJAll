// @Author: tanjinhua
// @Date: 2021/12/17  13:51

using System;
using Unity.Widget;
using UnityEngine.UI;
using UnityEngine;

namespace Common
{
    public class ReplayGameSelection : WNode
    {
        public int index;
        public Action<int> onSelect;

        private SKText _indexTxt;
        private Image _selectedImg;

        public ReplayGameSelection(int index)
        {
            InitGameObject("Common/Game/replay_ui_prefabs", "game_selection.prefab");

            this.index = index;
            _indexTxt = FindReference<SKText>("txt_index");
            _indexTxt.text = (index + 1).ToString();

            _selectedImg = gameObject.GetComponent<Image>();
            gameObject.GetComponent<Button>().onClick.AddListener(() => Select());
        }

        public void Select(bool sendEvent = true)
        {
            _selectedImg.color = Color.white;
            _indexTxt.color = cc.color("#43BAFF");
            if (sendEvent)
            {
                onSelect?.Invoke(index);
            }
        }

        public void Deselect()
        {
            _selectedImg.color = Color.clear;
            _indexTxt.color = Color.white;
        }
    }
}

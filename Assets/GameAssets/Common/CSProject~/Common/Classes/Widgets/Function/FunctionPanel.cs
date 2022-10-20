// @Author: tanjinhua
// @Date: 2021/9/13  15:08

using UnityEngine;
using Unity.Widget;
using UnityEngine.UI;
using System;

namespace Common
{
    public class FunctionPanel : WLayer
    {
        public Action onClickFlowBtn;
        public Action onClickHintBtn;
        public Action onClickMenuBtn;


        private RectTransform _flowBtn;
        private RectTransform _hintBtn;
        private RectTransform _menuBtn;

        public FunctionPanel()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "function_panel.prefab");

            _flowBtn = FindInChildren("lower_left_group/waterflow") as RectTransform;
            _hintBtn = FindInChildren("lower_left_group/hint") as RectTransform;
            _menuBtn = FindInChildren("menu_button") as RectTransform;

            _flowBtn.GetComponent<Button>().onClick.AddListener(() => onClickFlowBtn?.Invoke());
            _hintBtn.GetComponent<Button>().onClick.AddListener(() => onClickHintBtn?.Invoke());
            _menuBtn.GetComponent<Button>().onClick.AddListener(() => onClickMenuBtn?.Invoke());
        }

        /// <summary>
        /// 设置提示按钮可见
        /// </summary>
        /// <param name="visible"></param>
        public void SetHintButtonVisible(bool visible)
        {
            _hintBtn.gameObject.SetActive(visible);
        }
    }
}

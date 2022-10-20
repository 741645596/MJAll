// @Author: tanjinhua
// @Date: 2021/4/20  10:02


using System;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    /// <summary>
    /// 胡牌提示层
    /// </summary>
    public class MJHintsPanel : WNode
    {
        /// <summary>
        /// 点开自动打牌开关事件
        /// </summary>
        public Action<bool> onClickAuto;

        private const int MaxColumn = 5;     // 最大摆几列提示节点
        private RectTransform _bgTrs;
        private Toggle _autoToggle;
        private ArtText _totalCount;
        private GridLayoutGroup _container;
        private RectTransform _containerTrs;
        private Rect _containerBorder;
        private SKText _textWinAny;

        public MJHintsPanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "hint_panel.prefab");

            _bgTrs = FindInChildren("background") as RectTransform;
            _autoToggle = FindInChildren<Toggle>("background/auto_toggle");
            _totalCount = FindInChildren<ArtText>("background/total_count");
            _container = FindInChildren<GridLayoutGroup>("background/container");
            _containerTrs = _container.transform as RectTransform;
            _containerBorder = new Rect
            {
                position = _containerTrs.offsetMin,
                size = _containerTrs.offsetMax
            };
            _textWinAny = FindReference<SKText>("text_win_any");
            _autoToggle.onValueChanged.AddListener(isOn => onClickAuto?.Invoke(isOn));
        }

        /// <summary>
        /// 设置自动打牌开关是否可见
        /// </summary>
        /// <param name="visible"></param>
        public void SetAutoToggleVisible(bool visible)
        {
            _autoToggle.gameObject.SetActive(visible);
        }

        /// <summary>
        /// 设置自动打牌开关勾选状态
        /// </summary>
        /// <param name="isOn"></param>
        public void SetAutoToggleIsOn(bool isOn)
        {
            _autoToggle.isOn = isOn;
        }

        /// <summary>
        /// 显示胡牌提示
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="configs"></param>
        public void ShowHints(int totalCount, MJHintCell.Config[] configs)
        {
            if (totalCount == 0)
            {
                return;
            }

            _totalCount.gameObject.SetActive(true);
            _totalCount.text = totalCount + "张";
            _container.transform.RemoveAllChildren();
            _textWinAny.gameObject.SetActive(false);

            for (int i = 0; i < configs.Length; i++)
            {
                var showSlice = i % MaxColumn < (MaxColumn - 1) && i != (configs.Length - 1);
                var cell = new MJHintCell(configs[i], showSlice);
                cell.AddTo(_containerTrs);
            }
            _bgTrs.sizeDelta = CalculateBackgroundSize(configs.Length);
        }


        /// <summary>
        /// 显示“能胡所有牌”
        /// </summary>
        public void ShowAnyHints()
        {
            _totalCount.gameObject.SetActive(false);
            _textWinAny.gameObject.SetActive(true);
            _container.transform.RemoveAllChildren();
            _bgTrs.sizeDelta = new Vector2(400, 160);
        }

        private Vector2 CalculateBackgroundSize(int dataCount)
        {
            int row = Mathf.CeilToInt((float)dataCount / MaxColumn);
            int column = dataCount < MaxColumn ? dataCount : MaxColumn;
            Vector2 cellSize = _container.cellSize;
            Vector2 spacing = _container.spacing;
            RectOffset padding = _container.padding;

            float containerWidth = column * cellSize.x + (column - 1) * spacing.x + padding.left + padding.right;
            float containerHeight = row * cellSize.y + (row - 1) * spacing.y + padding.top + padding.bottom;

            Rect border = _containerBorder;
            float width = containerWidth + Mathf.Abs(border.position.x) + Mathf.Abs(border.size.x);
            float height = containerHeight + Mathf.Abs(border.position.y) + Mathf.Abs(border.size.y);

            return new Vector2(width, height);
        }
    }
}

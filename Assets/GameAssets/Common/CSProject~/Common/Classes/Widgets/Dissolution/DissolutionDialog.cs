// @Author: lili
// @Date: 2021/5/11  15:35

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace Common
{
    /// <summary>
    /// 解散弹窗
    /// </summary>
    public class DissolutionDialog : WLayer
    {
        public struct Config
        {
            public string avatarUrl;
            public string nickname;
            public uint userId;
            public int gender;
            public bool isInitiator;
            public bool agreed;
        }

        public Action<byte> onClickButton;

        private Button _argeeBtn;
        private Button _refuseBtn;
        private RectTransform _bgTrs;
        private VerticalLayoutGroup _container;
        private DissolutionInitior _initior;
        private Dictionary<uint, DissolutionPlayer> _players;

        public DissolutionDialog()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "dissolution_dialog.prefab");

            _argeeBtn = FindInChildren<Button>("background/btn_argee");
            _argeeBtn.onClick.AddListener(() => onClickButton?.Invoke(1));
            _refuseBtn = FindInChildren<Button>("background/btn_refuse");
            _refuseBtn.onClick.AddListener(() => onClickButton?.Invoke(0));
            _bgTrs = FindReference<RectTransform>("background");
            _container = FindInChildren<VerticalLayoutGroup>("background/container");
        }

        public void SetConfigs(List<Config> configs)
        {
            var copy = new List<Config>(configs);
            int idx = copy.GetIndexByCondition(c => c.isInitiator);
            if (idx == -1)
            {
                WLDebug.LogWarning("DissolutionDialog.SetConfigs: 参数配置错误");
                return;
            }

            _container.transform.RemoveAllChildren();
            _players = new Dictionary<uint, DissolutionPlayer>();

            float height = 0f;
            int spaceCount = 1;

            var cfg = copy[idx];
            _initior = new DissolutionInitior(cfg);
            var invitorTrs = _initior.rectTransform;
            invitorTrs.SetParent(_container.transform, false);
            height += invitorTrs.rect.size.y;
            copy.RemoveAt(idx);

            for (var i = 0; i < copy.Count; i++)
            {
                var showSlice = i != copy.Count - 1;
                var player = new DissolutionPlayer(copy[i], showSlice);
                var playerTrs = player.rectTransform;
                playerTrs.SetParent(_container.transform, false);
                height += playerTrs.rect.size.y;
                spaceCount += i != copy.Count - 1 ? 1 : 0;
                _players.Add(copy[i].userId, player);
            }

            UpdateSize(height, spaceCount);
        }


        /// <summary>
        /// 设置按钮是否可见
        /// </summary>
        public void SetButtonsVisible(bool visible)
        {
            _argeeBtn.gameObject.SetActive(visible);

            _refuseBtn.gameObject.SetActive(visible);
        }

        /// <summary>
        /// 开始计时器
        /// </summary>
        /// <param name="time"></param>
        public void StartCountdown(int time)
        {
            _initior.StartCountdown(time);
        }

        /// <summary>
        /// 根据玩家ID显示同意状态
        /// </summary>
        public void ShowAgree(uint userId)
        {
            if (!_players.ContainsKey(userId))
            {
                WLDebug.LogWarning($"DissolutionDialog.ShowAgree: 玩家节点不存在，userId = {userId}");
                return;
            }

            _players[userId].ShowArgee();
        }

        private void UpdateSize(float contentHeight, int spaceCount)
        {
            float spacing = spaceCount * _container.spacing;
            float paddingHeight = _container.padding.top + _container.padding.bottom;
            float height = contentHeight + spacing + paddingHeight;
            float width = _bgTrs.rect.size.x;

            var containerTrs = _container.transform as RectTransform;
            var offsetMin = containerTrs.offsetMin;
            var offsetMax = containerTrs.offsetMax;
            height += Mathf.Abs(offsetMin.y) + Mathf.Abs(offsetMax.y);

            _bgTrs.SetContentSizeInZero(new Vector2(width, height));
            _bgTrs.Layout(new Vector2(0.5f, 0.5f));
        }
    }
}

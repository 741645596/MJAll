// @Author: lili
// @Date: 2021/5/20 18:55:14

using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    /// <summary>
    /// 朋友场准备界面
    /// </summary>
    public class FriendReadyLayer : WLayer
    {
        public class Config
        {
            public string title;    // 游戏名字
            public string roomKey;  // 房号
            public string gameCountTitle;   // 局数自定义显示 例如：圈数
            public string gameCountValue;   // 局数自定义显示
            public string otherInfoTitle; // 例如：翻数，默认为null
            public string otherInfo; // 例如：8翻，默认为null
            public uint deskNum;    // 桌号
            public List<string> rules;  // 规则
            public bool isOpenGPS;  // 是否开启gps
            public bool isOvertimeDisslotion;   // 是否开启超时解散
            public bool isAccumulatingOfflineTime;  // 是否累计离线时间
        }

        public Action onClickReady;
        public Action onClickInvite;
        public Action onClickDissolution;
        public Action onClickExit;


        private Button _exitBtn;
        private Button _dismissBtn;
        private Button _readyBtn;
        private Button _inviteBtn;

        private Text _gameTitle;
        private Image _infoReady;
        private VerticalLayoutGroup _container;
        private RectTransform _bgTrs;

        private FriendReadySummary _summary;
        private FriendReadyRuleText _ruleText;
        private FriendReadyExtraInfo _extraInfo;

        public FriendReadyLayer()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "friend_ready.prefab");

            _exitBtn = FindInChildren<Button>("exit");
            _exitBtn.onClick.AddListener(() => onClickExit?.Invoke());
            _dismissBtn = FindInChildren<Button>("dismiss");
            _dismissBtn.onClick.AddListener(() => onClickDissolution?.Invoke());
            _readyBtn = FindInChildren<Button>("bg/btn_ready");
            _readyBtn.onClick.AddListener(() =>
            {
                _readyBtn.gameObject.SetActive(false);
                onClickReady?.Invoke();
            });
            _inviteBtn = FindInChildren<Button>("bg/btn_invite");
            _inviteBtn.onClick.AddListener(() => onClickInvite?.Invoke());

            _gameTitle = FindInChildren<Text>("bg/game_name");
            _infoReady = FindInChildren<Image>("bg/info_ready");
            _container = FindInChildren<VerticalLayoutGroup>("bg/container");
            _bgTrs = FindInChildren<RectTransform>("bg");
        }

        public void SetConfig(Config config)
        {
            _gameTitle.text = config.title;

            float summaryHeight = 0;
            float ruleHeight = 0;
            float extraInfoHeight = 0;

            var containerTrs = _container.transform as RectTransform;
            var summary = GetSummary();
            summary.AddTo(containerTrs);
            summary.onUpdateSize = s =>
            {
                summaryHeight = s.y;
                UpdateSize(summaryHeight, ruleHeight, extraInfoHeight, 0);
            };
            summary.SetConfig(config);

            if (config.rules.Count > 0)
            {
                var rule = GetRuleText();
                rule.AddTo(containerTrs);
                rule.onUpdateSize = s =>
                {
                    ruleHeight = s.y;
                    UpdateSize(summaryHeight, ruleHeight, extraInfoHeight, 1);
                };
                rule.SetRules(config.rules);
            }

            if (config.isOpenGPS || config.isOvertimeDisslotion || config.isAccumulatingOfflineTime)
            {
                var extraInfo = GetExtraInfo();
                extraInfo.AddTo(containerTrs);
                extraInfo.onUpdateSize = s =>
                {
                    extraInfoHeight = s.y;
                    UpdateSize(summaryHeight, ruleHeight, extraInfoHeight, 2);
                };
                extraInfo.SetConfig(config);
            }
        }

        private void UpdateSize(float summaryHeight, float ruleHeight, float extraInfoHeight, int spaceCount)
        {
            var height = summaryHeight + ruleHeight + extraInfoHeight;
            var containerTrs = _container.transform as RectTransform;
            var h = height + _container.spacing * spaceCount + _container.padding.top + _container.padding.bottom;
            var w = containerTrs.rect.size.x;
            var offsetMin = containerTrs.offsetMin;
            var offsetMax = containerTrs.offsetMax;

            w += Mathf.Abs(offsetMin.x) + Math.Abs(offsetMax.x);
            h += Math.Abs(offsetMin.y) + Math.Abs(offsetMax.y);
            _bgTrs.SetContentSizeInZero(new Vector2(w, h));
            _bgTrs.Layout(new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        }

        public void ShowExitButton()
        {
            _exitBtn.gameObject.SetActive(true);
            (_exitBtn.transform as RectTransform).LayoutScreen(new Vector2(0, 1), cc.p(163, -42));
        }

        public void ShowDismissButton()
        {
            _dismissBtn.gameObject.SetActive(true);
            (_dismissBtn.transform as RectTransform).LayoutScreen(new Vector2(0, 1), cc.p(163, -42));
        }

        public void ShowReadyButton()
        {
            _readyBtn.gameObject.SetActive(true);
        }

        public void ShowInviteButton()
        {
            _inviteBtn.gameObject.SetActive(true);
        }

        public void ShowReady()
        {
            _infoReady.gameObject.SetActive(true);
        }

        private FriendReadySummary GetSummary()
        {
            if (_summary == null)
            {
                _summary = new FriendReadySummary();
            }
            return _summary;
        }

        private FriendReadyRuleText GetRuleText()
        {
            if (_ruleText == null)
            {
                _ruleText = new FriendReadyRuleText();
            }
            return _ruleText;
        }

        private FriendReadyExtraInfo GetExtraInfo()
        {
            if (_extraInfo == null)
            {
                _extraInfo = new FriendReadyExtraInfo();
            }
            return _extraInfo;
        }
    }
}

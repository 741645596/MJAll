// @Author: lili
// @Date: 2021/5/11  15:50

using System.Collections.Generic;
using Common;
using DG.Tweening;
using Unity.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJAvatar : AvatarBase
    {
        private RemoteImage _remoteImage;
        private Image _bankcrupcyImage;
        private Image _readyImage;
        private Text _scoreText;
        private Image _trustImage;
        private GameObject _lightEf;
        private Dictionary<string, RectTransform> _icons;

        public MJAvatar(int viewChairId, uint userId, int gender, string avatarUrl)
        {
            this.viewChairId = viewChairId;

            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "avatar.prefab");
            _remoteImage = FindInChildren<RemoteImage>("content/remote");
            _bankcrupcyImage = FindInChildren<Image>("content/bankcrupcy");
            _readyImage = FindInChildren<Image>("content/ready");
            _scoreText = FindInChildren<Text>("content/score");
            _trustImage = FindInChildren<Image>("content/trust");
            _lightEf = FindInChildren("light").gameObject;
            FindInChildren<Button>("content").onClick.AddListener(() => onClick?.Invoke(userId));
            SetUserInfo(userId, gender, avatarUrl);

            var trs = gameObject.transform as RectTransform;
            trs.anchorMin = Vector2.zero;
            trs.anchorMax = Vector2.zero;

            _icons = new Dictionary<string, RectTransform>();
        }

        /// <summary>
        /// 添加小图标
        /// </summary>
        /// <param name="name"></param>
        /// <param name="icon"></param>
        public void AddIcon(string name, RectTransform icon)
        {
            icon.SetParent(rectTransform, false);
            icon.name = name;
            _icons[name] = icon;
        }

        /// <summary>
        /// 获取小图标
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RectTransform GetIcon(string name)
        {
            if (!_icons.ContainsKey(name))
            {
                return null;
            }
            return _icons[name];
        }

        /// <summary>
        /// 移除小图标
        /// </summary>
        /// <param name="name"></param>
        public void RemoveIcon(string name)
        {
            if (!_icons.ContainsKey(name))
            {
                return;
            }
            var icon = _icons[name];
            if (icon != null && icon.gameObject != null)
            {
                Object.Destroy(icon.gameObject);
            }
            _icons.Remove(name);
        }

        /// <summary>
        /// 移除所有小图标
        /// </summary>
        public void ClearIcons()
        {
            var icons = new List<RectTransform>(_icons.Values);
            icons.ForEach(icon =>
            {
                if (icon != null && icon.gameObject != null)
                {
                    Object.Destroy(icon.gameObject);
                }
            });
            _icons.Clear();
        }

        /// <summary>
        /// 设置光效显示状态
        /// </summary>
        public void SetLightEffectActive(bool active)
        {
            _lightEf.SetActive(active);
        }


        #region Inherit
        public override void SetUserInfo(uint userId, int gender, string avatarUrl)
        {
            this.userId = userId;

            var key = gender == 1 ? "common/avatar/male.png" : "common/avatar/female.png";
            _remoteImage.sprite = AssetsManager.Load<Sprite>("Common/Game/game_ui_atlas", key);
            _remoteImage.url = avatarUrl;
        }

        public override void ShowBankruptcy(bool animated = true)
        {
            DOTween.Kill(_bankcrupcyImage.gameObject);

            _bankcrupcyImage.gameObject.SetActive(true);
            var trs = _bankcrupcyImage.transform;
            trs.localScale = Vector3.one;

            if (animated)
            {
                trs.localScale = Vector2.one * 2f;
                trs.DOScale(Vector2.one, 0.2f).SetEase(Ease.InExpo);
            }
        }

        public override void HideBankruptcy()
        {
            _bankcrupcyImage.gameObject.SetActive(false);
        }

        public override void ShowGpsWarning(GPSWarning.Type type)
        {

        }

        public override void HideGpsWarning()
        {

        }

        public override void ShowOffline()
        {

        }

        public override void ShowOfflineCountdown(int countdown)
        {

        }

        public override void ShowOfflineTime(int time)
        {

        }

        public override void HideOffline()
        {

        }

        public override void ShowReady()
        {
            _readyImage.gameObject.SetActive(true);
            var trs = _readyImage.transform as RectTransform;
            var pos = viewChairId == 1 || viewChairId == 2 ? new Vector2(-120, 10) : new Vector2(120, 10);
            trs.anchoredPosition = pos;
        }

        public override void HideReady()
        {
            _readyImage.gameObject.SetActive(false);
        }

        public override void SetScore(string text)
        {
            _scoreText.gameObject.SetActive(true);
            _scoreText.text = text;
        }

        public override void HideScore()
        {
            _scoreText.gameObject.SetActive(false);
        }

        public override void ShowTrust()
        {
            _trustImage.gameObject.SetActive(true);
            _remoteImage.gameObject.SetActive(false);
        }

        public override void HideTrust()
        {
            _trustImage.gameObject.SetActive(false);
            _remoteImage.gameObject.SetActive(true);
        }

        public override void Reset()
        {
            base.Reset();

            ClearIcons();

            SetLightEffectActive(false);
        }
        #endregion
    }
}

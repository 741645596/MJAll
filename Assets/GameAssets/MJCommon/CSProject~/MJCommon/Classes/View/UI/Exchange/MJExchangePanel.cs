// @Author: tanjinhua
// @Date: 2021/10/20  9:02

using System;
using UnityEngine;
using Unity.Widget;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MJCommon
{
    /// <summary>
    /// 麻将换牌UI界面
    /// </summary>
    public class MJExchangePanel : WLayer
    {

        public Action onClickConfirm;

        private Dictionary<int, GameObject> _selectingInfos;
        private GameObject _changingInfo;
        private Transform _confirmButton;
        private SKText _tipText;

        public MJExchangePanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "exchange_card_panel.prefab");

            _changingInfo = FindInChildren("info_changing").gameObject;
            _selectingInfos = new Dictionary<int, GameObject>();
            for (int i = 0; i < 4; i++)
            {
                _selectingInfos.Add(i, FindInChildren($"info_selecting_{i}").gameObject);
            }

            _confirmButton = FindInChildren("hptishi_l_01_01");
            FindInChildren<Button>("hptishi_l_01_01/hp/btn_confirm").onClick.AddListener(() => onClickConfirm?.Invoke());

            _tipText = FindInChildren<SKText>("info_changing/zi/tip_txt");
        }

        public void ShowInfoChanging(ExchangeCardType type)
        {
            _changingInfo.SetActive(true);

            var txt = new string[]
            {
                "本局<color=#C86E3C>顺时针</color>换牌",
                "本局<color=#C86E3C>逆时针</color>换牌",
                "本局<color=#C86E3C>对家</color>换牌",
            }[(int)type];

            _tipText.text = txt;
        }

        public void HideInfoChanging()
        {
            _changingInfo.SetActive(false);
        }

        public void ShowInfoSelecting(int viewChairId, Vector2 pos)
        {
            var info = _selectingInfos[viewChairId];
            info.SetActive(true);
            var trs = info.transform as RectTransform;
            trs.SetPositionInZero(pos);
        }

        public void HideInfoSelecting(int viewChairId)
        {
            _selectingInfos[viewChairId].SetActive(false);
        }

        public void HideInfoSelecting()
        {
            foreach(var pair in _selectingInfos)
            {
                pair.Value.gameObject.SetActive(false);
            }
        }

        public void SetConfirmButtonActive(bool active)
        {
            _confirmButton.gameObject.SetActive(active);
        }
    }
}

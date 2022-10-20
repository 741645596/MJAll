// @Author: tanjinhua
// @Date: 2021/4/13  9:54


using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJActionButton : WNode
    {
        /// <summary>
        /// 根据showType类型获取通用默认配置
        /// </summary>
        /// <param name="showType"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> GetDefaultConfig(int showType)
        {
            switch(showType)
            {
                case ActionShowType.Peng:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_anniu1", "peng_h_01_01.prefab");

                case ActionShowType.Gang:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_anniu1", "gang_h_01_01.prefab");

                case ActionShowType.Hu:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_anniu1", "hu_h_01_01.prefab");

                case ActionShowType.Guo:
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_ui_effe_anniu1", "guo_h_01_01.prefab");

                default:
                    WLDebug.LogWarning($"MJActionButton.GetDefaultConfig: 通用不支持showyType = {showType}的动作按钮");
                    return new KeyValuePair<string, string>("MJCommon/MJ/mj_tmp", "tmp_button.prefab");
            }
        }


        public Action<List<MJActionData>> onClick;
        public List<MJActionData> actionDatas;
        public string identifier { get; private set; }

        private MJReplayActionButtonSelectMark _mark;

        public MJActionButton(string asset, string key, List<MJActionData> actionDatas)
        {
            InitGameObject(asset, key);

            identifier = asset + key;

            this.actionDatas = actionDatas;

            var button = gameObject.GetComponent<PressButton>();

            button.onClick.AddListener(() =>
            {
                if (this.actionDatas == null)
                {
                    WLDebug.LogWarning("MJActionButton.onClick: 动作数据为空");
                    return;
                }

                onClick?.Invoke(this.actionDatas);
            });
        }

        public void ShowReplaySelectMark()
        {
            if (_mark == null)
            {
                _mark = new MJReplayActionButtonSelectMark();
                _mark.rectTransform.SetParent(rectTransform, false);
                _mark.rectTransform.Layout(new Vector2(0.5f, 0.5f), new Vector2(0, 120));
            }
            _mark.gameObject.SetActive(true);
            _mark.Animate();
        }

        public void HideReplaySelectMark()
        {
            if (_mark == null)
            {
                return;
            }

            DOTween.Kill(_mark.gameObject);
            _mark.gameObject.SetActive(false);
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/4/20  9:51

using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using Unity.Core;

namespace MJCommon
{
    /// <summary>
    /// 胡牌提示节点
    /// </summary>
    public class MJHintCell : WNode
    {
        public struct Config
        {
            public int cardValue;
            public string firstInfo;
            public string secondInfo;
        }


        private Image _flower;
        private SKText _des1;
        private SKText _des2;
        private GameObject _slice;

        public MJHintCell(Config config, bool sliceVisible = true)
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "hint_cell.prefab");

            _flower = FindInChildren<Image>("card/flower");
            _des1 = FindInChildren<SKText>("des_group/des_1");
            _des2 = FindInChildren<SKText>("des_group/des_2");
            _slice = FindInChildren("slice").gameObject;

            SetFlower(config.cardValue);
            SetFirstInfo(config.firstInfo);
            SetSecondInfo(config.secondInfo);
            _slice.SetActive(sliceVisible);
        }


        public virtual void SetFlower(int cardValue)
        {
            _flower.sprite = AssetsManager.Load<Sprite>("MJCommon/MJ/mj_ui_atlas", $"card_2d/flower/hand_card_{cardValue}.png");

            _flower.SetNativeSize();
        }


        public virtual void SetFirstInfo(string info)
        {
            _des1.text = info;

            if (info == "0张")
            {
                if (ColorUtility.TryParseHtmlString("#6a6969", out Color color))
                {
                    _des1.color = color;
                }
            }
        }


        public virtual void SetSecondInfo(string info)
        {
            _des2.gameObject.SetActive(!string.IsNullOrEmpty(info));

            _des2.text = info;

            if (info == "0张")
            {
                if (ColorUtility.TryParseHtmlString("#6a6969", out Color color))
                {
                    _des2.color = color;
                }
            }
        }
    }
}

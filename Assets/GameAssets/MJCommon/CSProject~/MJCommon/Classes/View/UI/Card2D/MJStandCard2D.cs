// @Author: tanjinhua
// @Date: 2021/9/6  10:17

using Unity.Core;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJStandCard2D : MJCard2D
    {

        public const float DimensionX = 99;
        public const float DimensionY = 144;

        public MJStandCard2D(int value) : base(value)
        {
        }

        protected override void OnInit()
        {
            var key = cardValue == Card.Rear ?
                "stand_card_back_2d.prefab" :
                "stand_card_2d.prefab";

            InitGameObject("MJCommon/MJ/mj_ui_prefabs", key);

            Image flower = FindReference<Image>("flower");

            var flowerKey = $"card_2d/flower/hand_card_{cardValue}.png";

            flower.sprite = AssetsManager.Load<Sprite>("MJCommon/MJ/mj_ui_atlas", flowerKey);

            flower.SetNativeSize();
        }
    }
}

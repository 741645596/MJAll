// @Author: tanjinhua
// @Date: 2021/5/10  14:20

using UnityEngine;
using Unity.Widget;

namespace MJCommon
{
    public class MJBlankEndAnimation : WNode
    {
        public MJBlankEndAnimation(string asset, string imagePath)
        {
            gameObject.name = "blankend_anim";

            WSprite bg = WSprite.CreateScale9Sprite("MJCommon/MJ/mj_ui_atlas", "common_use/tips_bg.png");

            bg.AddTo(this.gameObject);

            bg.Layout(cc.p(0.5f, 0.5f), cc.p(0, 50));

            bg.SetContentSize(new Vector2(display.width, 130));

            bg.SetScaleY(0.01f);

            CCAction.NewSequence().ScaleTo(0.3f, 1).DelayTime(0.3f).FadeOut(0.3f).Run(bg);

            var txt = WSprite.Create(asset, imagePath);

            txt.AddTo(gameObject);

            txt.Layout(cc.p(0.5f, 0.5f), cc.p(0, 50));

            txt.SetScale(2f);

            CCAction.NewEaseExponentialIn().ScaleTo(0.3f, 1f).Run(txt);
        }
    }
}

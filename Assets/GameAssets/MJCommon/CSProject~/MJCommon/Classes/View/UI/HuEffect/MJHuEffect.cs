// @Author: tanjinhua
// @Date: 2021/9/8  13:44

using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJHuEffect : WNode
    {
        public const float ImageScale = 0.7f;

        private List<WSprite> _texts;
        private CanvasGroup _canvasGroup;

        public MJHuEffect(string asset, List<string> textImageKeys)
        {
            var obj = new GameObject("hu_effect", typeof(RectTransform));

            InitGameObject(obj);

            rectTransform.sizeDelta = Vector2.zero;

            _texts = new List<WSprite>();

            _canvasGroup = gameObject.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0;

            SetAnchor(cc.p(0.5f, 0.5f));

            Init(asset, textImageKeys);
        }

        public void Init(string asset, List<string> keys)
        {
            _texts.ForEach(sp => Object.Destroy(sp.gameObject));
            _texts.Clear();

            float w = 0, h = 0, spacing = -20 * ImageScale;
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i];
                var sp = WSprite.Create(asset, k);
                var trs = sp.gameObject.transform as RectTransform;
                trs.SetParent(gameObject.transform);
                trs.anchorMin = new Vector2(0, 0.5f);
                trs.anchorMax = new Vector2(0, 0.5f);
                trs.pivot = new Vector2(0, 0.5f);
                trs.anchoredPosition = new Vector2(w, 0);

                var size = trs.rect.size;
                w += size.x + (i == keys.Count - 1 ? 0 : spacing);
                h = size.y > h ? size.y : h;
            }

            SetContentSize(w, h);
        }

        public void PlayShowyAnim()
        {
            //var ef = WSpineNode.Create("MJCommon/MJ/mj_ui_spine",
            //    "hu_effect/showy/ef_tishi_zijihu_Material.mat",
            //    "hu_effect/showy/ef_tishi_zijihu_SkeletonData.asset");
            //ef.AddTo(gameObject).Layout(cc.p(0.5f, 0.5f));
            //ef.rectTransform.SetAsFirstSibling();
            //ef.SetAnimation(0, "animation", false);

            //_canvasGroup.alpha = 0;

            //float frameRate = 1f / 30f, scale = 1f / ImageScale;

            //var seq = DOTween.Sequence();
            //seq.AppendInterval(frameRate * 3);

            //var appear = DOTween.Sequence();
            //appear.Join(_canvasGroup.DOFade(0.55f, frameRate));
            //appear.Join(rectTransform.DOScale(0.8f * scale, frameRate));
            //seq.Append(appear);

            //var zoomIn = DOTween.Sequence();
            //zoomIn.Join(_canvasGroup.DOFade(1, frameRate));
            //zoomIn.Join(rectTransform.DOScale(1.4f * scale, frameRate));
            //seq.Append(zoomIn);

            //seq.Append(rectTransform.DOScale(0.9f * scale, frameRate * 3));
            //seq.Append(rectTransform.DOScale(1 * scale, frameRate * 4));
            //seq.AppendInterval(frameRate * 21);
            //seq.Append(_canvasGroup.DOFade(0, frameRate * 7));
            //seq.AppendCallback(() => RemoveFromParent());

            //seq.SetTarget(gameObject);
            //seq.SetLink(gameObject);
            //seq.SetAutoKill(true);
            //seq.Play();
        }

        public void PlaySimpleAnim()
        {
            //float frameRate = 1f / 30f, scale = 1f / ImageScale;

            //SetScale(0.7f * scale);
            //_canvasGroup.alpha = 0;

            //var seq = DOTween.Sequence();
            //seq.Append(_canvasGroup.DOFade(1, frameRate * 5));
            //seq.AppendInterval(frameRate * 25);
            //seq.Append(_canvasGroup.DOFade(0, frameRate * 5));
            //seq.AppendCallback(() => RemoveFromParent());

            //seq.SetTarget(gameObject);
            //seq.SetLink(gameObject);
            //seq.SetAutoKill(true);
            //seq.Play();

            //var ef = WSpineNode.Create("MJCommon/MJ/mj_ui_spine",
            //    "hu_effect/simple/ef_tishi_hu_Material.mat",
            //    "hu_effect/simple/ef_tishi_hu_SkeletonData.asset");
            //ef.AddTo(gameObject).Layout(cc.p(0.5f, 0.5f));
            //ef.SetAnimation(0, "animation", false);
        }
    }
}

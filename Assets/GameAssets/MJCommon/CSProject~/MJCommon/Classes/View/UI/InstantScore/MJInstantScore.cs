// @Author: tanjinhua
// @Date: 2021/9/8  17:42

using DG.Tweening;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJInstantScore : WNode
    {
        private long _score;
        private SKText _text;
        private Transform _effectTrs;

        public MJInstantScore(long score)
        {
            _score = score;
            
            var key = score >= 0 ? "instant_score_p.prefab" : "instant_score_n.prefab";
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", key);

            var sign = score >= 0 ? "+" : "-";
            var abs = Mathf.Abs(score);
            var str = sign + (abs > 100000 ? (Mathf.RoundToInt(abs / 1000f) / 10f).ToString() + "万" : abs.ToString());
            _text = FindInChildren<SKText>("text");
            _text.onUpdateSize += () => rectTransform.SetContentSizeInZero(_text.rectTransform.GetContentSizeInZero());
            _text.text = str;

            _effectTrs = FindInChildren("effect");

        }


        public void Animate()
        {
            if (_score >= 0)
            {
                PlayPositiveAnim();
            }
            else
            {
                PlayNegativeAnim();
            }
        }

        private void PlayPositiveAnim()
        {
            _text.color = new Color(1, 1, 1, 0);
            var textTrs = _text.rectTransform;
            textTrs.anchoredPosition = new Vector2(0, -100);
            var seq = DOTween.Sequence();
            seq.SetDelay(0f);
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, 10), 0.2f));
            seq.Join(_text.DOFade(1, 0.2f));
            seq.AppendCallback(() => _effectTrs.gameObject.SetActive(true));
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, -5), 0.1f));
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, 0), 0.05f));
            seq.AppendInterval(0.8f);
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, 50), 0.5f));
            seq.Join(_text.DOFade(0, 0.5f));
            seq.AppendInterval(2f);
            seq.AppendCallback(() => RemoveFromParent());
            seq.SetTarget(gameObject);
            seq.SetLink(gameObject);
            seq.Play();
        }

        private void PlayNegativeAnim()
        {
            _text.color = new Color(1, 1, 1, 0);
            var textTrs = _text.rectTransform;
            textTrs.anchoredPosition = new Vector2(0, 100);
            var seq = DOTween.Sequence();
            seq.SetDelay(0f);
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, -10), 0.2f));
            seq.Join(_text.DOFade(1, 0.2f));
            seq.AppendCallback(() => _effectTrs.gameObject.SetActive(true));
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, 5), 0.1f));
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, 0), 0.05f));
            seq.AppendInterval(0.8f);
            seq.Append(textTrs.DOAnchorPos(new Vector2(0, -50), 0.5f));
            seq.Join(_text.DOFade(0, 0.5f));
            seq.AppendInterval(2f);
            seq.AppendCallback(() => RemoveFromParent());
            seq.SetTarget(gameObject);
            seq.SetLink(gameObject);
            seq.Play();
        }
    }
}

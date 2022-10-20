// @Author: tanjinhua
// @Date: 2021/12/20  17:35


using Unity.Widget;
using DG.Tweening;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJReplayActionButtonSelectMark : WNode
    {
        private Image _image;

        public MJReplayActionButtonSelectMark()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "replay_action_button_select_mark.prefab");

            _image = gameObject.GetComponent<Image>();
        }

        public void Animate()
        {
            var seq = DOTween.Sequence();
            seq.AppendInterval(0.4f);
            seq.AppendCallback(() => _image.enabled = true);
            seq.Append(rectTransform.DOScale(1.2f, 0.1f));
            seq.Append(rectTransform.DOScale(0.8f, 0.1f));
            seq.Append(rectTransform.DOScale(1, 0.1f));
            seq.SetTarget(gameObject);
            seq.SetLink(gameObject);
            seq.Play();
        }
    }
}

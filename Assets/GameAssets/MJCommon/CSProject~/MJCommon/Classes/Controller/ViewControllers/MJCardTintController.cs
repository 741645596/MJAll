// @Author: tanjinhua
// @Date: 2021/5/9  23:50


using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    public class MJCardTintController : BaseGameController
    {
        /// <summary>
        /// 高亮颜色
        /// </summary>
        protected virtual Color32 _highlightTint => new Color32(170, 232, 251, 255);

        private MJDeskCardController _deskCardController;
        private MJMeldController _meldController;
        private MJWinCardController _winCardController;

        public override void OnSceneLoaded()
        {
            _deskCardController = stage.GetController<MJDeskCardController>();
            _meldController = stage.GetController<MJMeldController>();
            _winCardController = stage.GetController<MJWinCardController>();
        }


        /// <summary>
        /// 根据传入牌值高亮牌
        /// </summary>
        /// <param name="cardValue"></param>
        public virtual void Highlight(int cardValue)
        {
            _deskCardController.TintByValue(_highlightTint, cardValue);

            _meldController.TintByValue(_highlightTint, cardValue);

            _winCardController.TintByValue(_highlightTint, cardValue);
        }


        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            _deskCardController.ResetTint();

            _meldController.ResetTint();

            _winCardController.ResetTint();
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/12/17  16:44


using Common;
using DG.Tweening;
using WLCore;

namespace MJCommon
{
    public class MJOutCardRecord : BaseRecord
    {
        private int _chairId;
        private int _cardValue;
        private int _viewChairId;

        private MJGameData _gameData;
        private MJGamePlayer _player;
        private MJHandController _handController;
        private MJDeskCardController _deskCardController;

        private Sequence _discardAnim;
        private Sequence _insertAnim;

        public override void OnInitialize()
        {
            _gameData = stage.gameData as MJGameData;
            _handController = stage.GetController<MJHandController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
        }

        public override void Read(MsgHeader msg)
        {
            _cardValue = msg.ReadByte();
            _chairId = msg.ReadByte();
            _viewChairId = stage.ToViewChairId(_chairId);
            _player = _gameData.GetPlayerByChairId(_chairId) as MJGamePlayer;
        }

        public override float Execute()
        {
            // 更新数据
            _player.RemoveHandCardValue(_cardValue);
            _player.AddOutCardValue(_cardValue);

            // 播放动画
            var card = _handController.FindCardByValue(_viewChairId, _cardValue, true);
            if (card != null)
            {
                _discardAnim = _deskCardController.PlayDiscard(_viewChairId, _cardValue, card);

                var handSet = _handController.GetHandSet(_viewChairId);
                var isLastCard = handSet.IndexOf(card) == handSet.count - 1;
                _handController.RemoveCard(_viewChairId, card);
                if (!isLastCard)
                {
                    _insertAnim = _handController.PlayInsert(_viewChairId);
                }
            }

            return 0.3f;
        }

        public override void Undo()
        {
            // 恢复数据
            _player.AddHandCardValue(_cardValue);
            _player.RemoveLastOutCardValue();

            // 刷新手牌、出牌
            _discardAnim?.Kill();
            _insertAnim?.Kill();

            var handSet = _handController.GetHandSet(_viewChairId);
            handSet.Append(_cardValue);
            handSet.Sort();

            _deskCardController.Pop(_viewChairId);
        }

        public override void Abort()
        {
            _discardAnim?.Kill();
            _insertAnim?.Kill();

            _handController.GetHandSet(_viewChairId).Sort();
            _deskCardController.GetDeskCardSet(_viewChairId).Refresh();
        }
    }
}

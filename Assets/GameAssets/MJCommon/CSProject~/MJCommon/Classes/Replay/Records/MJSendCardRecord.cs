// @Author: tanjinhua
// @Date: 2021/12/18  16:50


using Common;
using DG.Tweening;
using WLCore;

namespace MJCommon
{
    public class MJSendCardRecord : BaseRecord
    {
        private int _cardValue;
        private int _chairId;
        private bool _isKongCard;
        private int _viewChairId;

        private MJGameData _gameData;
        private MJGamePlayer _player;
        private MJHandController _handController;
        private MJWallController _wallController;
        private MJActionButtonController _actionButtonController;

        private Sequence _drawAnim;

        public override void OnInitialize()
        {
            _gameData = stage.gameData as MJGameData;
            _handController = stage.GetController<MJHandController>();
            _wallController = stage.GetController<MJWallController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
        }

        public override void Read(MsgHeader msg)
        {
            _cardValue = msg.ReadByte();
            _chairId = msg.ReadByte();
            _isKongCard = msg.ReadByte() == 1;
            _viewChairId = stage.ToViewChairId(_chairId);
            _player = _gameData.GetPlayerByChairId(_chairId) as MJGamePlayer;
        }

        public override float Execute()
        {
            // 更新数据
            _player.AddHandCardValue(_cardValue);
            if (_isKongCard)
            {
                _gameData.sendedKongCardCount++;
            }
            else
            {
                _gameData.sendedCardCount++;
            }

            // 播放发牌动画、更新牌墙
            _actionButtonController.ClearButtons();
            _drawAnim = _handController.PlayDraw(_viewChairId, _cardValue);
            _wallController.Take(_isKongCard);

            // TODO: 更新剩余牌数

            return 0.3f;
        }

        public override void Undo()
        {
            // 恢复数据
            _player.RemoveHandCardValue(_cardValue, true);
            if (_isKongCard)
            {
                _gameData.sendedKongCardCount--;
            }
            else
            {
                _gameData.sendedCardCount--;
            }

            // 恢复手牌、牌墙
            var card = _handController.FindCardByValue(_viewChairId, _cardValue);
            _handController.RemoveCard(_viewChairId, card);
            _handController.GetHandSet(_viewChairId).Sort();
            _wallController.Recover(_isKongCard);
        }

        public override void Abort()
        {
            _drawAnim?.Kill();

            _handController.GetHandSet(_viewChairId).Sort();
        }
    }
}

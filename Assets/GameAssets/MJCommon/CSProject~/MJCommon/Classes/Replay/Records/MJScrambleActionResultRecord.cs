// @Author: tanjinhua
// @Date: 2021/12/23  10:38


using WLCore;

namespace MJCommon
{
    public class MJScrambleActionResultRecord : MJActionResultRecord
    {
        protected int _scrambledChairId;
        protected int _scrambledViewChairId;
        protected int _scrambledFuziIndex;
        protected FuziData _scrambledFuziData;
        protected FuziData _fuziDataBeforeScrambled; // 用于恢复

        protected MJGamePlayer _scrambledPlayer;

        public override void Read(MsgHeader msg)
        {
            base.Read(msg);

            _scrambledChairId = msg.ReadByte();
            _scrambledFuziIndex = msg.ReadByte();
            _scrambledFuziData = FuziData.From(msg);

            _scrambledViewChairId = stage.ToViewChairId(_scrambledChairId);
            _scrambledPlayer = _gameData.GetPlayerByChairId<MJGamePlayer>(_scrambledChairId);
        }


        protected override void UpdateFuziData(bool isUndo)
        {
            base.UpdateFuziData(isUndo);

            if (_scrambledFuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            if (isUndo)
            {
                _scrambledPlayer.ReplaceFuziData(_scrambledFuziIndex, _fuziDataBeforeScrambled);
            }
            else
            {
                _fuziDataBeforeScrambled = _scrambledPlayer.fuziDatas[_scrambledFuziIndex];
                _scrambledPlayer.ReplaceFuziData(_scrambledFuziIndex, _scrambledFuziData);
            }
        }

        protected override void UpdateMeldStack(bool isAbort, bool isUndo)
        {
            base.UpdateMeldStack(isAbort, isUndo);

            if (_scrambledFuziIndex == MJDefine.InvaildFuziIndex)
            {
                return;
            }

            var fuziData = _scrambledPlayer.fuziDatas[_scrambledFuziIndex];
            var arg = _meldController.ToMeldData(_scrambledChairId, fuziData);
            _meldController.GetMeldStack(_scrambledViewChairId).Replace(_scrambledFuziIndex, arg);
        }
    }
}

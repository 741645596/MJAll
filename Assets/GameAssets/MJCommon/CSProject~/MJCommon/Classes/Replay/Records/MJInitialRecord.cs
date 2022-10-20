// @Author: tanjinhua
// @Date: 2021/12/15  16:23


using Common;
using WLCore;
using WLHall;
using System.Collections;

namespace MJCommon
{
    /// <summary>
    /// 初始化记录，新服务器可继承此类
    /// </summary>
    public class MJInitialRecord : InitialRecord
    {
        private MJGameData _gameData;
        private MJHandController _handController;
        private MJMeldController _meldController;
        private MJWallController _wallController;
        private MJAvatarController _avatarController;
        private MJSettleController _settleController;
        private MJWinCardController _winCardController;
        private MJDeskCardController _deskCardController;
        private MJActionButtonController _actionButtonController;
        private MJActionEffectController _actionEffectController;

        public override void OnInitialize()
        {
            base.OnInitialize();

            _gameData = stage.gameData as MJGameData;
            _handController = stage.GetController<MJHandController>();
            _meldController = stage.GetController<MJMeldController>();
            _wallController = stage.GetController<MJWallController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _settleController = stage.GetController<MJSettleController>();
            _winCardController = stage.GetController<MJWinCardController>();
            _deskCardController = stage.GetController<MJDeskCardController>();
            _actionButtonController = stage.GetController<MJActionButtonController>();
            _actionEffectController = stage.GetController<MJActionEffectController>();
        }

        public override void Read(MsgHeader msg)
        {
            // 由于服务器没有返回状态码，无法判断数据是否读取成功，
            // 如果错误，服务器会返回json提示信息
            // 这里做强制判断，如果玩家数量超过5人，则判断数据读取失败！
            var defineValue = msg.ReadByte();
            if (defineValue > 4)
            {
                throw new System.Exception("MJInitialRecord.Read: 数据错误，请检查服务器逻辑");
            }

            var msgFormat = new ArrayList
            {
                new ArrayList{ "BYTE", "byPlayerCount" },
                new ArrayList{ "BYTE", "byAllCardCount" },
                new ArrayList{ "BYTE", "byBankerChairId" },
                new ArrayList{ "BYTE", "byAllPlayCount" },
                new ArrayList{ "BYTE", "byCurPlayCount" },
                new ArrayList{ "Card", "byHand", 4, 14 },
                new ArrayList{ "DWORD", "dwUserId", 4 },
                new ArrayList{ "LONGLONG", "llCarryScores", 4 },
                new ArrayList{ "BYTE", "bSex", 4 },
                new ArrayList{ "DWORD", "dwAvatarID", 4 },
                new ArrayList{ "CHAR", "szAvatarImgPath", 4, 128 },
                new ArrayList{ "TCHAR", "szNickName", 4, 36 },
            };

            var data = MsgParser.Parse(msg, msgFormat);

            InitializeGameData(data);
        }

        protected virtual void InitializeGameData(MsgData data)
        {
            _gameData.Initialize();
            _gameData.bankerChairId = data.GetByte("byBankerChairId");
            _gameData.friendGameInfo = new FriendGameInfo
            {
                totalGameCount = data.GetByte("byAllPlayCount"),
                currentGameCount = data.GetByte("byCurPlayCount"),
            };

            var allHandValues = data.GetCrossList<int>("byHand");
            var allScores = data.GetList<long>("llCarryScores");
            var allUserIds = data.GetList<uint>("dwUserId");
            var allGenders = data.GetList<int>("bSex");
            var allAvatarIds = data.GetList<uint>("dwAvatarID");
            var allAvatarUrls = data.GetCharStringArray("szAvatarImgPath");
            var allNicknames = data.GetTCharStringArray("szNickName");
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.Initialize();

                var index = player.chairId;
                var handValues = allHandValues[index];
                handValues = handValues.Fetch(value => value != Card.Invalid);
                player.ReloadHandCardValues(handValues);
                _gameData.sendedCardCount += handValues.Count;

                player.friendGameScore = allScores[index];
                player.userInfo.id = allUserIds[index];
                player.userInfo.sex = allGenders[index];
                player.userInfo.avatarId = allAvatarIds[index];
                player.userInfo.avatarPath = allAvatarUrls[index];
                player.userInfo.nickName = allNicknames[index];
            });
        }


        public override float Execute()
        {
            base.Execute();

            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                var viewChairId = stage.ToViewChairId(player.chairId);

                _handController.Reload(viewChairId, player.handCardValues);

                _handController.Sort(viewChairId);

                _meldController.Reload(viewChairId, _meldController.ToMeldDatas(player.chairId, player.fuziDatas));

                _winCardController.Reload(viewChairId, player.winCardValues);

                _deskCardController.Reload(viewChairId, player.outCardValues);
            });

            _wallController.InitializeWalls();

            _wallController.TakeMultiple(_gameData.sendedCardCount);

            _avatarController.ShowBankerIcon(stage.ToViewChairId(_gameData.bankerChairId));

            _actionButtonController.ClearButtons();

            _actionEffectController.Clear();

            _deskCardController.HidePointer();

            _settleController.RemoveSettleView();

            return 0f;
        }


        public override void Undo()
        {
        }


        public override void Abort()
        {
        }
    }
}

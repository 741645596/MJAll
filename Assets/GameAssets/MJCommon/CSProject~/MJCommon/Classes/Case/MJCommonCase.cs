// @Author: tanjinhua
// @Date: 2021/9/3  18:24


using System.Collections.Generic;
using Unity.Core;
using Unity.Widget;
using UnityEngine;
using WLCommon;
using WLCore;
using WLCore.Stage;
using WLHall;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 麻将业务测试用例
    /// </summary>
    public class MJCommonCase : BaseCaseStage
    {

        private MJCaseStage _gameStage;
        private MJGameData _gameData => _gameStage.gameData as MJGameData;


        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();

            AdaptationLogic.Init();

            AudioMgr.Init();

            _gameStage = StageManager.RunStage("MJCommon.MJCaseStage", new MJCaseRoomManager()) as MJCaseStage;
        }


        public void CaseReload()
        {
            Clear();

            int takeCardCount = 0;

            _gameData.TraversePlayer<MJGamePlayer>(p =>
            {
                var fuziCount = 0;
                var handCount = _gameData.maxHandCardCount - fuziCount * 3;
                var handValues = p.chairId == _gameData.bankerChairId ?
                    MockUtils.MockRandomCardValues(handCount) :
                    MockUtils.MockRandomCardValues(handCount);
                p.ReloadHandCardValues(handValues);

                var fuziDatas = MockUtils.MockRandomFuziDatas(fuziCount, _gameData.maxPlayerCount);
                p.ReloadFuziDatas(fuziDatas);

                var deskCards = MockUtils.MockRandomCardValues(21);
                p.ReloadOutCardValues(deskCards);
                takeCardCount += deskCards.Count;

                var winCards = MockUtils.MockRandomCardValues(4);
                p.ReloadWinCardValues(winCards);

                int viewChairId = _gameStage.ToViewChairId(p.chairId);
                _gameStage.GetController<MJHandController>().Reload(viewChairId, handValues);
                _gameStage.GetController<MJMeldController>().Reload(viewChairId, _gameStage.GetController<MJMeldController>().ToMeldDatas(p.chairId, fuziDatas));
                _gameStage.GetController<MJDeskCardController>().Reload(viewChairId, deskCards);
                _gameStage.GetController<MJWinCardController>().Reload(viewChairId, winCards);
            });

            _gameStage.GetController<MJWallController>().InitializeWalls();
            _gameStage.GetController<MJWallController>().TakeMultiple(takeCardCount);

            _gameStage.GetController<MJHandController>().GetOwnHand().Sort();
            _gameStage.GetController<MJHandController>().GetOwnHand().interactable = true;
        }


        public void CasePlayGameStart()
        {
            Clear();

            _gameData.diceValues = new List<int>
            {
                Random.Range(1, 7),
                Random.Range(1, 7)
            };

            _gameData.verifyingOutCardValue = Card.Invalid;

            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                var handCount = player.chairId == _gameData.bankerChairId ? _gameData.maxHandCardCount : _gameData.maxHandCardCount - 1;
                player.ReloadHandCardValues(MockUtils.MockRandomCardValues(handCount));
                player.ClearOutCardValues();
                player.ClearFuziDatas();
                player.ClearWinCardValues();
            });

            _gameStage.GetController<MJTableController>().RemoveMatchingAnimation();

            _gameStage.GetController<MJGameStartHandler>().EnqueueGameStart();

            _gameStage.GetController<MJGameStartHandler>().EnqueueBankerTimer();
        }


        public void CaseExchangeStart()
        {
            _gameStage.GetController<MJExchangeHandler>().EnqueueExchangeStart();
        }


        public void CaseExchangeResult()
        {
            var type = (ExchangeCardType)Random.Range(0, 3);
            _gameStage.GetController<MJExchangeHandler>().EnqueueExchangeResult(type, new List<int> { 1, 2, 3 });
        }

        public void CaseDingqueStart()
        {
            _gameStage.GetController<MJDingqueHandler>().EnqueueDingqueStart();
        }


        public void CaseDingqueResult()
        {
            var selfDingque = (_gameData.playerSelf as MJGamePlayer).dingqueColorValue;
            selfDingque = selfDingque == Card.ColorNone ? 0 : selfDingque;
            var suitValues = new int[4];
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                if (player == _gameData.playerSelf)
                {
                    suitValues[player.chairId] = selfDingque;
                }
                else
                {
                    suitValues[player.chairId] = Random.Range(0, 3);
                }
            });
            _gameStage.GetController<MJDingqueHandler>().EnqueueDingqueResult(new List<int>(suitValues));
        }

        public void CaseDrawCard()
        {
            int cardValue = MockUtils.MockRandomCardValue();
            (_gameData.playerSelf as MJGamePlayer).AddHandCardValue(cardValue);
            _gameStage.GetController<MJHandController>().PlayDraw(Chair.Down, cardValue);
            _gameData.verifyingOutCardValue = Card.Invalid;
            _gameStage.GetController<MJWallController>().Take();
            _gameStage.GetController<MJTableController>().PlayTurn(Chair.Down, 15);
            _gameStage.GetController<MJAvatarController>().SwitchLightEffect(Chair.Down);
        }


        public void CaseExposedKong()
        {
            Clear();

            var playerSelf = _gameData.playerSelf as MJGamePlayer;
            playerSelf.ReloadHandCardValues(new List<int> { 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 5 });
            playerSelf.ClearFuziDatas();
            playerSelf.AddFuziDatas(new List<FuziData> { MockUtils.MockPongFuzi(5, 3) });

            _gameStage.GetController<MJHandController>().Reload(Chair.Down, playerSelf.handCardValues);
            _gameStage.GetController<MJMeldController>().Reload(Chair.Down, _gameStage.GetController<MJMeldController>().ToMeldDatas(playerSelf.chairId, playerSelf.fuziDatas));

            var data = new MsgActionResult
            {
                chairId = 0,
                showType = ActionShowType.Gang,
                toRemoveCardValues = new List<int> { 6, 6, 6 },
                fuziIndex = 1,
                fuziData = MockUtils.MockExposedKongFuzi(6, 1)
            };

            PlayActionResult(data);
        }


        public void CaseCompletementKong()
        {
            Clear();

            var playerSelf = _gameData.playerSelf as MJGamePlayer;
            playerSelf.ReloadHandCardValues(new List<int> { 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 5 });
            playerSelf.ClearFuziDatas();
            playerSelf.AddFuziDatas(new List<FuziData> { MockUtils.MockPongFuzi(5, 3) });

            _gameStage.GetController<MJHandController>().Reload(Chair.Down, playerSelf.handCardValues);
            _gameStage.GetController<MJMeldController>().Reload(Chair.Down, _gameStage.GetController<MJMeldController>().ToMeldDatas(playerSelf.chairId, playerSelf.fuziDatas));

            var data = new MsgActionResult
            {
                chairId = 0,
                showType = ActionShowType.Gang,
                toRemoveCardValues = new List<int> { 5 },
                fuziIndex = 0,
                fuziData = MockUtils.MockExposedKongFuzi(5, Chair.Invalid)
            };

            PlayActionResult(data);
        }


        public void CaseConcealedKong()
        {
            Clear();

            var playerSelf = _gameData.playerSelf as MJGamePlayer;
            playerSelf.ReloadHandCardValues(new List<int> { 6, 6, 6, 7, 7, 7, 8, 8, 8, 9, 7 });
            playerSelf.ClearFuziDatas();
            playerSelf.AddFuziDatas(new List<FuziData> { MockUtils.MockPongFuzi(5, 3) });

            _gameStage.GetController<MJHandController>().Reload(Chair.Down, playerSelf.handCardValues);
            _gameStage.GetController<MJMeldController>().Reload(Chair.Down, _gameStage.GetController<MJMeldController>().ToMeldDatas(playerSelf.chairId, playerSelf.fuziDatas));

            var data = new MsgActionResult
            {
                chairId = 0,
                showType = ActionShowType.Gang,
                toRemoveCardValues = new List<int> { 7, 7, 7, 7 },
                fuziIndex = 1,
                fuziData = MockUtils.MockConcealedKongFuzi(7, Chair.Invalid)
            };

            PlayActionResult(data);
        }


        public void CasePlayScoreAnim()
        {
            var scores = new List<long>
            {
                125000, -50000, -50000, -25000
            };

            _gameStage.GetController<MJGameOverHandler>().PlayInstantScoreAnim(scores);
        }


        public void CaseWinAnimation()
        {
            Clear();

            var endType = EndType.DianPao;
            ushort dianPaoChairId = Chair.Right;
            var huChairIds = new List<ushort> { 0 };
            var huTypes = new List<long> { 0 };
            var scores = new List<long> { 1500, -500, -500, -500 };
            var winCardValue = Card.Wan1;

            _gameStage.animationQueue.Enqueue(1.7f, () => _gameStage.GetController<MJWinCardController>().PlayWinCardAnimation(Chair.Down, winCardValue));

            _gameStage.animationQueue.Enqueue(0, () => _gameStage.GetController<MJEnvController>().PlayShake());

            _gameStage.GetController<MJGameOverHandler>().EnqueueActionEffects(endType, dianPaoChairId, huChairIds, huTypes);

            _gameStage.GetController<MJGameOverHandler>().EnqueueHuEffect(endType, dianPaoChairId, huChairIds, scores);
        }

        MJReplayActionButtonPanel _buttonPanel;
        public void CaseReplayActionButton()
        {
            if (_buttonPanel == null)
            {
                _buttonPanel = new MJReplayActionButtonPanel();
                _buttonPanel.AddTo(WDirector.GetRootLayer());
            }

            var showTypes = new List<int>
            {
                ActionShowType.Hu, ActionShowType.Gang, ActionShowType.Guo
            };
            var configs = new List<KeyValuePair<string, string>>();
            showTypes.ForEach(s => configs.Add(MJActionButton.GetDefaultConfig(s)));

            for (int i = 0; i < 4; i++)
            {
                if (i == 1 || i == 3)
                {
                    var offset = new Vector2(120 * (i - 2), 0);
                    _buttonPanel.GetContainer(i).SetPositionInZero(_gameStage.GetController<MJAvatarController>().GetAvatarPosition(i) + offset);
                }
                _buttonPanel.Show(i, configs, 0);
            }
        }


        private MJRTCardSet[] _rtCardSets;
        public void CaseRTCardSet()
        {
            if (_rtCardSets == null)
            {
                _rtCardSets = new MJRTCardSet[4];
                for (int i = 0; i < _rtCardSets.Length; i++)
                {
                    _rtCardSets[i] = new MJRTCardSet();
                    _rtCardSets[i].AddTo(WDirector.GetRootLayer());
                    _rtCardSets[i].rectTransform.SetPositionInZero(new Vector2(200, 200 + 200 * i));

                }
            }

            foreach(var set in _rtCardSets)
            {
                var fuziDatas = MockUtils.MockRandomFuziDatas(2);
                var meldArgs = _gameStage.GetController<MJMeldController>().ToMeldDatas(0, fuziDatas);
                var handValues = MockUtils.MockRandomCardValues(8);
                set.LoadData(meldArgs, handValues);
            }
        }

        private void PlayActionResult(MsgActionResult data)
        {
            _gameStage.GetController<MJActionResultHandler>().EnqueueActionEffect(data.chairId, data.showType, data.fuziData);
            _gameStage.GetController<MJActionResultHandler>().EnqueueUpdateActionPlayerHand(data.chairId, data.showType, data.toRemoveCardValues);
            _gameStage.GetController<MJActionResultHandler>().EnqueueUpdateActionPlayerMeld(data.chairId, data.showType, data.fuziIndex, data.fuziData);
            _gameStage.GetController<MJActionResultHandler>().EnqueueRefreshActionPlayerHand(data.chairId, data.showType, data.toRemoveCardValues);
        }


        private void Clear()
        {
            _gameStage.GetController<MJHandController>().Clear();
            _gameStage.GetController<MJDeskCardController>().Clear();
            _gameStage.GetController<MJMeldController>().Clear();
            _gameStage.GetController<MJWinCardController>().Clear();
            _gameStage.GetController<MJWallController>().Clear();
            _gameStage.GetController<MJTableController>().RemoveMatchingAnimation();
            _gameStage.GetController<MJGameStartHandler>().RemoveGameStartAnimation();
            _gameStage.GetController<MJEnvController>().ResetMainCamera();
            _gameStage.GetController<MJExchangeHandler>().RemoveExchangePanel();
            _gameStage.GetController<MJDingqueHandler>().RemoveDingquePanel();
            _gameData.TraversePlayer<MJGamePlayer>(p => p.dingqueColorValue = Card.ColorNone);
        }
    }


    public class MJCaseStage : MJStage
    {
        public bool isSceneLoaded = false;

        private MJCaseRoomManager _roomManager;

        public MJCaseStage(BaseRoomManager roomManager) : base(roomManager)
        {
            _roomManager = roomManager as MJCaseRoomManager;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            AddController<MJCaseHandController>(1);
        }

        protected override void OnSceneLoaded()
        {
            base.OnSceneLoaded();

            isSceneLoaded = true;

            for (ushort i = 0; i < gameData.maxPlayerCount; i++)
            {
                MJGamePlayer player = MockUtils.MockPlayer<MJGamePlayer>(i);

                gameData.SetPlayer(player);

                OnPlayerJoin(player, i == _roomManager.selfChairId);
            }
        }


        protected override BaseGameData OnCreateGameData() => new MJCaseData();
    }


    public class MJCaseData : MJGameData
    {
        public override bool isFriendRoom => false;

        public override DiceUsage diceUsage => DiceUsage.Two;

        public override int maxHandCardCount => 14;

        public override List<int> wallStacks => Wall.Stacks144;

        public override bool showBankerIcon => false;

        public MJCaseData ()
        {
            bankerChairId = 0;
        }
    }


    public class MJCaseRoomManager : BaseRoomManager
    {

        public ushort selfChairId => _playerSelf.chairId;

        public MJCaseRoomManager()
        {
            _roomInfo = new RoomInfo
            {
                baseMoney = 500,
                playersPerDesk = 4,
                name = "普通场"
            };

            _gameInfo = new GameInfo
            {
                shortName = "mjcase"
            };

            _playerSelf = MockUtils.MockPlayer<BaseGamePlayer>(0);
        }


        protected override bool NeedsWaiting(MsgHeader msg) => false;
    }

    public class MJCaseHandController : MJHandController
    {
        protected override void OnDiscard(MJHandSet hand, MJCard card)
        {
            if (!Discardable(hand, card))
            {
                return;
            }

            base.OnDiscard(hand, card);


            (stage.gameData.playerSelf as MJGamePlayer).RemoveHandCardValue(card.cardValue);
        }
    }
}

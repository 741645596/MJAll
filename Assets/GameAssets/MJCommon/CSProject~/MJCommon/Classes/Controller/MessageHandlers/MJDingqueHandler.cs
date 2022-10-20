// @Author: tanjinhua
// @Date: 2021/5/14  9:59


using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;
using WLHall.Game;
using Object = UnityEngine.Object;

namespace MJCommon
{
    /// <summary>
    /// 定缺消息业务
    /// </summary>
    public class MJDingqueHandler : BaseGameController
    {
        private MJGameData _gameData;
        private MJDingquePanel _panel;

        private MJHandController _handController;
        private MJTableController _tableController;
        private MJAvatarController _avatarController;
        private MJGameStartHandler _gameStartHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _handController = stage.GetController<MJHandController>();
            _tableController = stage.GetController<MJTableController>();
            _avatarController = stage.GetController<MJAvatarController>();
            _gameStartHandler = stage.GetController<MJGameStartHandler>();
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="dingqueColorValues">玩家定缺颜色值数组，根据玩家服务器座位号顺序存</param>
        public void UpdateData(List<int> dingqueColorValues)
        {
            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                player.dingqueColorValue = dingqueColorValues[player.chairId];

                player.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable);
            });
        }


        /// <summary>
        /// 定缺开始，gameStart调用
        /// </summary>
        public void EnqueueDingqueStart()
        {
            _gameData.gameState = GameState.Dingque;

            stage.animationQueue.Enqueue(0, () =>
            {
                _gameData.trustForbidden = true;

                _handController.GetOwnHand().interactable = false;

                ShowDingquePanel();

                _panel.ShowButtons(FindRecommonColorValue());

                _gameData.TraversePlayer(p =>
                {
                    int viewChairId = stage.ToViewChairId(p.chairId);
                    if (viewChairId != Chair.Down)
                    {
                        var pos = GetInfoPos(viewChairId);
                        _panel.ShowInfoSelecting(viewChairId, pos);
                    }
                });

                StartDingqueTimer();
            });
        }


        /// <summary>
        /// 收到玩家选择定缺消息
        /// </summary>
        /// <param name="chairId"></param>
        public void EnqueueDingqueSelected(int chairId)
        {
            // TODO：暂不处理
        }


        /// <summary>
        /// 播放定缺结果动画
        /// </summary>
        /// <param name="dingqueColorValues"></param>
        public void EnqueueDingqueResult(List<int> dingqueColorValues)
        {
            UpdateData(dingqueColorValues);

            _gameData.gameState = GameState.Play;

            _gameData.trustForbidden = false;

            var viewChairId = stage.ToViewChairId(_gameData.bankerChairId);

            var playerSelf = _gameData.playerSelf as MJGamePlayer;

            var handCardValues = playerSelf.handCardValues;

            stage.animationQueue.Enqueue(1.3f, () =>
            {
                RemoveDingquePanel();

                _handController.Reload(Chair.Down, handCardValues);

                UpdateSelfHandByDingque();

                _gameData.trustForbidden = false;

                _handController.GetOwnHand().interactable = true;

                _gameData.TraversePlayer<MJGamePlayer>(player =>
                {
                    if (player == playerSelf)
                    {
                        return;
                    }
                    var colorValue = dingqueColorValues[player.chairId];
                    PlayFlyInAnimation(stage.ToViewChairId(player.chairId), colorValue, display.center + new Vector2(0, 100));
                });
            });

            _gameStartHandler.EnqueueBankerTimer(_gameData.trustTimeout);
        }


        /// <summary>
        /// 定缺断线重连逻辑
        /// </summary>
        /// <param name="dingqueColorValues"></param>
        public void EnqueueResetDingqueState(List<int> dingqueColorValues)
        {
            UpdateData(dingqueColorValues);

            bool isDingqueFinished = dingqueColorValues.CountByCondition(v => Card.ColorNone == v) == 0;

            if (!isDingqueFinished)
            {
                _gameData.gameState = GameState.Dingque;
            }

            stage.animationQueue.Enqueue(0, () =>
            {
                UpdateSelfHandByDingque();

                if (!isDingqueFinished)
                {
                    RecoverDingqueState(dingqueColorValues);
                }
            });
        }

        /// <summary>
        /// 显示定缺界面
        /// </summary>
        public void ShowDingquePanel()
        {
            if (_panel != null && _panel.gameObject != null)
            {
                return;
            }
            _panel = OnCreateDingquePanel();
            _panel.onSelect = OnSelect;

        }

        /// <summary>
        /// 移除定缺UI
        /// </summary>
        public void RemoveDingquePanel()
        {
            if (_panel != null && _panel.gameObject != null)
            {
                _panel.RemoveFromParent();
            }

            _panel = null;
        }


        /// <summary>
        /// 开始自家定缺倒计时
        /// </summary>
        /// <param name="timeout"></param>
        public void StartDingqueTimer(int timeout = -1)
        {
            timeout = timeout == -1 ? _gameData.trustTimeout : timeout;

            Action<int> action = elapse =>
            {
                if (_gameData.isFriendRoom)
                {
                    return;
                }

                if (elapse == timeout)
                {
                    int recommoned = FindRecommonColorValue();

                    OnSelect(recommoned);
                }
            };
            _tableController.StartTimer(timeout, action);
        }

        /// <summary>
        /// 头像显示定缺小图标
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="colorValue"></param>
        /// <returns></returns>
        public MJDingqueIcon ShowDingqueIcon(int viewChairId, int colorValue)
        {
            var avatar = _avatarController.GetAvatar<MJAvatar>(viewChairId);
            if (avatar == null)
            {
                return null;
            }

            var name = "dingque_icon";
            var icon = avatar.GetIcon(name);
            if (icon != null)
            {
                Object.Destroy(icon.gameObject);
            }

            var newIcon = new MJDingqueIcon(colorValue);
            avatar.AddIcon(name, newIcon.rectTransform);
            newIcon.rectTransform.SetPositionInZero(GetDingqueIconPos(viewChairId));
            newIcon.SetScale(0.4f);

            return newIcon;
        }

        /// <summary>
        /// 显示多家头像定缺小图标
        /// </summary>
        /// <param name="colorValues"></param>
        public void ShowDingqueIcon(List<int> colorValues)
        {
            _gameData.TraversePlayer(p =>
            {
                var colorValue = colorValues[p.chairId];
                var viewChairId = stage.ToViewChairId(p.chairId);
                ShowDingqueIcon(viewChairId, colorValue);
            });
        }

        /// <summary>
        /// 更新自家手牌状态，定缺牌可出牌，其他牌不可出且变暗
        /// </summary>
        public virtual void UpdateSelfHandByDingque()
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            if (playerSelf.isReadyHand)
            {
                return;
            }

            playerSelf.unselectableCardValues = null;

            _handController.ResetTint(Chair.Down);

            int dingque = playerSelf.dingqueColorValue;

            if (dingque == Card.ColorNone)
            {
                return;
            }

            List<int> values = _handController.GetValues(Chair.Down);

            int dingqueCardCount = values.CountByCondition(v => Card.GetCardColorValue(v) == dingque);

            if (dingqueCardCount == 0)
            {
                return;
            }

            // 设置不可选择牌
            playerSelf.unselectableCardValues = values.Fetch(v => Card.GetCardColorValue(v) != dingque);

            // 不可选择牌变暗
            _handController.DarkenByValues(Chair.Down, playerSelf.unselectableCardValues);
        }


        /// <summary>
        /// 根据定缺情况判断牌值能否打出
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        public bool DiscardableByDingque(int cardValue)
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            int dingque = playerSelf.dingqueColorValue;

            if (dingque == Card.ColorNone)
            {
                return true;
            }

            if (Card.GetCardColorValue(cardValue) == dingque)
            {
                return true;
            }

            var count = playerSelf.handCardValues.CountByCondition(v => Card.GetCardColorValue(v) == dingque);
            if (count == 0)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 点击选择定缺按钮事件
        /// </summary>
        /// <param name="dingqueColorValue"></param>
        protected virtual void OnSelect(int dingqueColorValue)
        {
            PlayFlyInAnimation(Chair.Down, dingqueColorValue, _panel.GetButtonDisplayPos(dingqueColorValue));

            _panel.HideButtons();

            _panel.ShowInfoWaitOthers();

            _tableController.StopTurn();

            _avatarController.HideAllLightEffect();

            stage.Send("SendDingque", dingqueColorValue);

            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            playerSelf.dingqueColorValue = dingqueColorValue;

            playerSelf.SortHandCardValues(_gameData.jokerCardValues, _gameData.jokerDiscardable, true);

            _handController.Reload(Chair.Down, playerSelf.handCardValues);

            UpdateSelfHandByDingque();
        }


        /// <summary>
        /// 获取定缺小图标在头像上的位置
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        protected virtual Vector2 GetDingqueIconPos(int viewChairId)
        {
            return new List<Vector2>
            {
                new Vector2(135, 165),
                new Vector2(10, 165),
                new Vector2(10, 165),
                new Vector2(135, 165)
            }[viewChairId];
        }

        /// <summary>
        /// 创建定缺飞入头像的特效节点
        /// </summary>
        /// <param name="colorValue"></param>
        /// <returns></returns>
        protected WNode CreateFlyInEffect(int colorValue)
        {
            var asset = "MJCommon/MJ/mj_ui_effe_anniu";
            var name = new List<string>
            {
                "feiru_wan_x_01_01.prefab",
                "feiru_tiao_x_01_01.prefab",
                "feiru_tong_x_01_01.prefab"
            }[colorValue];

            return WNode.Create(asset, name);
        }

        /// <summary>
        /// 播放定缺飞头像入动画
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="colorValue"></param>
        /// <param name="startDisplayPos"></param>
        protected void PlayFlyInAnimation(int viewChairId, int colorValue, Vector2 startDisplayPos)
        {
            var icon = ShowDingqueIcon(viewChairId, colorValue);
            if (icon == null)
            {
                return;
            }

            icon.gameObject.SetActive(false);

            var avatar = _avatarController.GetAvatar<MJAvatar>(viewChairId);
            var startLocalPos = avatar.rectTransform.DisplayToLocal(startDisplayPos);
            var effect = CreateFlyInEffect(colorValue);
            effect.AddTo(avatar);
            effect.rectTransform.SetPositionInZero(startLocalPos);

            var targetAnchoredPos = icon.rectTransform.anchoredPosition;

            var seq = DOTween.Sequence();
            seq.Join(effect.rectTransform.DOAnchorPos(targetAnchoredPos, 0.65f));
            seq.Join(effect.rectTransform.DOScale(0.3f, 0.65f));
            seq.AppendCallback(() =>
            {
                icon.gameObject.SetActive(true);
                icon.PlayEffect();
                effect.RemoveFromParent();

                var avatarEffect = WNode.Create("MJCommon/MJ/mj_ui_effe_anniu", "touxianglight_x_01_03.prefab");
                avatarEffect.AddTo(avatar);
                avatarEffect.DelayInvoke(2, () => avatarEffect.RemoveFromParent());
                avatarEffect.SetScale(1.3f);
            });
            seq.SetEase(Ease.InQuart);
            seq.SetTarget(avatar.gameObject);
            seq.SetLink(avatar.gameObject);
            seq.Play();
        }

        /// <summary>
        /// 创建定缺UI界面
        /// </summary>
        /// <returns></returns>
        protected virtual MJDingquePanel OnCreateDingquePanel()
        {
            var panel = new MJDingquePanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.DingquePanel);

            return panel;
        }

        /// <summary>
        /// 获取定缺状态信息ui位置
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <returns></returns>
        protected virtual Vector2 GetInfoPos(int viewChairId)
        {
            List<Vector2> configs = new List<Vector2>
            {
                new Vector2(display.cx, 200),
                new Vector2(display.width - 250, display.cy),
                new Vector2(display.cx, display.height - 100),
                new Vector2(250, display.cy)
            };
            return configs[viewChairId];
        }


        private void RecoverDingqueState(List<int> dingqueColorValues)
        {
            ShowDingquePanel();

            _gameData.TraversePlayer<MJGamePlayer>(player =>
            {
                var colorValue = dingqueColorValues[player.chairId];
                int viewChairId = stage.ToViewChairId(player.chairId);
                if (viewChairId == Chair.Down)
                {
                    if (colorValue == Card.ColorNone)
                    {
                        _handController.GetOwnHand().interactable = false;

                        _panel.ShowButtons(FindRecommonColorValue());

                        StartDingqueTimer();
                    }
                    else
                    {
                        _panel.ShowInfoWaitOthers();

                        ShowDingqueIcon(Chair.Down, colorValue);
                    }
                }
                else
                {
                    var pos = GetInfoPos(viewChairId);
                    if (colorValue == Card.ColorNone)
                    {
                        _panel.ShowInfoSelecting(viewChairId, pos);
                    }
                    else
                    {
                        _panel.ShowInfoSelected(viewChairId, pos);
                    }
                }
            });
        }


        private int FindRecommonColorValue()
        {
            MJGamePlayer playerSelf = _gameData.playerSelf as MJGamePlayer;

            List<int> handValues = playerSelf.handCardValues;

            int result = Card.ColorWan;

            int count = handValues.CountByCondition(v => Card.GetCardColorValue(v) == result);

            int countTiao = handValues.CountByCondition(v => Card.GetCardColorValue(v) == Card.ColorTiao);

            if (countTiao < count)
            {
                count = countTiao;

                result = Card.ColorTiao;
            }

            int countBing = handValues.CountByCondition(v => Card.GetCardColorValue(v) == Card.ColorBing);

            if (countBing < count)
            {
                result = Card.ColorBing;
            }

            return result;
        }


        #region Game Events
        public override void OnFriendGameOver()
        {
            RemoveDingquePanel();
        }
        #endregion
    }
}

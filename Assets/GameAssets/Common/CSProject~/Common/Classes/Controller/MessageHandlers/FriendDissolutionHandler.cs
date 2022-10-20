// @Author: tanjinhua
// @Date: 2021/5/25  20:04


using System.Collections.Generic;
using Unity.Widget;
using WLHall.Game;
using WLHall;

namespace Common
{
    public class FriendDissolutionHandler : BaseGameController
    {

        private const ushort InvalidChairId = 0xFFFF;
        private DissolutionDialog _dissolutionDialog;
        private GameData _gameData;

        private FriendGameStartHandler _friendGameStartHandler;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as GameData;

            _friendGameStartHandler = stage.GetController<FriendGameStartHandler>();
        }

        /// <summary>
        /// 收到玩家申请或同意解散广播
        /// </summary>
        /// <param name="chairId"></param>
        public virtual void RecvDissolutionBroadcast(int chairId)
        {
            if (_gameData.applyDissolutionChairId == InvalidChairId)
            {
                _gameData.applyDissolutionChairId = chairId;
            }

            if (_dissolutionDialog == null || _dissolutionDialog.gameObject == null)
            {
                List<DissolutionDialog.Config> configs = GetDialogConfigs(chairId, out bool needsShowButton);

                GetDissolutionDialog().SetConfigs(configs);

                GetDissolutionDialog().SetButtonsVisible(needsShowButton);

                GetDissolutionDialog().StartCountdown(120);
            }

            GetDissolutionDialog().ShowAgree(_gameData.GetPlayerByChairId(chairId).userId);
        }


        /// <summary>
        /// 收到断线重连消息
        /// </summary>
        /// <param name="applyChairId"></param>
        /// <param name="votes"></param>
        public virtual void RecvDissolutionReconnect(int applyChairId, List<int> votes)
        {
            if (_gameData.applyDissolutionChairId == InvalidChairId)
            {
                _gameData.applyDissolutionChairId = applyChairId;
            }

            List<DissolutionDialog.Config> configs = GetDialogConfigs(applyChairId, out bool needsShowButton, votes);

            GetDissolutionDialog().SetConfigs(configs);

            GetDissolutionDialog().SetButtonsVisible(needsShowButton);

            GetDissolutionDialog().StartCountdown(120);
        }


        /// <summary>
        /// 倒计时更新
        /// </summary>
        /// <param name="time"></param>
        public virtual void EnqueueDissolutionTimeUpdate(int time)
        {
            stage.animationQueue.Enqueue(0, () =>
            {
                if (_dissolutionDialog != null && _dissolutionDialog.gameObject != null)
                {
                    GetDissolutionDialog().StartCountdown(time);
                }
            });
        }

        /// <summary>
        /// 收到解散结果消息
        /// </summary>
        /// <param name="isDismiss"></param>
        /// <param name="chairId"></param>
        public virtual void RecvDissolutionResult(bool isDismiss, int chairId)
        {
            RemoveDialog();

            if (isDismiss)
            {
                var inReadyState = _friendGameStartHandler.IsInReadyState();
                if (inReadyState)
                {
                    if (_gameData.playerSelf.chairId == chairId)
                    {
                        stage.Exit();
                        return;
                    }

                    string tips = _gameData.isQinYouQuan ? "房主(管理员)已解散房间" : "房主已解散房间";

                    stage.ExitWithDialog(tips);
                }
                else
                {
                    // TODO: 弹出解散成功吐司提示
                }

                return;
            }

            _gameData.applyDissolutionChairId = InvalidChairId;
            string nickname = _gameData.GetPlayerByChairId(chairId).GetNickNameUtf16();
            string msg = $"{nickname}拒绝解散，请继续游戏";
            // TODO: 弹出玩家拒绝解散吐司提示
        }

        /// <summary>
        /// 申请解散逻辑
        /// </summary>
        public virtual void ApplyDissolution()
        {
            //string text = "您确定要解散本局游戏吗？";

            // TODO: 显示申请解散提示弹窗 
            //GameDialog.CreateWithOKCancel(text, index =>
            //{
            //    if (index == GameDialog.ClickType.OK)
            //    {
                    stage.Send("SendDissolutionApply");

                    // 如果在准备界面，则退出到大厅
                    var inReadyState = _friendGameStartHandler.IsInReadyState();
                    if (inReadyState)
                    {
                        stage.Exit();
                    }
                //}
            //});

        }


        /// <summary>
        /// 移除解散对话框
        /// </summary>
        public void RemoveDialog()
        {
            if (_dissolutionDialog != null && _dissolutionDialog.gameObject != null)
            {
                _dissolutionDialog.RemoveFromParent();
            }

            _dissolutionDialog = null;
        }


        /// <summary>
        /// 获取解散对话框参数配置
        /// </summary>
        /// <param name="applyChairId"></param>
        /// <param name="needsShowButtons"></param>
        /// <returns></returns>
        protected virtual List<DissolutionDialog.Config> GetDialogConfigs(int applyChairId, out bool needsShowButtons, List<int> votes = null)
        {
            bool showBtn = false;

            List<DissolutionDialog.Config> configs = new List<DissolutionDialog.Config>();

            _gameData.TraversePlayer(p =>
            {
                GamePlayer player = p as GamePlayer;

                DissolutionDialog.Config config = new DissolutionDialog.Config
                {
                    nickname = player.GetNickNameUtf16(),
                    avatarUrl = player.GetAvatarPath(),
                    userId = player.userId,
                    gender = player.gender,
                    isInitiator = p.chairId == applyChairId,
                    agreed = p.chairId == applyChairId
                };

                if (votes != null)
                {
                    config.agreed = votes[p.chairId] == 1;
                }

                if (p == _gameData.playerSelf && !config.agreed)
                {
                    showBtn = true;
                }

                if (config.isInitiator)
                {
                    configs.Insert(0, config);
                }
                else
                {
                    configs.Add(config);
                }
            });

            needsShowButtons = showBtn;

            return configs;
        }


        /// <summary>
        /// 创建申请解散对话框
        /// </summary>
        /// <returns></returns>
        protected virtual DissolutionDialog OnCreateDissolutionDialog()
        {
            DissolutionDialog dialog = new DissolutionDialog();

            dialog.AddTo(WDirector.GetRootLayer(), HallZorder.End + HallZorder.Gap);

            return dialog;
        }


        /// <summary>
        /// 点击解散弹窗按钮事件，code = 1 表示同意， 0 表示拒绝
        /// </summary>
        /// <param name="code"></param>
        protected virtual void OnClickDialogButtons(byte code)
        {
            GetDissolutionDialog().SetButtonsVisible(false);

            stage.Send("SendFriendDissolutionVote", code);
        }


        private DissolutionDialog GetDissolutionDialog()
        {
            if (_dissolutionDialog == null || _dissolutionDialog.gameObject == null)
            {
                _dissolutionDialog = OnCreateDissolutionDialog();

                _dissolutionDialog.onClickButton = OnClickDialogButtons;
            }
            return _dissolutionDialog;
        }
    }
}

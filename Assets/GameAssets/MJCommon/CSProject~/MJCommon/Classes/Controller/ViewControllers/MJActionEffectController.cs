// @Author: tanjinhua
// @Date: 2021/4/15  13:44


using System.Collections.Generic;
using Unity.Widget;
using WLHall.Game;

namespace MJCommon
{
    public class MJActionEffectController : BaseGameController
    {
        private MJGameData _gameData;
        private MJActionEffectPanel _panel;

        private MJAudioController _audioController;

        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _audioController = stage.GetController<MJAudioController>();

            _panel = OnCreateActionEffectPanel();
        }


        public MJActionEffectPanel GetPanel()
        {
            return _panel;
        }

        /// <summary>
        /// 创建动作特效层，添加到场景并设置zorder
        /// </summary>
        /// <returns></returns>
        protected virtual MJActionEffectPanel OnCreateActionEffectPanel()
        {
            var panel = new MJActionEffectPanel();

            panel.AddTo(WDirector.GetRootLayer(), MJZorder.ActionEffectPanel);

            return panel;
        }


        /// <summary>
        /// 播放动作特效
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        public virtual void PlayActionEffect(int chairId, int showType, FuziData fuziData)
        {
            int viewChairId = stage.ToViewChairId(chairId);

            int gender = _gameData.GetPlayerByChairId(chairId).gender;

            var config = GetActionEffectConfig(showType);

            _audioController.PlayActionVoice(showType, gender);

            _panel.PlayEffect(viewChairId, config);
        }


        /// <summary>
        /// 播放胡动作特效
        /// </summary>
        /// <param name="chairId"></param>
        /// <param name="showType"></param>
        /// <param name="huType"></param>
        public virtual void PlayHuActionEffect(ushort chairId, int showType, long huType)
        {
            int viewChairId = stage.ToViewChairId(chairId);

            int gender = _gameData.GetPlayerByChairId(chairId).gender;

            var config = GetHuActionEffectConfig(showType, huType);

            _audioController.PlayActionVoice(showType, gender);

            if (showType == ActionShowType.Zimo && viewChairId == Chair.Down)
            {
                _panel.PlaySelfZimoEffect();
            }
            else
            {
                _panel.PlayEffect(viewChairId, config);
            }
        }


        /// <summary>
        /// 清除特效与动画
        /// </summary>
        public virtual void Clear()
        {
            _panel.ClearEffect();
        }


        /// <summary>
        /// 清除动作特效
        /// </summary>
        /// <param name="viewChairId"></param>
        public virtual void Clear(int viewChairId)
        {
            _panel.ClearEffect(viewChairId);
        }


        /// <summary>
        /// 获取动作特效配置
        /// </summary>
        /// <param name="gender"></param>
        /// <param name="showType"></param>
        /// <returns></returns>
        protected virtual KeyValuePair<string, string> GetActionEffectConfig(int showType)
        {
            return MJActionEffect.GetDefaultConfig(showType);
        }


        /// <summary>
        /// 获取胡动作特效配置
        /// </summary>
        /// <param name="gender">性别</param>
        /// <param name="showType"></param>
        /// <param name="huType"></param>
        /// <returns></returns>
        protected virtual KeyValuePair<string, string> GetHuActionEffectConfig(int showType, long huType)
        {
            return MJActionEffect.GetDefaultConfig(showType);
        }


        #region Game Events
        public override void OnContinue()
        {
            base.OnContinue();

            Clear();
        }


        public override void OnChangeDesk()
        {
            base.OnChangeDesk();

            Clear();
        }
        #endregion
    }
}

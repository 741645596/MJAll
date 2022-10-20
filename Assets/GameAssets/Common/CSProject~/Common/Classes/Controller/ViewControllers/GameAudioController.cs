// @Author: tanjinhua
// @Date: 2021/5/28  22:46


using Unity.Core;
using WLHall.Game;

namespace Common
{
    public abstract class GameAudioController : BaseGameController
    {

        private GameSettingController _settingController;


        public override void OnSceneLoaded()
        {
            _settingController = stage.GetController<GameSettingController>();

            PlayBackgroundMusic();
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public abstract void PlayBackgroundMusic();

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            AudioMgr.StopMusic();
        }

        /// <summary>
        /// 播放破产音效
        /// </summary>
        public virtual void PlayBankcrypt()
        {
            //PlayEffect("Common/Audio/bankruptcy.mp3");
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="path"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public virtual int PlayEffect(string assetName, string path, bool loop = false)
        {
            if (!_settingController.GetSoundEffectState())
            {
                return -1;
            }

            //return AudioManager.Instance.PlayEffect(path, loop);
            return -1;
        }

        /// <summary>
        /// 停止音效
        /// </summary>
        /// <param name="index"></param>
        public virtual void StopEffect(int index)
        {
            //AudioManager.Instance.StopEffect(index);
        }

        /// <summary>
        /// 停止所有音效
        /// </summary>
        public virtual void StopAllEffects()
        {
            //AudioManager.Instance.StopAllEffects();
        }

        public override void OnShutdown()
        {
            base.OnShutdown();

            StopBackgroundMusic();
        }
    }
}

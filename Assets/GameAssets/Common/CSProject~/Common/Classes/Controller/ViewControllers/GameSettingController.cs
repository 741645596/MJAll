// @Author: tanjinhua
// @Date: 2021/6/7  9:19


using WLHall.Game;

namespace Common
{
    public class GameSettingController : BaseGameController
    {
        protected string _shortName;

        public override void OnSceneLoaded()
        {
            _shortName = stage.gameData.gameInfo.shortName;
        }

        /// <summary>
        /// 获取音乐开关状态
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetMusicState(bool defaultValue = true)
        {
            //return SettingManager.GetHallMusicState(defaultValue);
            return true;
        }

        /// <summary>
        /// 设置音乐开关状态
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetMusicState(bool state)
        {
            //SettingManager.SetHallMusicState(state);
        }

        /// <summary>
        /// 获取音效开关状态
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetSoundEffectState(bool defaultValue = true)
        {
            //return SettingManager.GetHallEffectState(defaultValue);
            return true;
        }

        /// <summary>
        /// 设置音效开关状态
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetSoundEffectState(bool state)
        {
            //SettingManager.SetHallEffectState(state);
        }

        /// <summary>
        /// 获取方言开关状态
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public virtual bool GetDialectState(bool defaultValue = false)
        {
            //string key = "Dialect" + m_shortName;

            //return LocalStorage.GetBool(key, defaultValue);

            return false;
        }

        /// <summary>
        /// 设置方言开关状态
        /// </summary>
        /// <param name="state"></param>
        public virtual void SetDialectState(bool state)
        {
            string key = "Dialect" + _shortName;

            //LocalStorage.SetBool(key, state);
        }
    }
}

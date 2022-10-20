// @Author: tanjinhua
// @Date: 2021/5/28  22:29


using Common;
using Unity.Core;

namespace MJCommon
{
    public class MJAudioController : GameAudioController
    {
        /// <summary>
        /// 默认人物音效路径
        /// </summary>
        public static string DefaultVoicePath => "MJCommon/Audio/mj_putonghua";

        /// <summary>
        /// 东北方言音效路径
        /// </summary>
        public static string NortheastVoicePath => "MJCommon/Audio/mj_putonghua2";


        private MJGameData _gameData;
        private GameSettingController _settingController;


        public override void OnSceneLoaded()
        {
            _gameData = stage.gameData as MJGameData;

            _settingController = stage.GetController<GameSettingController>();

            base.OnSceneLoaded();
        }

        public override void PlayBackgroundMusic()
        {
            if (!_settingController.GetMusicState())
            {
                return;
            }
            AudioMgr.PlayMusic("MJCommon/MJ/mj_audio", "background.mp3");
        }

        /// <summary>
        /// 点击按钮音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlayButtonClick()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/button.mp3");
            return -1;
        }

        /// <summary>
        /// 打牌碰撞桌子音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlayDiscard()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/dapai.mp3");
            return -1;
        }

        /// <summary>
        /// 开局发牌音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlayDealCard()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/draw.mp3");
            return -1;
        }

        /// <summary>
        /// 选牌音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlaySelectCard()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/select.mp3");
            return -1;
        }

        /// <summary>
        /// 骰子音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlayDice()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/shaizi.mp3");
            return -1;
        }

        /// <summary>
        /// 理牌动画音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlaySortCard()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/sort.mp3");
            return -1;
        }

        /// <summary>
        /// 分数动画音效
        /// </summary>
        /// <returns></returns>
        public virtual int PlayScoring()
        {
            //return PlayEffect("MJCommon/Audio/mj_effect/jiesuan.mp3");
            return -1;
        }

        /// <summary>
        /// 播放人物出牌音效
        /// </summary>
        /// <param name="cardValue"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public virtual int PlayOutCardVoice(int cardValue, int gender)
        {
            string fileName = GetOutCardVoiceName(cardValue, gender);

            string path = GetVoiceFullPath(fileName, SupportsNortheastOutCardVoice(cardValue));

            //return PlayEffect(path);
            return -1;
        }

        /// <summary>
        /// 获取人物出牌音效文件名，有随机音效需求的游戏可以重写此方法
        /// </summary>
        /// <param name="cardValue"></param>
        /// <returns></returns>
        protected virtual string GetOutCardVoiceName(int cardValue, int gender)
        {
            string folder = gender == 1 ? "boy" : "girl";

            return $"{folder}/card_{cardValue}.mp3";
        }

        /// <summary>
        /// 播放人物动作音效
        /// </summary>
        /// <param name="showType"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public virtual int PlayActionVoice(int showType, int gender)
        {
            string fileName = GetActionVoiceName(showType, gender);

            string path = GetVoiceFullPath(fileName, SupportsNortheastActionVoice(showType));

            //return PlayEffect(path);
            return -1;
        }

        /// <summary>
        /// 获取人物动作音效文件名，有随机音效需求的游戏可重写此方法
        /// </summary>
        /// <param name="showType"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        protected virtual string GetActionVoiceName(int showType, int gender)
        {
            string folder = gender == 1 ? "boy" : "girl";

            return $"{folder}/event_{showType}.mp3";
        }

        /// <summary>
        /// 获取人物音效完整路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="supportsNortheastVoice"></param>
        /// <returns></returns>
        protected virtual string GetVoiceFullPath(string fileName, bool supportsNortheastVoice)
        {
            var dialectEnabled = _settingController.GetDialectState();
            // 优先东北方言音效
            if (supportsNortheastVoice && _gameData.useNortheastDialect && (dialectEnabled || _gameData.northeastDialectIgnoresSetting))
            {
                return $"{NortheastVoicePath}/{fileName}";
            }

            // 其次自定义方言音效
            if (dialectEnabled && _gameData.dialectVoiceDirectories != null)
            {
                for (int i = 0; i < _gameData.dialectVoiceDirectories.Count; i++)
                {
                    string directory = _gameData.dialectVoiceDirectories[i];

                    string path = $"{directory}/{fileName}";
                    // TODO：
                    //if (AssetsLoader.Instance.AssetExists(path))
                    //{
                    //    return path;
                    //}
                }
            }

            // 最后通用音效
            return $"{DefaultVoicePath}/{fileName}";
        }

        private bool SupportsNortheastOutCardVoice(int cardValue)
        {
            return cardValue >= Card.Bing1 && cardValue <= Card.Bing9;
        }

        private bool SupportsNortheastActionVoice(int showType)
        {
            return showType == 2;
        }
    }
}

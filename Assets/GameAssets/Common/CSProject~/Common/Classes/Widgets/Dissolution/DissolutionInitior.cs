// @Author: lili
// @Date: 2021/5/11  15:35

using Unity.Core;
using UnityEngine;

namespace Common
{
    public class DissolutionInitior : CountdownBase
    {
        private SKText _countdown;

        public DissolutionInitior(DissolutionDialog.Config config)
        {
            InitGameObject("Common/Game/game_ui_prefabs", "dissolution_Invitor.prefab");

            var avatar = FindReference<RemoteImage>("avatar");
            var key = config.gender == 1 ? "common/avatar/male.png" : "common/avatar/female.png";
            avatar.sprite = AssetsManager.Load<Sprite>("Common/Game/game_ui_atlas", key);
            avatar.url = config.avatarUrl;

            FindReference<SKText>("nickname").text = config.nickname;
            FindReference<SKText>("userId").text = $"ID:{config.userId}";
            _countdown = FindReference<SKText>("countdown");
        }

        protected override void OnTick(int time)
        {
            int minutes = time / 60;
            int seconds = time - minutes * 60;
            string min = minutes.ToString();
            if (min.Length == 1)
            {
                min = "0" + min;
            }
            string sec = seconds.ToString();
            if (sec.Length == 1)
            {
                sec = "0" + sec;
            }
            _countdown.text = min + ":" + sec;
        }
    }
}

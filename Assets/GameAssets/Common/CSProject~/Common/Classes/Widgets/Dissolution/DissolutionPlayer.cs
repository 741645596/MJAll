// @Author: lili
// @Date: 2021/5/11  15:35

using Unity.Widget;
using Unity.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class DissolutionPlayer : WNode
    {
        public uint userId { get; private set; }

        private GameObject _argee;
        private GameObject _notSelect;

        public DissolutionPlayer(DissolutionDialog.Config config, bool showSlice = true)
        {
            InitGameObject("Common/Game/game_ui_prefabs", "dissolution_player.prefab");

            userId = config.userId;

            var avatar = FindReference<RemoteImage>("avatar");
            var key = config.gender == 1 ? "common/avatar/male.png" : "common/avatar/female.png";
            avatar.sprite = AssetsManager.Load<Sprite>("Common/Game/game_ui_atlas", key);
            avatar.url = config.avatarUrl;

            FindReference<SKText>("nickname").text = config.nickname;
            FindReference<Image>("slice").gameObject.SetActive(showSlice);
            _argee = FindReference("argee").gameObject;
            _notSelect = FindReference("notselect").gameObject;

            if (config.agreed)
            {
                ShowArgee();
            }
        }

        public void ShowArgee()
        {
            _argee.SetActive(true);

            _notSelect.SetActive(false);
        }
    }
}

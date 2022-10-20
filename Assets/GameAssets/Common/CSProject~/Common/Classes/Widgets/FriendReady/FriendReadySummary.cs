// @Author: tanjinhua
// @Date: 2021/9/16  15:18

using System;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class FriendReadySummary : WNode
    {
        public Action<Vector2> onUpdateSize;

        private SKText _roomKey;
        private SKText _gameCountTitle;
        private SKText _gameCount;
        private SKText _otherInfoTitle;
        private SKText _otherInfo;
        private VerticalLayoutGroup _group;

        public FriendReadySummary()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "friendready_summary.prefab");

            _roomKey = FindInChildren<SKText>("room_key_title/room_key");
            _roomKey.onUpdateSize += OnUpdateSize;

            _gameCountTitle = FindInChildren<SKText>("game_count_title");
            _gameCount = FindInChildren<SKText>("game_count_title/game_count");
            _gameCount.onUpdateSize += OnUpdateSize;

            _otherInfoTitle = FindInChildren<SKText>("other_info_title");
            _otherInfo = FindInChildren<SKText>("other_info_title/other_info");
            _otherInfo.onUpdateSize += OnUpdateSize;

            _group = gameObject.GetComponent<VerticalLayoutGroup>();
        }


        public void SetConfig(FriendReadyLayer.Config config)
        {
            _roomKey.text = config.roomKey;
            _gameCountTitle.text = config.gameCountTitle;
            _gameCount.text = config.gameCountValue;
            if (!string.IsNullOrEmpty(config.otherInfoTitle) && !string.IsNullOrEmpty(config.otherInfo))
            {
                _otherInfoTitle.gameObject.SetActive(true);
                _otherInfoTitle.text = config.otherInfoTitle;
                _otherInfo.text = config.otherInfo;
            }
        }

        private void OnUpdateSize()
        {
            float height = 0f;
            int spaceCount = 1;

            height += _roomKey.rectTransform.rect.size.y;
            height += _gameCount.rectTransform.rect.size.y;
            if (_otherInfo.gameObject.activeInHierarchy)
            {
                height += _otherInfo.rectTransform.rect.size.y;
                spaceCount = 2;
            }

            var spacing = _group.spacing;
            var padding = _group.padding;

            var size = new Vector2(550, height + spaceCount * spacing + padding.top + padding.bottom);

            SetContentSize(size);

            onUpdateSize?.Invoke(size);
        }
    }
}

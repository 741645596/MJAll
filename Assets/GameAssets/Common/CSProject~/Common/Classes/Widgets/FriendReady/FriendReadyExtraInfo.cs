// @Author: tanjinhua
// @Date: 2021/9/16  15:50

using Unity.Widget;
using System;
using UnityEngine;

namespace Common
{
    public class FriendReadyExtraInfo : WNode
    {
        public Action<Vector2> onUpdateSize;

        private SKText _text;

        public FriendReadyExtraInfo()
        {
            InitGameObject("Common/Game/game_ui_prefabs", "friendready_extrainfo.prefab");

            _text = gameObject.GetComponent<SKText>();

            _text.onUpdateSize += () => onUpdateSize?.Invoke(rectTransform.rect.size);
        }

        public void SetConfig(FriendReadyLayer.Config config)
        {
            var str = "";
            if (config.isOpenGPS)
            {
                str += "本房间已开启GPS定位功能    ";
            }
            if (config.isOvertimeDisslotion)
            {
                str += "已开启超时解散    ";
            }
            if (config.isAccumulatingOfflineTime)
            {
                str += "已开启离线时间累计    ";
            }
            _text.text = str;
        }
    }
}

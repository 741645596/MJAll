// @Author: tanjinhua
// @Date: 2021/5/28  0:05


using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace Common
{
    public class FriendRecordButton : WNode
    {
        public enum Type
        {
            /// <summary>
            /// 分享
            /// </summary>
            Share,
            /// <summary>
            /// 战绩分享
            /// </summary>
            ShareRecord,
            /// <summary>
            /// 好友分享
            /// </summary>
            ShareFriend,
            /// <summary>
            /// 默往分享
            /// </summary>
            ShareMowang,
            /// <summary>
            /// 好运来
            /// </summary>
            AdHYL,
            /// <summary>
            /// 续好运
            /// </summary>
            AdXHY,
            /// <summary>
            /// 复制
            /// </summary>
            Copy,
            /// <summary>
            /// 再来一局
            /// </summary>
            OneMoreGame,
            /// <summary>
            /// 退出
            /// </summary>
            Exit,
        }

        public Action<Type> onClick;

        public FriendRecordButton(Type type)
        {
            string path = $"Common/RecordStatistics/Buttons/Prefabs/{type}.prefab";

            //GameObject obj = Instantiate(path);

            //Setup(obj);

            //Classify();

            //gameObject.GetComponent<Button>().onClick.AddListener(() => onClick?.Invoke(type));

            //if (type == Type.AdHYL || type == Type.AdXHY)
            //{
            //    Node2D bubble = FindReference("Bubble") as Node2D;
            //    Action2D.NewDelayCallfunWithNode(bubble, 2f, () => bubble.RemoveFromParent());
            //}
        }
    }
}

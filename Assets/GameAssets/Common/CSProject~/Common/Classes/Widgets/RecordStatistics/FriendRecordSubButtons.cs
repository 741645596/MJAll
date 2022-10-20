// @Author: tanjinhua
// @Date: 2021/5/28  10:37


using System;
using UnityEngine;
using Unity.Widget;
using System.Collections.Generic;

namespace Common
{
    public class FriendRecordSubButtons : WNode
    {

        public Action<FriendRecordButton.Type> onClick;

        //private Node2D m_container;

        public FriendRecordSubButtons(List<FriendRecordButton.Type> types)
        {
            //m_container = FindReference("Container") as Node2D;

            //types.ForEach(t =>
            //{
            //    FriendRecordButton button = new FriendRecordButton(t);

            //    button.onClick = type => onClick?.Invoke(type);

            //    m_container.AddChild(button);
            //});

            //size = new Vector2(162, types.Count * 102f + (types.Count - 1) * 20f + 60f);

            //RegisterTouchEvents2D(d => true, null, d => RemoveFromParent());
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/RecordStatistics/Buttons/Prefabs/SubButtons.prefab");
        //}
    }
}

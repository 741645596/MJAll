// @Author: lili
// @Date: 2021/6/2 15:29:35

using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace Common
{
    /// <summary>
    /// 朋友场信息气泡（显示局数，扑克用）
    /// </summary>
    public class FriendRoundInfoBubble : WNode
    {
        //private Text m_roundText;
        private Button m_button;

        public Action onClick;

        public FriendRoundInfoBubble()
        {
            //m_roundText = FindReference<Text>("Round");
            m_button = gameObject.GetComponent<Button>();
            m_button.onClick.AddListener(() => onClick?.Invoke());
        }

        /// <summary>
        /// 更新局数
        /// </summary>
        /// <param name="curCount">当前局数</param>
        /// <param name="allCount">总局数</param>
        public void UpdateGameCount(int curCount, int allCount)
        {
            //m_roundText.text = curCount + "/" + allCount + " 局";
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/FriendBubble/Prafabs/FriendRoundInfoBubble.prefab");
        //}
    }
}

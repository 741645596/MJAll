// @Author: lili
// @Date: 2021/6/2 15:30:25

using UnityEngine;

namespace Common
{
    public class FriendInfoPopNode2 : FriendInfoPopNode
    {
        public FriendInfoPopNode2(FriendGameInfo info, string roomKey) : base(info, roomKey)
        {
        }

        protected override void SetRule(FriendGameInfo info)
        {
            //FriendReadyLayer.WordWrapInSKText(m_ruleText, info.rules, "、");
            //m_ruleText.onUpdateSize += () =>
            //{
            //    size = new Vector2(size.x, m_roomInfoNode.size.y + m_ruleNode.size.y + 35);
            //    // 分隔线换成了深色背景
            //    m_lineNode.size = new Vector2(m_lineNode.size.x, m_ruleNode.size.y + 20);
            //};
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/FriendBubble/Prafabs/FriendInfoPopNode2.prefab");
        //}
    }
}

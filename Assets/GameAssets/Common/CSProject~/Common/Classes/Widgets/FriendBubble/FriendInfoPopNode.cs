// @Author: lili
// @Date: 2021/5/31 17:22:48

using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using WLHall;

namespace Common
{
    public class FriendInfoPopNode : WNode
    {
        //protected Node2D m_roomInfoNode;
        //protected Text m_roomKeyValue;
        //protected Text m_gameCountTitle;
        //protected Text m_gameCountValue;
        //protected Text m_othertTitle;
        //protected Text m_otherValue;
        //protected SKText m_ruleText;
        //protected Node2D m_ruleNode;
        //protected Node2D m_lineNode;


        public FriendInfoPopNode(FriendGameInfo info, string roomKey)
        {
            Init();
            SetInfo(info, roomKey);
            SetRule(info);
        }

        protected virtual void Init()
        {
            //m_roomInfoNode = FindReference("RoomInfoBg") as Node2D;
            //m_roomKeyValue = FindReference<Text>("RoomKeyValue");
            //m_gameCountTitle = FindReference<Text>("GameCountTitle");
            //m_gameCountValue = FindReference<Text>("GameCountValue");
            //m_othertTitle = FindReference<Text>("OtherTitle");
            //m_otherValue = FindReference<Text>("OtherValue");
            //m_ruleText = FindReference<SKText>("RuleText");
            //m_ruleNode = FindReference("RuleText") as Node2D;
            //m_lineNode = FindReference("Line") as Node2D;

            //RegisterTouchEvents2D(data => true, null, data => RemoveFromParent());
        }

        protected virtual void SetInfo(FriendGameInfo info, string roomKey)
        {
            //m_roomKeyValue.text = roomKey;
            //if (info.useCustomGameCount)
            //{
            //    m_gameCountTitle.text = info.customGameCountTitle;
            //    m_gameCountValue.text = info.customGameCount;
            //}
            //else
            //{
            //    m_gameCountTitle.text = "局数:";
            //    m_gameCountValue.text = info.currentGameCount + "/" + info.totalGameCount;
            //}
            //if (info.hasMultiplyInfo)
            //{
            //    string str = info.multiplyTitle;
            //    if (MathKit.IsChineseChar(str) && str.Length == 2)
            //    {
            //        str = str + ":";
            //    }
            //    m_othertTitle.gameObject.SetActive(true);
            //    m_othertTitle.text = str;
            //    m_otherValue.text = info.multiply;
            //}
            //float h = (info.hasMultiplyInfo ? 3 : 2) * 42 + 30;
            //m_roomInfoNode.size = new Vector2(m_roomInfoNode.size.x, h);
        }

        protected virtual void SetRule(FriendGameInfo info)
        {
            //FriendReadyLayer.WordWrapInSKText(m_ruleText, info.rules, "　");
            //m_ruleText.onUpdateSize += () =>
            //{
            //    size = new Vector2(size.x, m_roomInfoNode.size.y + m_ruleNode.size.y + 35);
            //    m_lineNode.position = new Vector2(size.x / 2, 35 + m_ruleNode.size.y);
            //};
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/FriendBubble/Prafabs/FriendInfoPopNode.prefab");
        //}
    }
}

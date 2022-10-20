// @Author: lili
// @Date: 2021/4/27  20:01

using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using WLHall;
using Common;

namespace MJCommon
{
    public class MJRecordItemNode : WNode
    {
        private Image m_rankImg;
        private Text m_nameTxt;
        private Text m_playerIDTxt;
        private Text m_scoreTxt;
        private RectTransform m_avatarParent;
        private Image m_applyDissolutionImg;
        private Image m_overtiemDissolutionImg;
        private RectTransform m_ownerNode;
        private RectTransform m_desContainer;
        private RectTransform m_nameNode;
        private RectTransform m_scoreNode;

        FriendRecordStatistics.PlayerInfo m_info;


        public MJRecordItemNode(FriendRecordStatistics.PlayerInfo info)
        {
            m_rankImg = FindReference<Image>("rank");
            m_nameTxt = FindReference<Text>("name");
            m_playerIDTxt = FindReference<Text>("playerID");
            m_scoreTxt = FindReference<Text>("score");
            m_avatarParent = FindReference("avatar") as RectTransform;
            m_applyDissolutionImg = FindReference<Image>("applyDissolution");
            m_overtiemDissolutionImg = FindReference<Image>("overtimeDissolution");
            m_ownerNode = FindReference("owner") as RectTransform;
            m_desContainer = FindReference("desContainer") as RectTransform;
            m_nameNode = FindReference("name") as RectTransform;
            m_scoreNode = FindReference("score") as RectTransform;

            m_info = info;

            SetAvatar();

            //排名
            SetRank();

            //名字
            SetNickname();

            //id
            SetUserId();

            //房主
            SetRoomOwner();

            //详细信息
            OnCreateDetailInfo();

            if (m_info.applyDissolution)
            {
                ShowApplyDissolutionIcon();
            }
            else if (m_info.overtimeDissolution)
            {
                ShowOvertimeDissolutionIcon();
            }
        }

        protected virtual void OnCreateDetailInfo()
        {
            //统计详情
            CreateInfoNode();

            //最终分数
            SetScoreTxt();
        }

        private void SetAvatar()
        {
            //OverlayAvatar avatar = new OverlayAvatar(m_info.userId, m_info.gender, m_info.avatarUrl);
            //avatar.size = new Vector2(90, 90);
            //m_avatarParent.AddChild(avatar, -1);
        }

        private void ShowOvertimeDissolutionIcon()
        {
            m_overtiemDissolutionImg.gameObject.SetActive(true);
        }

        private void ShowApplyDissolutionIcon()
        {
            m_applyDissolutionImg.gameObject.SetActive(true);
        }

        private void SetRank()
        {
            if (m_info.rank < 1 || m_info.rank > 3)
            {
                return;
            }
            string path = "Common/RecordStatistics/Sprites/pic_rank_" + m_info.rank + ".png";
            //m_rankImg.sprite = AssetsLoader.Instance.Load<Sprite>(path);
            m_rankImg.gameObject.SetActive(true);
        }

        private void SetNickname()
        {
            m_nameTxt.text = m_info.nickname;
            //m_nameNode.graphicSettings.color = m_info.isSelfPlayer ? cc.c3b(203, 0, 0) : cc.c3b(105, 77, 30);
        }

        private void SetUserId()
        {
            m_playerIDTxt.text = "ID:" + m_info.userId;
        }

        private void SetRoomOwner()
        {
            m_ownerNode?.gameObject?.SetActive(m_info.isRoomOwner);
        }

        private void CreateInfoNode()
        {
            //m_desContainer.RemoveAllChildren();
            //foreach (var pair in m_info.details)
            //{
            //    MJRecordDesNode node = new MJRecordDesNode(pair.Key, pair.Value);
            //    m_desContainer.AddChild(node);
            //}
        }

        private void SetScoreTxt()
        {
            if (m_info.score > 0)
            {
                m_scoreTxt.text = "+" + m_info.score;
                //m_scoreNode.graphicSettings.color = cc.c3b(204, 0, 0);
            }
            else
            {
                m_scoreTxt.text = "" + m_info.score;
                //m_scoreNode.graphicSettings.color = cc.c3b(0, 126, 26);
            }
        }


        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("MJCommon/Images/RecordStatistics/Prefabs/RecordItemNode.prefab");
        //}
    }
}

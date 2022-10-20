// @Author: lili
// @Date: 2021/4/25  15:50


using Common;
using UnityEngine;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJFriendRecordStatistics : FriendRecordStatistics
    {

        protected override Image m_logoImage => m_logo;

        private Image m_logo;
        private Text m_titleTxt;
        private Text m_ruleTxt;
        private RectTransform m_detailContainer;

        public MJFriendRecordStatistics()
        {
            m_logo = FindReference<Image>("logo");
            m_titleTxt = FindReference<Text>("title");
            m_ruleTxt = FindReference<Text>("rule");
            m_detailContainer = FindReference("container") as RectTransform;

            SetLogo();
        }

        public override void SetConfig(Config config)
        {
            //设置标题
            m_titleTxt.text = config.gameInfoText;

            //设置规则
            m_ruleTxt.text = "规则：" + string.Join(",", config.rules);

            //设置战绩详情
            for (int i = 0; i < config.playerInfos.Count; ++i)
            {
                PlayerInfo info = config.playerInfos[i];
                //Node2D node = OnCreateRecordItemNode(info);
                //m_detailContainer.AddChild(node);
            }
        }


        protected virtual MJRecordItemNode OnCreateRecordItemNode(PlayerInfo info)
        {
            return new MJRecordItemNode(info);
        }


        //protected override GameObject OnCreateGameObject()
        //{
        //    GameObject obj = base.OnCreateGameObject();

        //    GameObject record = Instantiate("MJCommon/Images/RecordStatistics/Prefabs/RecordStatistics.prefab");

        //    record.name = "Background";

        //    record.transform.SetParent(obj.transform);

        //    return obj;
        //}
    }
}

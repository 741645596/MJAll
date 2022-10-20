// @Author: lili
// @Date: 2021/5/19  14:37

using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using WLHall;

namespace Common
{
    public class IdentityConfig
    {
        public string nickname;
        public int gender;
        public string avatarUrl;
        public uint userId;
        public long money;
        public uint winCount;
        public uint gameCount;
        public float winRate;
    }

    public class IdentityLayer : WNode
    {

        public Action<uint> onClickKick;
        public Action<uint> onClickReport;


        //private RemoteImage m_remoteImage;
        //private Text m_userId;
        //private Text m_nickname;
        //private SKText m_shengjuCount;
        //private SKText m_duijuCount;
        //private SKText m_shenglvCount;
        //private SKText m_beanCount;
        //private Button m_kickBtn;
        //private Button m_jubaoBtn;
        //private Button m_closeBtn;

        public IdentityLayer(IdentityConfig config)
        {
            //m_remoteImage = FindReference<RemoteImage>("RemoteImage");
            //m_userId = FindReference<Text>("UserId");
            //m_nickname = FindReference<Text>("Nickname");
            //m_beanCount = FindReference<SKText>("BeanCount");
            //m_shengjuCount = FindReference<SKText>("ShengJuCount");
            //m_duijuCount = FindReference<SKText>("DuiJuCount");
            //m_shenglvCount = FindReference<SKText>("ShengLvCount");
            //m_kickBtn = FindReference<Button>("KickBtn");
            //m_jubaoBtn = FindReference<Button>("JuBaoBtn");
            //m_closeBtn = FindReference<Button>("CloseBtn");
            //m_kickBtn.onClick.AddListener(() =>
            //{
            //    onClickKick?.Invoke(config.userId);
            //    RemoveSelf();
            //});
            //m_jubaoBtn.onClick.AddListener(() => onClickReport?.Invoke(config.userId));
            //m_closeBtn.onClick.AddListener(() => RemoveSelf());

            //SetConfig(config);

            //RegisterTouchEvents2D(d => true);
        }

        /// <summary>
        /// 显示"请出房间"按钮
        /// </summary>
        public void ShowKickButton()
        {
            //m_kickBtn.gameObject.SetActive(true);
        }

        /// <summary>
        /// 显示举报按钮
        /// </summary>
        public void ShowReportButton()
        {
            //m_jubaoBtn.gameObject.SetActive(true);
        }

        /// <summary>
        /// 显示gps信息
        /// </summary>
        public void ShowGpsInfo()
        {
            // TODO: 个人信息界面显示gps信息
        }

        private void SetConfig(IdentityConfig config)
        {
            // 头像
            //string defaultImagePath = config.gender == 1 ? OverlayAvatar.DEFAULT_MAN_IMAGE_PATH : OverlayAvatar.DEFAULT_WOMAN_IMAGE_PATH;

            //m_remoteImage.sprite = AssetsLoader.Instance.Load<Sprite>(defaultImagePath);

            //m_remoteImage.url = config.avatarUrl;
            //// ID
            //m_userId.text = "ID." + config.userId;
            //// 昵称
            //m_nickname.text = config.nickname;
            //// 豆豆
            //m_beanCount.text = config.money + "";
            //// 胜局、对局、胜率
            //m_shengjuCount.text = config.winCount + "";

            //m_duijuCount.text = config.gameCount + "";

            //m_shenglvCount.text = config.winRate + "%";
        }


        private void RemoveSelf()
        {
            // TODO：魔法道具

            //RemoveFromParent();
        }


        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/Identity/Prafabs/IdentityLayer.prefab");
        //}
    }
}

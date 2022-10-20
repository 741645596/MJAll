// @Author: tanjinhua
// @Date: 2021/5/28  0:16


using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine.UI;

namespace Common
{
    public abstract class FriendRecordStatistics : WLayer
    {
        /// <summary>
        /// 战绩统计玩家信息
        /// </summary>
        public struct PlayerInfo
        {
            public string nickname;             // 昵称
            public string avatarUrl;            // 头像地址
            public uint userId;                 // 玩家id
            public int gender;                  // 性别 1-男 0-女
            public bool isRoomOwner;            // 是否是房主
            public bool isSelfPlayer;           // 是否是自己
            public bool applyDissolution;       // 是否是申请解散玩家
            public bool overtimeDissolution;    // 是否是超时解散玩家
            public long score;                  // 最终得分
            public int rank;                    // 排名
            public Dictionary<string, string> details;  //详细结算信息

        }

        /// <summary>
        /// 战绩统计参数配置
        /// </summary>
        public struct Config
        {
            public string gameInfoText;             // 游戏名称、房间号、局数等游戏信息字符串
            public string[] rules;                   // 规则（数组）
            public List<PlayerInfo> playerInfos;           // 玩家信息
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        public Action<FriendRecordButton, FriendRecordButton.Type> onClickButton;

        protected abstract Image m_logoImage { get; }
        //private Node2D m_buttonContainer;
        private List<FriendRecordButton> m_buttons;


        public FriendRecordStatistics()
        {
            //m_buttonContainer = FindReference("ButtonContainer") as Node2D;

            //m_buttonContainer.zorder = 1;

            m_buttons = new List<FriendRecordButton>();
        }

        /// <summary>
        /// 设置参数配置
        /// </summary>
        /// <param name="config"></param>
        public abstract void SetConfig(Config config);

        /// <summary>
        /// 设置logo
        /// </summary>
        protected virtual void SetLogo()
        {
            // TODO: 平台判断

            //string path = "Common/RecordStatistics/Sprites/weile.png";

            //m_logoImage.sprite = AssetsLoader.Instance.Load<Sprite>(path);
        }

        /// <summary>
        /// 添加按钮
        /// </summary>
        /// <param name="type"></param>
        public void AddButton(FriendRecordButton.Type type)
        {
            FriendRecordButton button = new FriendRecordButton(type);

            button.onClick = t => onClickButton?.Invoke(button, t);

            //m_buttonContainer.AddChild(button);

            m_buttons.Add(button);

            LayoutButtons();
        }

        /// <summary>
        /// 添加多个按钮
        /// </summary>
        /// <param name="types"></param>
        public void AddButtons(List<FriendRecordButton.Type> types)
        {
            types.ForEach(t => AddButton(t));
        }


        private void LayoutButtons()
        {
            //float spacing = 20;

            int row = 5;

            for (int i = 0; i < m_buttons.Count; i++)
            {
                int index = i % row;

                //Vector2 layout = i < row ? Layouts.rightCenter : Layouts.leftCenter;

                //float offsetY = index * (102f + spacing) - ScreenAdapter.displaySize.y / 4f - 60f;

                //Vector2 offset = i < row ? new Vector2(-85f, offsetY) : new Vector2(75f, offsetY);

                //m_buttons[i].LayoutWithParent(layout, offset, true);
            }
        }


        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/RecordStatistics/Prafabs/RecordStatistics.prefab");
        //}
    }
}

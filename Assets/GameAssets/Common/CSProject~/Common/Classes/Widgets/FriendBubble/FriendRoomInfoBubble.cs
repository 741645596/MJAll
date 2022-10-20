// @Author: lili
// @Date: 2021/5/31 10:00:14

using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;

namespace Common
{
    /// <summary>
    /// 朋友场信息气泡（显示房号、时间电量，麻将用）
    /// </summary>
    public class FriendRoomInfoBubble : WNode
    {
        //private Text m_roomKey;
        //private SKText m_time;
        //private Node2D m_batteryNode;
        //private Node2D m_precentNode;
        private Button m_button;

        public Action onClick;


        public FriendRoomInfoBubble()
        {
            //m_roomKey = FindReference<Text>("RoomKey");
            //m_time = FindReference<SKText>("Time");
            //m_batteryNode = FindReference("BatteryBG") as Node2D;
            //m_precentNode = FindReference("BatteryPercent") as Node2D;
            m_button = gameObject.GetComponent<Button>();
            m_button.onClick.AddListener(() => onClick?.Invoke());

            Update();

        }

        /// <summary>
        /// 更新房号
        /// </summary>
        /// <param name="roomKey"></param>
        public void UpdateRoomKey(string roomKey)
        {
            //m_roomKey.text = roomKey;
        }

        private void Update()
        {
            UpdateTime();

            // TODO:设置电量百分比

            //Invoke(5, Update);
        }

        private void UpdateTime()
        {
            DateTime dt = DateTime.Now;
            //m_time.text = dt.ToShortTimeString();
        }

        /// <summary>
        /// 设置电量百分比
        /// </summary>
        /// <param name="percent">0～100</param>
        public void SetBatteryPercent(float percent)
        {
            percent = percent > 100 ? 100 : percent;
            percent = percent < 0 ? 0 : percent;
            //m_precentNode.scale = new Vector2(percent / 100, 1);
        }

        public void HideBattery()
        {
            //m_batteryNode.gameObject.SetActive(false);
        }

        //protected override GameObject OnCreateGameObject()
        //{
        //    return Instantiate("Common/FriendBubble/Prafabs/FriendRoomInfoBubble.prefab");
        //}
    }
}

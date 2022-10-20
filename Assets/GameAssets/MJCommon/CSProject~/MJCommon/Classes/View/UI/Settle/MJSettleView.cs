// @Author: tanjinhua
// @Date: 2021/9/10  14:40

using Common;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJSettleView : SettleViewBase
    {

        public MJSettleView()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "settle_view_normal.prefab");

            FindReference<Button>("continue").onClick.AddListener(() => onEvent?.Invoke(SettleEvent.Continue));
            FindReference<Button>("changedesk").onClick.AddListener(() => onEvent?.Invoke(SettleEvent.ChangeDesk));
            FindReference<Button>("leave").onClick.AddListener(() => onEvent?.Invoke(SettleEvent.Leave));
        }

        public override void Countdown(int time)
        {

        }

        public override void ShowFriendGameOverState()
        {

        }

        public void HideButtons()
        {
            FindReference("buttons").gameObject.SetActive(false);
        }
    }
}

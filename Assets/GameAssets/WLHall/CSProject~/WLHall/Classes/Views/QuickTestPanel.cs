// @Author: tanjinhua
// @Date: 2021/8/18  15:05


using Unity.Widget;
using UnityEngine.UI;

namespace WLHall
{
    public class QuickTestPanel : WNode
    {

        public QuickTestPanel()
        {
            InitGameObject("WLHall/Main/hall_quicktest", "quick_test_panel.prefab");

            FindReference<Text>("QualityText").text = $"Quality : {QualityManager.GetQualityDes()}, Ver : {QualityManager.CheckVerValue()}";

            foreach (var pair in QuickConfig.testRoomIds)
            {
                FindReference<Button>(pair.Key).onClick.AddListener(() =>
                    GameAppStage.Instance.hallManager.JoinRoom(pair.Value));
            }

            foreach(var pair in QuickConfig.friendRoomIds)
            {
                FindReference<Button>(pair.Key).onClick.AddListener(() =>
                    GameAppStage.Instance.hallManager.CreateFriendRoom(QuickConfig.shortName, pair.Value, QuickConfig.rules));
            }

            var field = FindReference<InputField>("room_key_field");
            field.onValueChanged.AddListener(txt =>
            {
                if (txt.Length == 6)
                {
                    GameAppStage.Instance.hallManager.JoinFriendRoom(txt);
                }
            });
        }
    }
}

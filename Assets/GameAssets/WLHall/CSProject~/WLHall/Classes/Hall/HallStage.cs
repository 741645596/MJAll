// @Author: tanjinhua
// @Date: 2021/8/13  14:31

using UnityEngine.SceneManagement;
using Unity.Widget;
using WLCore.Stage;

namespace WLHall
{
    public class HallStage : BaseStage
    {
        /// <summary>
        /// 进入大厅界面
        /// </summary>
        public static void EnterHallStage()
        {
            StageManager.RunStage(new HallStage());
        }

        public override void OnInitialize()
        {
            LoadScene("CommonScene", () => OnSceneDidLoad());
        }

        private void OnSceneDidLoad()
        {
            // TODO: 显示大厅场景、UI
            WLayer.Create("WLHall/Main/hall_f_other", "SkyCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.Sky);

            WLayer.Create("WLHall/Main/hall_f_other", "CloudCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.Cloud);

            WLayer.Create("WLHall/Main/hall_f_other", "BuildingCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.Building);

            WLayer.Create("WLHall/Main/hall_f_other", "DynBuildingCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.DynBuilding);

            WLayer.Create("WLHall/Main/hall_f_other", "RoomBgCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.RoomBg);

            WLayer.Create("WLHall/Main/hall_f_other", "SofaCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), HallZorder.Sofa);

            HallDeskCanvas.Create()
                .AddTo(WDirector.GetRootLayer(), HallZorder.Desk);

            HallTvCanvas.Create()
                .AddTo(WDirector.GetRootLayer(), HallZorder.Tv);

            HallBtnCanvas.Create()
                .AddTo(WDirector.GetRootLayer(), HallZorder.Btn);

            // 显示测试房间列表
            var quickPanel = new QuickTestPanel();
            quickPanel.AddTo(WDirector.GetRootLayer());
        }
    }
}

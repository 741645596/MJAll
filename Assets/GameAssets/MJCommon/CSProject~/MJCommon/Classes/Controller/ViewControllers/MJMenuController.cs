// @Author: tanjinhua
// @Date: 2021/10/22  9:39


using Common;
using Unity.Widget;

namespace MJCommon
{
    public class MJMenuController : GameMenuController
    {
        protected override GameMenu OnCreateGameMenu()
        {
            var menu = new GameMenu();

            menu.AddTo(WDirector.GetRootLayer(), MJZorder.GameMenu);

            return menu;
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/5/2  21:36


using Unity.Widget;

namespace MJCommon
{
    public class MJGameStartAnimation : WNode
    {
        public MJGameStartAnimation()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "game_start.prefab");

            DelayInvoke(0.5f, () => RemoveFromParent());
        }
    }
}

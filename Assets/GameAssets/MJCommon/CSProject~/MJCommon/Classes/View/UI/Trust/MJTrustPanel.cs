// @Author: tanjinhua
// @Date: 2021/9/7  11:13

using Common;
using UnityEngine.UI;

namespace MJCommon
{
    public class MJTrustPanel : TrustPanel
    {
        public MJTrustPanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "trust_panel.prefab");

            FindReference<Button>("cancel_btn").onClick.AddListener(() =>
            {
                onClickCancel?.Invoke();

                RemoveFromParent();
            });
        }
    }
}

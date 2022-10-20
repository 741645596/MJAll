// @Author: tanjinhua
// @Date: 2021/4/13  18:45


using System;
using System.Collections.Generic;
using Unity.Widget;

namespace MJCommon
{
    public class MJMeldSelectPanel : WNode
    {

        public Action<MJActionData> onSelect;


        public MJMeldSelectPanel(List<MJActionData> actionDatas)
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "meld_select_panel.prefab");

            actionDatas.ForEach(data =>
            {
                var selection = new MJMeldSelection(data);

                selection.AddTo(this);

                selection.onClick = OnSelect;
            });
        }


        private void OnSelect(MJActionData data)
        {
            onSelect?.Invoke(data);

            RemoveFromParent();
        }
    }
}

using System;
using UnityEngine.UI;
using WLCore.Helper;

namespace Common
{
    public class TableViewCellCase : TableViewCell
    {
        private Text _text;
        public TableViewCellCase()
        {
            // 必须：创建预设之后调用InitGameObject
            var obj = ObjectHelper.Instantiate("Common/Module2/prefabs3", "TableViewCell.prefab");
            InitGameObject(obj);

            // 根据自己业务需求先保存控件索引
            var bt = obj.FindReference<Button>("Button");
            bt.onClick.AddListener(() =>
            {
                WLDebug.Log(_text.text);
            });
            _text = obj.FindReference<Text>("Text");
        }

        public void SetString(string str)
        {
            _text.text = str;
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/4/13  9:50


using System;
using UnityEngine;
using Unity.Widget;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace MJCommon
{
    public class MJActionButtonPanel : WLayer
    {
        public Action<MJActionData> onSelectAction;

        private RectTransform _container;

        public MJActionButtonPanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "action_button_panel.prefab");

            _container = FindReference("container") as RectTransform;
        }


        public void Show(Dictionary<KeyValuePair<string, string>, List<MJActionData>> datas)
        {
            Clear();

            foreach (var pair in datas)
            {
                var config = pair.Key;

                List<MJActionData> classifiedDatas = pair.Value;

                MJActionButton button = new MJActionButton(config.Key, config.Value, classifiedDatas);

                button.onClick += OnClickButton;

                button.gameObject.transform.SetParent(_container, false);

                button.gameObject.transform.SetAsFirstSibling();
            }
        }


        public void Clear()
        {
            var count = _container.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform child = _container.GetChild(i);

                Object.Destroy(child.gameObject);
            }
        }


        protected virtual void OnClickButton(List<MJActionData> classifiedDatas)
        {
            if (classifiedDatas.Count < 2)
            {
                onSelectAction?.Invoke(classifiedDatas[0]);

                return;
            }

            MJMeldSelectPanel selectPanel = OnCreateMeldSelectPanel(classifiedDatas);

            selectPanel.onSelect += data => onSelectAction?.Invoke(data);

            selectPanel.AddTo(this);
        }


        protected virtual MJMeldSelectPanel OnCreateMeldSelectPanel(List<MJActionData> classifiedDatas)
        {
            return new MJMeldSelectPanel(classifiedDatas);
        }
    }
}

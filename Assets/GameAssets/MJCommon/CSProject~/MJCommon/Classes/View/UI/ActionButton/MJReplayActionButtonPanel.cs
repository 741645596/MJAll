// @Author: tanjinhua
// @Date: 2021/12/20  15:43

using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJReplayActionButtonPanel : WLayer
    {
        private RectTransform[] _containers;
        private RectTransform _poolContainer;
        private Dictionary<string, List<MJActionButton>> _pool;
        private Dictionary<int, List<MJActionButton>> _inuse;

        public MJReplayActionButtonPanel()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "replay_action_button_panel.prefab");

            _poolContainer = new GameObject("pool", typeof(RectTransform)).transform as RectTransform;
            _poolContainer.SetParent(rectTransform, false);
            _poolContainer.gameObject.SetActive(false);
            _pool = new Dictionary<string, List<MJActionButton>>();

            _inuse = new Dictionary<int, List<MJActionButton>>();
            _containers = new RectTransform[4];
            for (int i = 0; i < 4; i++)
            {
                _containers[i] = FindReference<RectTransform>("container_"+i);
                _inuse[i] = new List<MJActionButton>();
            }
        }

        public RectTransform GetContainer(int viewChairId)
        {
            return _containers[viewChairId];
        }

        public void Show(int viewChairId,  List<KeyValuePair<string, string>> configs, int selectIndex)
        {
            Clear(viewChairId);

            var container = _containers[viewChairId];
            for (int i = 0; i < configs.Count; i++)
            {
                var config = configs[i];
                var button = GetFromPool(config.Key, config.Value);
                button.transform.SetParent(container, false);

                if (i == selectIndex)
                {
                    button.ShowReplaySelectMark();
                }

                _inuse[viewChairId].Add(button);
            }
        }

        public void Clear(int viewChairId)
        {
            var list = _inuse[viewChairId];

            if (list.Count == 0)
            {
                return;
            }

            for (int i = list.Count - 1; i >= 0; i--)
            {
                Recycle(list[i]);
                list.RemoveAt(i);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < 4; i++)
            {
                Clear(i);
            }
        }

        private MJActionButton GetFromPool(string asset, string key)
        {
            var identifier = asset + key;

            if (_pool.ContainsKey(identifier))
            {
                var list = _pool[identifier];

                if (list.Count > 0)
                {
                    var btn = list[0];
                    btn.gameObject.SetActive(true);
                    list.RemoveAt(0);
                    return btn;
                }
            }

            var newBtn = new MJActionButton(asset, key, null);
            return newBtn;
        }

        private void Recycle(MJActionButton button)
        {
            button.rectTransform.SetParent(_poolContainer, false);
            button.HideReplaySelectMark();
            button.gameObject.SetActive(false);

            var identifier = button.identifier;

            List<MJActionButton> list;
            if (_pool.ContainsKey(identifier))
            {
                list = _pool[identifier];
            }
            else
            {
                list = new List<MJActionButton>();
            }
            list.Add(button);
            _pool[identifier] = list;
        }
    }
}

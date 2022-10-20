// @Author: tanjinhua
// @Date: 2021/11/22  15:58


using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJRTCardSet : WNode
    {
        private Transform _container;
        private RTSystem _rTSystem;
        private List<MJMeld> _melds;
        private MJHandSet _handSet;

        private float _spacing = 0.01f;
        private float _aspect;
        private float _startX;

        public MJRTCardSet()
        {
            InitGameObject("MJCommon/MJ/mj_ui_prefabs", "rt_card_set_image.prefab");

            _container = FindInChildren("rt_sys/container");

            _rTSystem = FindInChildren<RTSystem>("rt_sys");

            _aspect = (float)_rTSystem.targetTexture.width / _rTSystem.targetTexture.height;

            _startX = -_rTSystem.cameraSettings.orthographicSize * _aspect;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="meldArgs"></param>
        /// <param name="handValues"></param>
        public void LoadData(List<MJMeld.Args> meldArgs, List<int> handValues)
        {
            Clear();

            LoadMelds(meldArgs);

            LoadHand(handValues);

            Refresh();

            UpdateContainerScale();
        }

        /// <summary>
        /// 如果对image进行缩放，需要调用此方法修正3D物体的缩放
        /// </summary>
        public void UpdateContainerScale()
        {
            var rootUIScale = WDirector.GetUIRootObject().transform.lossyScale;
            var scale = transform.localScale;
            _container.localScale = new Vector3(1f / (rootUIScale.x * scale.x), 1f / (rootUIScale.y * scale.y), 1f / (rootUIScale.z * scale.z));
        }

        /// <summary>
        /// 清除所有牌
        /// </summary>
        public void Clear()
        {
            _melds?.ForEach(m =>
            {
                m.Traverse((card, i) => MJCardPool.Recycle(card));
                m.Destroy();
            });

            _melds?.Clear();

            _handSet?.Clear();
        }


        private void LoadMelds(List<MJMeld.Args> args)
        {
            if (_melds == null)
            {
                _melds = new List<MJMeld>();
            }

            args.ForEach(a =>
            {
                var meld = new MJMeld(a);
                meld.gameObject.transform.SetParent(_container, false);
                meld.layer = _rTSystem.layerIndex;
                meld.Traverse((c, i) => c.HideShadow());
                _melds.Add(meld);
            });
        }

        private void LoadHand(List<int> handValues)
        {
            if (_handSet == null)
            {
                _handSet = new MJHandSet();
                _handSet.gameObject.transform.SetParent(_container, false);
                _handSet.layer = _rTSystem.layerIndex;
                _handSet.anchor = HandCardAnchor.Left;
            }

            _handSet.Reload(handValues);
            _handSet.Refresh();
            _handSet.Traverse((c, i) => c.HideShadow());
        }

        private void Refresh()
        {
            float x = _startX;
            _melds.ForEach(m =>
            {
                m.gameObject.transform.localPosition = new Vector3(x, -0.0268f, 0);
                x += m.GetSize().x + _spacing;
            });

            _handSet.gameObject.transform.localPosition = new Vector3(x, -0.046f, 0);
            _handSet.gameObject.transform.localEulerAngles = new Vector3(30, 0, 0);
        }
    }
}

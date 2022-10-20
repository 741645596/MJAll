// @Author: tanjinhua
// @Date: 2021/4/15  11:00

using System;
using System.Collections.Generic;
using Unity.Widget;
using UnityEngine;

namespace MJCommon
{
    public class MJActionEffectPanel : WLayer
    {
        public Func<int, Vector2> onGetEffectPos;

        private RectTransform _poolContriner;
        private Dictionary<string, List<MJActionEffect>> _pool;
        private MJActionEffect[] _inuse;

        public MJActionEffectPanel()
        {
            var obj = CanvasHelper.CreateEmptyCanvas();
            obj.name = "action_effect_panel";
            InitGameObject(obj);

            _poolContriner = new GameObject("pool", typeof(RectTransform)).transform as RectTransform;
            _poolContriner.SetParent(rectTransform, false);
            _poolContriner.gameObject.SetActive(false);
            _pool = new Dictionary<string, List<MJActionEffect>>();
            _inuse = new MJActionEffect[4];
        }


        /// <summary>
        /// 播放特效
        /// </summary>
        /// <param name="viewChairId"></param>
        /// <param name="config"></param>
        public void PlayEffect(int viewChairId, KeyValuePair<string, string> config)
        {
            ClearEffect(viewChairId);

            var effect = GetFromPool(config.Key, config.Value);
            effect.AddTo(this);
            effect.Layout(new Vector2(0.5f, 0.5f), GetOffset(viewChairId));
            _inuse[viewChairId] = effect;
        }


        /// <summary>
        /// 播放自己自摸特效
        /// </summary>
        public void PlaySelfZimoEffect()
        {
            ClearEffect(Chair.Down);

            var asset = "MJCommon/MJ/mj_ui_effe_tishi";
            var key = "zimo_h_01_01.prefab";
            var effect = GetFromPool(asset, key);
            effect.AddTo(this);
            effect.Layout(new Vector2(0.5f, 0.5f), GetOffset(Chair.Down) + new Vector2(0, 50));
            _inuse[Chair.Down] = effect;
        }


        /// <summary>
        /// 清理特效
        /// </summary>
        /// <param name="viewChairId"></param>
        public void ClearEffect(int viewChairId)
        {
            var effect = _inuse[viewChairId];

            if (effect != null && effect.gameObject != null)
            {
                Recycle(effect);
            }

            _inuse[viewChairId] = null;
        }


        /// <summary>
        /// 清理所有特效
        /// </summary>
        public void ClearEffect()
        {
            for (int i = 0; i < _inuse.Length; i++)
            {
                ClearEffect(i);
            }
        }

        public Vector2 GetOffset(int viewChairId)
        {
            return new List<Vector2>
            {
                new Vector2(0, -200),
                new Vector2(500, 110),
                new Vector2(0, 380),
                new Vector2(-500, 110)
            }[viewChairId];
        }

        private MJActionEffect GetFromPool(string asset, string key)
        {
            var identifier = asset + key;

            if (_pool.ContainsKey(identifier))
            {
                var list = _pool[identifier];

                if (list.Count > 0)
                {
                    var effect = list[0];
                    effect.gameObject.SetActive(true);
                    list.RemoveAt(0);
                    return effect;
                }
            }

            return new MJActionEffect(asset, key);
        }

        private void Recycle(MJActionEffect effect)
        {
            effect.rectTransform.SetParent(_poolContriner, false);
            effect.gameObject.SetActive(false);

            var identifier = effect.identifier;

            List<MJActionEffect> list;
            if (_pool.ContainsKey(identifier))
            {
                list = _pool[identifier];
            }
            else
            {
                list = new List<MJActionEffect>();
            }
            list.Add(effect);
            _pool[identifier] = list;
        }
    }
}

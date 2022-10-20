// @Author: tanjinhua
// @Date: 2021/11/12  14:35

using Unity.Widget;
using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;

namespace MJCommon
{
    public class MJExchangeAnimation : WNode3D
    {

        private Animator _animator;
        private List<GameObject> _cards;
        private List<GameObject> _crossGlowEffects;


        public MJExchangeAnimation()
        {
            InitGameObject("MJCommon/MJ/mj_zm_effe_tishi", "huan_pai_anim/huan_pai_anim.prefab");
            gameObject.name = "MJExchangeAnimation";

            _animator = gameObject.GetComponent<Animator>();

            _cards = new List<GameObject>();
            _cards.Add(FindInChildren("down").gameObject);
            _cards.Add(FindInChildren("right").gameObject);
            _cards.Add(FindInChildren("up").gameObject);
            _cards.Add(FindInChildren("left").gameObject);

            _crossGlowEffects = new List<GameObject>();
            _crossGlowEffects.Add(FindInChildren("fx_dj_x_01_01/fx_dj_glow_down").gameObject);
            _crossGlowEffects.Add(FindInChildren("fx_dj_x_01_01/fx_dj_glow_right").gameObject);
            _crossGlowEffects.Add(FindInChildren("fx_dj_x_01_01/fx_dj_glow_up").gameObject);
            _crossGlowEffects.Add(FindInChildren("fx_dj_x_01_01/fx_dj_glow_left").gameObject);
        }

        public void ShowCard(int viewChairId)
        {
            _cards[viewChairId].SetActive(true);
            _crossGlowEffects[viewChairId].SetActive(true);
        }

        public void Play(ExchangeCardType type, Action onFinished = null)
        {
            _animator.SetInteger("Type", (int)type);
            _animator.SetTrigger("Start");

            if (onFinished != null)
            {
                var seq = DOTween.Sequence();
                seq.AppendInterval(2.2f);
                seq.AppendCallback(() => onFinished.Invoke());
                seq.SetTarget(gameObject);
                seq.SetLink(gameObject);
                seq.Play();
            }
        }
    }
}

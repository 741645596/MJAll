// MJHandCard.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/27

using Unity.Core;
using UnityEngine;

namespace MJCommon
{
    public class MJHandCard : MJCard
    {
        public MJHandCard() : base()
        {
        }

        public MJHandCard(int value) : base(value)
        {
        }

        public override void Initialize(int value)
        {
            cardValue = value;

            var key = $"prefabs/mj_card_2_up.prefab";
            var prefab = AssetsManager.Load<GameObject>("MJCommon/MJ/mj_cards", key);

            if (prefab != null)
            {
                gameObject = new GameObject($"mj_handcard_{cardValue}");
                body = Object.Instantiate(prefab);
                body.transform.SetParent(gameObject.transform, false);
                renderer = body.GetComponent<MeshRenderer>();
                SetupMaterial();
            }
            else
            {
                Debug.Log("MJHandCard prefab is null key=" + key);
            }
        }
    }
}

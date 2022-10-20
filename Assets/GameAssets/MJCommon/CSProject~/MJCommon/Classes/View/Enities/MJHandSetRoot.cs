// MJHandSetRoot.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/8/6

using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    public class MJHandSetRoot : BaseEntity
    {
        public MJHandSetConfig config;

        protected Dictionary<MJOrientation, MJHandSet> hands;

        public MJHandSetRoot(MJHandSetConfig config)
        {
            gameObject = new GameObject("MJHandSetRoot");
            this.config = config;

            hands = new Dictionary<MJOrientation, MJHandSet>();

            for (int i = 0; i < this.config.position.Length; i++)
            {
                MJOrientation orientation = (MJOrientation)i;
                var hand = new MJHandSet();
                hand.SetOrientation(orientation);
                hand.UpdateConfig(config);
                var position = this.config.position[i].ToVector3();

                hand.gameObject.transform.localPosition = position;
                hand.SetParent(this);

                hands.Add(orientation, hand);
            }
        }

        public void InitializeHands(Camera handCamera, Camera mainCamera)
        {
            foreach (var pair in hands)
            {
                if (pair.Key == MJOrientation.Down)
                {
                    pair.Value.Initialize(config.maxHandCardCount, handCamera);
                    pair.Value.layer = LayerMask.NameToLayer(MJDefine.MJHandMask);
                }
                else
                {
                    pair.Value.Initialize(config.maxHandCardCount, mainCamera);
                }
            }
        }

        public MJHandSet GetMJHandSet(MJOrientation orientation)
        {
            if (hands.ContainsKey(orientation))
            {
                return hands[orientation];
            }
            return null;
        }

        public Dictionary<MJOrientation, MJHandSet> GetMJHandSets()
        {
            return hands;
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}

// MJWinCardRoot.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/9/8
using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{
    /// <summary>
    /// 麻将胡牌区根结点
    /// </summary>
    public class MJWinCardRoot : BaseEntity
    {
        protected Dictionary<MJOrientation, MJWinCardSet> sets;

        public MJWinCardRoot(MJWinSetConfig config)
        {
            Initialize(config);
        }

        public void Initialize(MJWinSetConfig config)
        {
            gameObject.name = "MJWinCardRoot";
            sets = new Dictionary<MJOrientation, MJWinCardSet>();

            for (int i = 0; i < config.position.Length; i++)
            {
                var pos = config.position[i].ToVector3();
                var orientation = (MJOrientation)i;
                var meldStack = new MJWinCardSet();
                meldStack.SetOrientation(orientation);
                meldStack.gameObject.transform.localPosition = pos;
                var scale = (float)config.scale[i];
                meldStack.gameObject.transform.localScale = new Vector3(scale, 1, scale);
                meldStack.SetParent(this);

                sets.Add(orientation, meldStack);
            }
        }

        public MJWinCardSet GetMJWinSet(MJOrientation orientation)
        {
            if (sets.ContainsKey(orientation))
            {
                return sets[orientation];
            }

            return null;
        }
    }
}

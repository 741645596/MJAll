// MJMeldRoot.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/08/05

using System.Collections.Generic;
using UnityEngine;
using WLCore.Entity;

namespace MJCommon
{ 
    /// <summary>
    /// 麻将副子根结点，管理最多4家的副子控件
    /// </summary>
    public class MJMeldRoot : BaseEntity
    {
        /// <summary>
        /// 配置
        /// </summary>
        public MJMeldConfig config;

        protected Dictionary<MJOrientation, MJMeldStack> meldSets;

        public MJMeldRoot(MJMeldConfig config)
        {
            this.config = config;
            gameObject = new GameObject("MJMeldRoot");
            meldSets = new Dictionary<MJOrientation, MJMeldStack>();

            for (int i = 0; i < config.position.Length; i++)
            {
                var pos = config.position[i].ToVector3();
                var orientation = (MJOrientation)i;
                var meldStack = new MJMeldStack();
                meldStack.UpdateConfig(config);
                meldStack.SetOrientation(orientation);
                meldStack.gameObject.transform.localPosition = pos;
                var scale = (float)config.scale[i];
                meldStack.gameObject.transform.localScale = new Vector3(scale, 1, scale);
                meldStack.SetParent(this);

                meldSets.Add(orientation, meldStack);
            }
        }

        public MJMeldStack GetMJMeldStack(MJOrientation orientation)
        {
            if(meldSets.ContainsKey(orientation))
            {
                return meldSets[orientation];
            }

            return null;
        }

        public void PushMeld(MJOrientation orientation, MJMeld meld)
        {
            var stack = GetMJMeldStack(orientation);
            if(stack != null)
            {
                stack.Append(meld);
            }
        }

        public void PopMeld(MJOrientation orientation)
        {
            var stack = GetMJMeldStack(orientation);
            if (stack != null)
            {
                stack.RemoveLast();
            }
        }

        protected override GameObject CreateGameObject()
        {
            return null;
        }
    }
}

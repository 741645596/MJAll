using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Core;

namespace Unity.Widget
{
    public class WSpineNode : WNode
    {
        public SkeletonGraphic skeletonGraphic;

        public static WSpineNode Create(string assertName, string matKey, string assetKey)
        {
            var node = new WSpineNode();
            var spine = SpineHelper.Create(assertName, matKey, assetKey);
            node.InitGameObject(spine);
            node._Init();
            return node;
        }

        public void SetAnimation(int trackIndex, string animationName, bool loop)
        {
            //skeletonGraphic.Skeleton.SetToSetupPose();
            //skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.AnimationState.SetAnimation(trackIndex, animationName, loop);
        }

        public void ClearTracks()
        {
            skeletonGraphic.AnimationState.ClearTracks();
        }

        public void SetCompleteListener(Action<Spine.TrackEntry> callback)
        {
            skeletonGraphic.AnimationState.Complete += (t)=>
            {
                callback?.Invoke(t);
            };
        }

        public void SetTimeScale(float timeScale)
        {
            skeletonGraphic.timeScale = timeScale;
        }

        /// <summary>
        /// 停止播放动画
        /// </summary>
        /// <param name="freeze">true停止动画播放</param>
        public void SetFreeze(bool freeze)
        {
            skeletonGraphic.freeze = freeze;
        }

        /// <summary>
        /// 动画是否停止
        /// </summary>
        /// <returns></returns>
        public bool IsFreeze()
        {
            return skeletonGraphic.freeze;
        }

        private void _Init()
        {
            skeletonGraphic = gameObject.GetComponent<SkeletonGraphic>();
        }

        // 禁止new
        private WSpineNode()
        {
        }
    }
}


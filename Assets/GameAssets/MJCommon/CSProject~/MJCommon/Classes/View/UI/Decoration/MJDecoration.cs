// @Author: tanjinhua
// @Date: 2021/10/11  15:10


using Spine.Unity;
using Unity.Widget;
using Spine;

namespace MJCommon
{
    public class MJDecoration : WLayer
    {

        private SkeletonGraphic _graphic;

        public MJDecoration()
        {
            InitGameObject("MJCommon/MJ/mj_env", "prefabs/flower.prefab");

            _graphic = FindReference<SkeletonGraphic>("graphic");

            _graphic.AnimationState.Complete += OnTrackConpleted;
        }

        public void PlayShake()
        {
            _graphic.AnimationState.SetAnimation(0, "idle2", false);
        }

        private void OnTrackConpleted(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == "idle2")
            {
                _graphic.AnimationState.SetAnimation(0, "idle", true);
            }
        }
    }
}

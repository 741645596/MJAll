// @Author: tanjinhua
// @Date: 2021/8/20  14:09


using DG.Tweening;
using Unity.Widget;
using UnityEngine;
using WLHall.Game;

namespace MJCommon
{
    /// <summary>
    /// 背景、主摄像机等基础场景控件控制器
    /// </summary>
    public class MJEnvController : BaseGameController
    {
        private MJMainCamera _mainCamera;
        private MJDecoration _decoration;
        private WNode3D _flowerParticle;
        private Transform _envParent;

        public override void OnSceneLoaded()
        {
            _envParent = new GameObject("MJEnvironment").transform;

            var space = stage.GetController<MJSpaceController>().GetSpace();
            _mainCamera = new MJMainCamera();
            _mainCamera.AddOverlayCamera(space.handSetCamera.camera);
            _mainCamera.AddOverlayCamera(CameraUtil.GetUICamera());

            var background = WNode3D.Create("MJCommon/MJ/mj_env", "prefabs/mj_background.prefab");
            background.gameObject.transform.SetParent(_envParent);

            _decoration = new MJDecoration();
            _decoration.AddTo(WDirector.GetRootLayer(), MJZorder.HandCardControl);

            // 循环花瓣粒子特效
            var p1 = WNode3D.Create("MJCommon/MJ/mj_cj_effe_fengwei", "huaban_p_01.prefab");
            p1.transform.SetParent(_envParent, false);
            p1.transform.localPosition = new Vector3(-0.729f, 0.209f, 0.131f);
            p1.transform.localEulerAngles = new Vector3(90, 90, 0);

            // 茶壶白汽
            var p2 = WNode3D.Create("MJCommon/MJ/mj_cj_effe_fengwei", "ywu.prefab");
            p2.transform.SetParent(_envParent);

        }


        /// <summary>
        /// 播放镜头拉远效果
        /// </summary>
        public void PlayZoomOut()
        {
            _mainCamera.PlayZoomOut();
        }


        /// <summary>
        /// 播放场景抖动动画
        /// </summary>
        public void PlayShake()
        {
            _mainCamera.PlayShake();

            _decoration.PlayShake();

            if (_flowerParticle != null && _flowerParticle.gameObject != null)
            {
                _flowerParticle.RemoveFromParent();

                _flowerParticle = null;
            }

            _flowerParticle = WNode3D.Create("MJCommon/MJ/mj_cj_effe_fengwei", "huaban_p_02.prefab");
            _flowerParticle.transform.SetParent(_envParent, false);
            _flowerParticle.transform.localPosition = new Vector3(-0.729f, 0.209f, 0.131f);
            _flowerParticle.transform.localEulerAngles = new Vector3(90, 90, 0);
            var seq = DOTween.Sequence().AppendInterval(3f).AppendCallback(() => _flowerParticle.RemoveFromParent());
            seq.SetTarget(_flowerParticle.gameObject);
            seq.SetLink(_flowerParticle.gameObject);
            seq.SetAutoKill(true);
            seq.Play();
        }


        /// <summary>
        /// 立即停止主摄像机动作，并重置参数
        /// </summary>
        public void ResetMainCamera()
        {
            _mainCamera.Reset();
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/8/20  14:06


using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WLCore.Entity;

namespace MJCommon
{
    public class MJMainCamera : BaseEntity
    {
        public Camera camera { get; private set; }

        private float _desinnFov;
        private Vector3 _designEuler;
        private UniversalAdditionalCameraData _cameraData;

        public MJMainCamera()
        {
            gameObject.name = "MainCamera";

            camera = gameObject.GetComponent<Camera>();

            _cameraData = gameObject.GetComponent<UniversalAdditionalCameraData>();

            var designResolution = DesignResolution.GetDesignSize();
            var designAspect = designResolution.x / designResolution.y;
            var designFov = camera.fieldOfView;
            if (display.aspect >= designAspect)
            {
                CameraUtil.SetupPerspectiveCameraByFixedHeight(camera, designAspect, designFov);
            }

            _desinnFov = camera.fieldOfView;
            _designEuler = camera.transform.eulerAngles;
        }


        /// <summary>
        /// 添加叠加摄像机
        /// </summary>
        /// <param name="camera"></param>
        public void AddOverlayCamera(Camera camera)
        {
            _cameraData.cameraStack.Add(camera);
        }


        /// <summary>
        /// 近景
        /// </summary>
        public void ZoomIn()
        {
            camera.fieldOfView = 18f;
            //camera.transform.eulerAngles = new Vector3(43.15f, 0, 0);
        }


        /// <summary>
        /// 播放镜头拉远效果
        /// </summary>
        public void PlayZoomOut()
        {
            DOTween.Kill(camera.gameObject);

            ZoomIn();

            var seq = DOTween.Sequence();
            //seq.Join(camera.transform.DORotate(_designEuler, 1.2f, RotateMode.FastBeyond360));
            seq.Join(DOTween.To(() => camera.fieldOfView, s => camera.fieldOfView = s, _desinnFov, 1.8f));
            seq.AppendCallback(() => Reset());
            seq.SetTarget(camera.gameObject);
            seq.SetLink(camera.gameObject);
            seq.SetAutoKill(true);
            seq.Play();
        }


        /// <summary>
        /// 播放镜头抖动效果
        /// </summary>
        public void PlayShake()
        {
            DOTween.Kill(camera.gameObject);

            Reset();

            var duration = 0.15f;

            var seq = DOTween.Sequence();
            seq.Join(camera.transform.DOShakePosition(duration, 0.015f));

            var punch = DOTween.Sequence();
            punch.Append(DOTween.To(() => camera.fieldOfView, x => camera.fieldOfView = x, -0.2f, duration * 0.5f).SetRelative(true));
            punch.Append(DOTween.To(() => camera.fieldOfView, x => camera.fieldOfView = x, -0.2f, duration * 0.5f).SetRelative(true));
            seq.Join(punch);

            seq.AppendCallback(() => Reset());
            seq.SetTarget(camera.gameObject);
            seq.SetLink(camera.gameObject);
            seq.SetAutoKill(true);
            seq.Play();
        }

        /// <summary>
        /// 重置摄像机
        /// </summary>
        public void Reset()
        {
            DOTween.Kill(camera.gameObject);
            camera.fieldOfView = _desinnFov;
            camera.transform.eulerAngles = _designEuler;
        }

        protected override GameObject CreateGameObject()
        {
            var go = GameObject.FindGameObjectWithTag("MainCamera");
            if (go == null)
            {
                go = ObjectHelper.Instantiate("MJCommon/MJ/mj_env", "prefabs/main_camera.prefab");
            }
            return go;
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/8/30  15:04

using Unity.Core;
using UnityEngine;
using WLCore.Entity;
using Unity.Widget;
using UnityEngine.EventSystems;

namespace Common
{
    public class Touch3DLayer : WLayer
    {
        public TouchInput3D touchInput3D;

        public Touch3DLayer()
        {
            Init();

            // 开启点击事件
            EnableTouchEvent();

            touchInput3D = new TouchInput3D();
        }

        protected override bool OnTouchBegan(PointerEventData d)
        {
            touchInput3D.DispatchTouchEvent(TouchPhase.Began, d);
            return true;
        }

        protected override void OnTouchMove(PointerEventData d)
        {
            touchInput3D.DispatchTouchEvent(TouchPhase.Moved, d);
        }

        protected override void OnTouchEnd(PointerEventData d)
        {
            touchInput3D.DispatchTouchEvent(TouchPhase.Ended, d);
        }
    }

    public class TouchEvent3DCase : BaseCaseStage
    {
        private Camera _camera;
        private CubeEntity _cube;
        private TouchInput3D _touchInput3D;

        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();

            _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            _cube = new CubeEntity();

            // 创建点击事件层
            var layer = new Touch3DLayer();
            layer.AddTo(root, 200); // 不能设置int.MinValue会识别不到
            
            _touchInput3D = layer.touchInput3D;
        }

        public void CaseUnregister()
        {
            _touchInput3D.RemoveListener(_cube.gameObject);

            WLDebug.Log("CaseUnregister");
        }

        public void CaseBeginEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.Begin, data =>
            {
                WLDebug.Log("Begin", data.hitIndex);
            });
        }

        public void CaseMoveEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.Move, data =>
            {
                WLDebug.Log("Move", data.hitIndex);
            });
        }

        public void CaseEndEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.End, data =>
            {
                WLDebug.Log("End", data.hitIndex);
            });
        }

        public void CaseEnterEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.Enter, data =>
            {
                WLDebug.Log("Enter", data.hitIndex);
            });
        }

        public void CaseExitEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.Exit, data =>
            {
                WLDebug.Log("Exit", data.hitIndex);
            });
        }

        public void CaseClickEvent()
        {
            _touchInput3D.AddListener(_cube.gameObject, _camera, TouchEvent3D.Click, data =>
            {
                WLDebug.Log("Click", data.hitIndex);
            });
        }

        public class CubeEntity : BaseEntity
        {
            protected override GameObject CreateGameObject()
            {
                return GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
        }
    }
}

// @Author: tanjinhua
// @Date: 2021/10/9  15:43

using Unity.Widget;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace MJCommon
{
    public class MJTouch3DLayer : WLayer
    {
        public TouchInput3D touchInput3D;

        public MJTouch3DLayer()
        {
            var obj = CanvasHelper.CreateEmptyCanvas();
            obj.AddComponent<GraphicRaycaster>();
            obj.name = "input_layer";
            InitGameObject(obj);

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
}

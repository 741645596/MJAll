
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonClickImage : Image
{
    private PolygonCollider2D _polygon = null;

    protected override void Awake()
    {
        base.Awake();
        _polygon = GetComponent<PolygonCollider2D>();
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        var point = eventCamera.ScreenToWorldPoint(screenPoint);
        return _polygon.OverlapPoint(point);
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(PolygonClickImage))]
//public class PolygonImageInspector : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        if (GUILayout.Button("重置多边形区域"))
//        {
//            PolygonClickImage ctr = target as PolygonClickImage;
//            var polygon2D = ctr.GetComponent<PolygonCollider2D>();
//            var size = ctr.rectTransform.sizeDelta;
//            float w = size.x / 2;
//            float h = size.y / 2;
//            polygon2D.points = new Vector2[]
//            {
//                new Vector2(-w, -h),
//                new Vector2(w, -h),
//                new Vector2(w, h),
//                new Vector2(-w, h)
//            };
//        }
//    }
//}
//#endif

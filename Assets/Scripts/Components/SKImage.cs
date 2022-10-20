// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEngine;
using UnityEngine.UI;


[AddComponentMenu("UI/Skew Image")]
public class SKImage : Image
{
    [SerializeField]
    private Vector2 _skew = Vector2.zero;
    public Vector2 skew
    {
        get
        {
            return _skew;
        }
        set
        {
            if (_skew != value)
            {
                _skew = value;
                UpdateGeometry();
            }
        }
    }


    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        base.OnPopulateMesh(toFill);

#if !UNITY_EDITOR
        if (_skew == Vector2.zero)
        {
            return;
        }
#endif

        // 调试：移动、旋转、缩放不会调用该方法，但是修改颜色会
        GraphicSkewHelper.PopulateMeshWithSkew(toFill, _skew);
    }
}
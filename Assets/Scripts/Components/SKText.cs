// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[AddComponentMenu("UI/Skew Text")]
public class SKText : Text
{
    public event Action onUpdateSize;


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

    [SerializeField]
    private bool _adaptedSize = false;
    public bool adaptedSize
    {
        get
        {
            return _adaptedSize;
        }
        set
        {
            _adaptedSize = value;
            if (_adaptedSize)
            {
                UpdateSize();
            }
        }
    }

    [SerializeField]
    private Vector2 _dimensions = Vector2.zero;
    public Vector2 dimensions
    {
        get
        {
            return _dimensions;
        }
        set
        {
            if (_dimensions != value)
            {
                _dimensions = value;
                UpdateSize();
            }
        }
    }


    internal float fntHeight = 0;


    protected override void Start()
    {
        base.Start();

        RegisterDirtyVerticesCallback(UpdateSize);

        UpdateSize();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();

        UpdateSize();
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
        GraphicSkewHelper.PopulateMeshWithSkew(toFill, _skew);
    }

    private void UpdateSize()
    {
        if (rectTransform == null)
        {
            return;
        }
        if (!adaptedSize)
        {
            return;
        }
        RectTransform ptrs = rectTransform.parent as RectTransform;
        if (ptrs == null)
        {
            return;
        }
        float w = dimensions.x <= Mathf.Epsilon ? preferredWidth : dimensions.x;
        float h = dimensions.y <= Mathf.Epsilon ? preferredHeight : dimensions.y;
        if (h <= Mathf.Epsilon && fntHeight <= Mathf.Epsilon && font != null)
        {
            CharacterInfo[] infos = font.characterInfo;
            for (int i = 0; i < infos.Length; i++)
            {
                CharacterInfo info = infos[i];
                float height = Mathf.Abs(info.glyphHeight);
                fntHeight = fntHeight < height ? height : fntHeight;
            }
        }
        else if (h > 0)
        {
            fntHeight = 0;
        }
        h = h == 0 ? fntHeight : h;
        Vector2 anchoredSize = (rectTransform.anchorMax - rectTransform.anchorMin) * ptrs.rect.size;
        rectTransform.sizeDelta = new Vector2(w, h) - anchoredSize;

        onUpdateSize?.Invoke();
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        base.OnValidate();

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(IEDelayApply());
    }

    private IEnumerator IEDelayApply()
    {
        yield return null;

        UpdateSize();
    }
#endif
}
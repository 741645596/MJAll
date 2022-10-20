// @Author: tanjinhua
// @Date: 2021/1/19  20:30


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("UI/Progress Indicator")]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SKImage))]
public class ProgressIndicator : MonoBehaviour
{
    public enum IndicateMode
    {
        /// <summary>
        /// 水平九宫格拉伸
        /// </summary>
        SlicedHorizontal,
        /// <summary>
        /// 垂直九宫格拉伸
        /// </summary>
        SlicedVertical,
        /// <summary>
        /// 水平填充
        /// </summary>
        FilledHorizontal,
        /// <summary>
        /// 垂直填充
        /// </summary>
        FilledVertical,
        /// <summary>
        /// 圆形填充
        /// </summary>
        FilledRadial
    }

    public enum HorizontalOrigin
    {
        Left,
        Right
    }

    public enum VerticalOrigin
    {
        Bottom,
        Top
    }

    public enum RadialOrigin
    {
        Bottom,
        Right,
        Top,
        Left
    }

    [SerializeField]
    private IndicateMode _indicateMode = IndicateMode.SlicedHorizontal;
    public IndicateMode indicateMode
    {
        get
        {
            return _indicateMode;
        }
        set
        {
            _indicateMode = value;
            SetupIndicateMode();
        }
    }

    [SerializeField]
    private HorizontalOrigin _horizontalOrigin = HorizontalOrigin.Left;
    public HorizontalOrigin horizontalOrigin
    {
        get
        {
            return _horizontalOrigin;
        }
        set
        {
            _horizontalOrigin = value;
            SetupIndicateMode();
        }
    }

    [SerializeField]
    private VerticalOrigin _verticalOrigin = VerticalOrigin.Bottom;
    public VerticalOrigin verticalOrigin
    {
        get
        {
            return _verticalOrigin;
        }
        set
        {
            _verticalOrigin = value;
            SetupIndicateMode();
        }
    }

    [SerializeField]
    private RadialOrigin _radialOrigin = RadialOrigin.Top;
    public RadialOrigin radialOrigin
    {
        get
        {
            return _radialOrigin;
        }
        set
        {
            _radialOrigin = value;
            SetupIndicateMode();
        }
    }

    [SerializeField]
    private bool _clockwise = true;
    public bool clockwise
    {
        get
        {
            return _clockwise;
        }
        set
        {
            _clockwise = value;
            SetupIndicateMode();
        }
    }

    [SerializeField]
    private SKImage _progressGraphic;
    public SKImage progressGraphic
    {
        get
        {
            return _progressGraphic;
        }
        set
        {
            _progressGraphic = value;
            Setup();
        }
    }


    [SerializeField]
    private float _progress = 1f;
    public float progress
    {
        get
        {
            return _progress;
        }
        set
        {
            _progress = value;
            SetupProgress();
        }
    }

    [SerializeField]
    private bool _useMask = false;
    public bool useMask
    {
        get
        {
            return _useMask;
        }
        set
        {
            _useMask = value;
            SetupMask();
        }
    }

    [SerializeField]
    private Mask _mask;
    public Mask mask
    {
        get
        {
            return _mask;
        }
        set
        {
            _mask = value;
            SetupMask();
        }
    }

    [SerializeField]
    private CanvasRenderer _maskRender;

    [SerializeField]
    private SKImage _maskGraphic;

    /// <summary>
    /// 矫正层级
    /// </summary>
    public void CorrectSibling()
    {
        _mask.rectTransform.SetAsFirstSibling();
    }

    private void Setup()
    {
        SetupIndicateMode();

        SetupMask();

        SetupProgress();

        CorrectSibling();
    }

    private void SetupIndicateMode()
    {
        switch (_indicateMode)
        {
            case IndicateMode.SlicedHorizontal:
                progressGraphic.type = Image.Type.Sliced;
                float px = (float)_horizontalOrigin;
                progressGraphic.rectTransform.pivot = new Vector2(px, 0.5f);
                break;
            case IndicateMode.SlicedVertical:
                progressGraphic.type = Image.Type.Sliced;
                float py = (float)_verticalOrigin;
                progressGraphic.rectTransform.pivot = new Vector2(0.5f, py);
                break;
            case IndicateMode.FilledHorizontal:
                ResetProgressRect();
                progressGraphic.type = Image.Type.Filled;
                progressGraphic.fillMethod = Image.FillMethod.Horizontal;
                progressGraphic.fillOrigin = (int)_horizontalOrigin;
                break;
            case IndicateMode.FilledVertical:
                ResetProgressRect();
                progressGraphic.type = Image.Type.Filled;
                progressGraphic.fillMethod = Image.FillMethod.Vertical;
                progressGraphic.fillOrigin = (int)_verticalOrigin;
                break;
            case IndicateMode.FilledRadial:
                ResetProgressRect();
                progressGraphic.type = Image.Type.Filled;
                progressGraphic.fillMethod = Image.FillMethod.Radial360;
                progressGraphic.fillOrigin = (int)_radialOrigin;
                progressGraphic.fillClockwise = _clockwise;
                break;
        }

        SetupProgress();
    }

    private void ResetProgressRect()
    {
        progressGraphic.rectTransform.pivot = Vector2.one / 2f;
        progressGraphic.rectTransform.offsetMin = Vector2.zero;
        progressGraphic.rectTransform.offsetMax = Vector2.zero;
    }

    private void SetupMask()
    {
        _mask.enabled = _useMask;
        _maskGraphic.enabled = _useMask;
        HideFlags flags = _useMask ? HideFlags.None : HideFlags.HideInInspector;
        _mask.hideFlags = flags;
        _maskRender.hideFlags = flags;
        _maskGraphic.hideFlags = flags;
    }

    private void SetupProgress()
    {
        Vector2 size = (transform as RectTransform).rect.size;
        switch (indicateMode)
        {
            case IndicateMode.FilledHorizontal:
            case IndicateMode.FilledVertical:
            case IndicateMode.FilledRadial:
                _progressGraphic.fillAmount = _progress;
                break;
            case IndicateMode.SlicedHorizontal:
                Vector2 sizeHorizontal = new Vector2(size.x * _progress, size.y);
                SetProgressSize(sizeHorizontal);
                break;
            case IndicateMode.SlicedVertical:
                Vector2 sizeVertical = new Vector2(size.x, size.y * _progress);
                SetProgressSize(sizeVertical);
                break;
        }
    }

    private void SetProgressSize(Vector2 size)
    {
        RectTransform rectTransform = _progressGraphic.rectTransform;
        Vector2 anchoredSize = (rectTransform.anchorMax - rectTransform.anchorMin) * (transform as RectTransform).rect.size;
        rectTransform.sizeDelta = size - anchoredSize;
    }

    public static GameObject CreateTemplate()
    {
        GameObject obj = new GameObject("Progress Indicator", typeof(ProgressIndicator));
        RectTransform trs = obj.transform as RectTransform;
        trs.sizeDelta = new Vector2(160, 15);

        SKImage background = obj.GetComponent<SKImage>();
        background.sprite = Resources.Load<Sprite>("DefaultSprite"); 
        background.type = Image.Type.Sliced;
        background.color = new Color(0.6f, 0.6f, 0.6f, 1f);

        // set up fill area
        GameObject maskObj = new GameObject("FillArea", typeof(Mask));
        RectTransform mtrs = maskObj.transform as RectTransform;
        mtrs.SetParent(trs);
        mtrs.anchorMin = Vector2.zero;
        mtrs.anchorMax = Vector2.one;
        mtrs.offsetMin = Vector2.zero;
        mtrs.offsetMax = Vector2.zero;
        Mask mask = maskObj.GetComponent<Mask>();
        mask.showMaskGraphic = false;
        mask.enabled = false;
            
        SKImage maskGraphic = maskObj.AddComponent<SKImage>();
        maskGraphic.type = Image.Type.Sliced;
        maskGraphic.raycastTarget = false;
        maskGraphic.enabled = false;

        // set up fill
        GameObject progressObj = new GameObject("Progress", typeof(SKImage));
        RectTransform ptrs = progressObj.transform as RectTransform;
        ptrs.SetParent(mtrs);
        ptrs.anchorMin = Vector2.zero;
        ptrs.anchorMax = Vector2.one;
        ptrs.offsetMin = Vector2.zero;
        ptrs.offsetMax = Vector2.zero;
        SKImage progressImage = progressObj.GetComponent<SKImage>();
        progressImage.raycastTarget = false;
        progressImage.sprite = Resources.Load<Sprite>("DefaultSprite");
        progressImage.type = Image.Type.Sliced;

        ProgressIndicator indicator = obj.GetComponent<ProgressIndicator>();
        indicator._mask = mask;
        indicator._maskGraphic = maskGraphic;
        indicator._maskRender = maskObj.GetComponent<CanvasRenderer>();
        indicator._progressGraphic = progressImage;

        return obj;
    }

#if UNITY_EDITOR
    [ContextMenu("Reset")]
    private void Reset()
    {
        _indicateMode = IndicateMode.SlicedHorizontal;
        _horizontalOrigin = HorizontalOrigin.Left;
        _verticalOrigin = VerticalOrigin.Bottom;
        _radialOrigin = RadialOrigin.Top;
        _clockwise = true;
        _progress = 1f;
        _useMask = false;

        OnValidate();
    }

    private void OnValidate()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(IEDelayApply());
    }

    private IEnumerator IEDelayApply()
    {
        yield return null;

        Setup();
    }
#endif
}

// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[AddComponentMenu("UI/Skew Raw Image")]
public class SKRawImage : RawImage
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


    [SerializeField]
    private RTSystem _rtSystem;
    public RTSystem rtSystem
    {
        get => _rtSystem;
        set
        {
            if (_rtSystem == value)
            {
                return;
            }

            if (_rtSystem != null)
            {
                _rtSystem.onTextureUpdate -= OnRTSystemTextureUpdate;
            }

            _rtSystem = value;

            SetupRTSystem();
        }
    }



    protected override void Awake()
    {
        base.Awake();

        SetupRTSystem();
    }


    private void SetupRTSystem()
    {
        if (_rtSystem == null)
        {
            return;
        }

        _rtSystem.onTextureUpdate -= OnRTSystemTextureUpdate;
        _rtSystem.onTextureUpdate += OnRTSystemTextureUpdate;

        OnRTSystemTextureUpdate();
    }


    private void OnRTSystemTextureUpdate()
    {
        texture = _rtSystem.targetTexture;

        SetNativeSize();
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

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();


        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(IEDelayApply());
        }
    }

    private IEnumerator IEDelayApply()
    {
        yield return null;

        SetupRTSystem();
    }
#endif
}
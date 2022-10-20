// @Author: tanjinhua
// @Date: 2021/1/18  22:42


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("UI/Page Indexer")]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(GridLayoutGroup))]
public class PageIndexer : MonoBehaviour
{
    public enum SelectMode
    {
        ColorTint,
        SpriteSwap
    }

    [SerializeField]
    private PageRect.Direction _layoutAxis = PageRect.Direction.Horizontal;
    public PageRect.Direction layoutAxis
    {
        get
        {
            return _layoutAxis;
        }
        set
        {
            _layoutAxis = value;
            SetupLayoutGroup();
        }
    }

    [SerializeField]
    private SelectMode _selectMode = SelectMode.ColorTint;
    public SelectMode selectMode
    {
        get
        {
            return _selectMode;
        }
        set
        {
            _selectMode = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private Color _selectedColor = Color.green;
    public Color selectedColor
    {
        get
        {
            return _selectedColor;
        }
        set
        {
            _selectedColor = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private Sprite _normalSprite;
    public Sprite normalSprite
    {
        get
        {
            if (_normalSprite == null)
            {
                _normalSprite = Resources.Load<Sprite>("DefaultSprite");
            }
            return _normalSprite;
        }
        set
        {
            _normalSprite = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private Sprite _selectedSprite;
    public Sprite selectSprite
    {
        get
        {
            return _selectedSprite;
        }
        set
        {
            _selectedSprite = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private Vector2 _indexerSize = new Vector2(8, 8);
    public Vector2 indexerSize
    {
        get
        {
            return _indexerSize;
        }
        set
        {
            _indexerSize = value;
            SetupLayoutGroup();
        }
    }

    [SerializeField]
    private Vector2 _spacing = new Vector2(5, 5);
    public Vector2 spacing
    {
        get
        {
            return _spacing;
        }
        set
        {
            _spacing = value;
            SetupLayoutGroup();
        }
    }

    [SerializeField]
    private int _selectedIndex = 0;
    public int selectedIndex
    {
        get
        {
            return _selectedIndex;
        }
        set
        {
            _selectedIndex = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private float _selectedScale = 1.1f;
    public float selectedScale
    {
        get
        {
            return _selectedScale;
        }
        set
        {
            _selectedScale = value;
            SetupSelected();
        }
    }

    [SerializeField]
    private int _count = 0;
    public int count
    {
        get
        {
            return _count;
        }
        set
        {
            if (_count != value)
            {
                _count = value;
                Setup();
            }
        }
    }

    [SerializeField]
    private GraphicSettings _graphicSettings;
    public GraphicSettings graphicSettings
    {
        get
        {
            if (_graphicSettings == null)
            {
                _graphicSettings = new GraphicSettings();
            }
            return _graphicSettings;
        }
    }

    [SerializeField]
    private GridLayoutGroup _layoutGroup;
    public GridLayoutGroup layoutGroup
    {
        get
        {
            if (_layoutGroup == null)
            {
                _layoutGroup = GetComponent<GridLayoutGroup>();
            }
            return _layoutGroup;
        }
    }

    [SerializeField]
    private List<SKImage> _poolList = new List<SKImage>();

    [SerializeField]
    private List<SKImage> _inuseList = new List<SKImage>();

    [SerializeField]
    private Material _customMaterial;

    internal bool masked { get; set; }


    /// <summary>
    /// 是否是索引图片
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool IsIndexerImage(GameObject obj)
    {
        if (obj.name != "@Page_Indexer_Image_Unique_Name")
        {
            return false;
        }

        if (!obj.TryGetComponent(out SKImage image))
        {
            return false;
        }

        if (!_poolList.Contains(image))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 矫正层级
    /// </summary>
    public void CorrectSibling()
    {
        for (int i = _inuseList.Count - 1; i >= 0; i--)
        {
            SKImage image = _inuseList[i];
            if (image != null)
            {
                image.rectTransform.SetAsFirstSibling();
            }
        }
    }

    private void Awake()
    {
        graphicSettings.onDirty += OnGraphicSettingDirty;

        Setup();

        ApplyGraphicSettings();
    }

    private void Setup()
    {
        SetupLayoutGroup();

        SetupPool();

        SetupInuse();

        SetupSelected();

        CorrectSibling();
    }

    private void SetupPool()
    {
        for (int i = _poolList.Count - 1; i >= 0; i--)
        {
            SKImage image = _poolList[i];
            if (image == null)
            {
                _poolList.RemoveAt(i);
            }
            else
            {
                image.gameObject.SetActive(false);
            }
        }
    }

    private void SetupInuse()
    {
        _inuseList.Clear();

        _count = Mathf.Max(0, _count);
        if (_count == 0)
        {
            return;
        }

        for (int i = 0; i < _count; i++)
        {
            SKImage inuse = GetFromPool(i);
            inuse.gameObject.SetActive(true);
            _inuseList.Add(inuse);
        }
    }

    private SKImage GetFromPool(int index)
    {
        if (index < _poolList.Count)
        {
            return _poolList[index];
        }

        return CreateImage();
    }

    private SKImage CreateImage()
    {
        string name = "@Page_Indexer_Image_Unique_Name";
        GameObject obj = new GameObject(name, typeof(SKImage));
        obj.SetActive(false);
        obj.hideFlags = HideFlags.HideInHierarchy;

        SKImage image = obj.GetComponent<SKImage>();
        image.raycastTarget = false;
        image.rectTransform.SetParent(transform);
        ApplyGraphicSettings(image);
        _poolList.Add(image);

        return image;
    }

    private void SetupLayoutGroup()
    {
        layoutGroup.hideFlags = HideFlags.NotEditable;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;
        layoutGroup.constraintCount = 1;
        layoutGroup.cellSize = _indexerSize;
        layoutGroup.spacing = _spacing;
        layoutGroup.constraint = _layoutAxis == PageRect.Direction.Horizontal ?
            GridLayoutGroup.Constraint.FixedRowCount :
            GridLayoutGroup.Constraint.FixedColumnCount;
    }

    private void SetupSelected()
    {
        _selectedIndex = Mathf.Max(0, Mathf.Clamp(_selectedIndex, 0, _count - 1));

        if (_inuseList.Count == 0)
        {
            return;
        }

        _inuseList.ForEach(inuse => {
            inuse.sprite = normalSprite;
            inuse.color = graphicSettings.color;
            inuse.rectTransform.localScale = Vector3.one;
        });

        SKImage selected = _inuseList[_selectedIndex];

        if (_selectMode == SelectMode.ColorTint)
        {
            selected.color = _selectedColor;
        }
        else
        {
            selected.sprite = _selectedSprite;
        }

        selected.rectTransform.localScale *= _selectedScale;
    }

    private void OnGraphicSettingDirty(GraphicSettings.DirtyFlag flag)
    {
        ApplyGraphicSettings();
    }

    internal void ApplyGraphicSettings(SKImage image = null)
    {
        if (image == null)
        {
            UpdateMaterial();

            _poolList.ForEach(pooled => ApplyGraphicSettings(pooled));
        }
        else
        {
            image.ApplyGraphicSettings(graphicSettings, _customMaterial, false);
        }
    }

    private void UpdateMaterial()
    {
        UpdateMaterial(graphicSettings);
    }

    private void UpdateMaterial(GraphicSettings settings)
    {
        if (settings.useDefaultMaterial)
        {
            _customMaterial = null;

            return;
        }

        // UI/Custom shader已废弃，请勿使用
        //if (_customMaterial == null || masked)
        //{
        //    _customMaterial = new Material(Shader.Find("UI/Custom"));
        //}

        //_customMaterial.SetUICustomShaderParams(settings);
    }

    public static GameObject CreateTemplate()
    {
        GameObject obj = new GameObject("Page Indexer", typeof(PageIndexer));
        PageIndexer component = obj.GetComponent<PageIndexer>();
        component.normalSprite = Resources.Load<Sprite>("DefaultSprite");
        return obj;
    }

#if UNITY_EDITOR
    [ContextMenu("Reset")]
    private void Reset()
    {
        _layoutAxis = PageRect.Direction.Horizontal;
        _selectMode = SelectMode.ColorTint;
        _selectedColor = Color.green;
        _normalSprite = null;
        _selectedSprite = null;
        _indexerSize = new Vector2(8, 8);
        _spacing = new Vector2(5, 5);
        _selectedIndex = 0;
        _selectedScale = 1.1f;
        _count = 0;
        _customMaterial = null;
        _graphicSettings?.Reset();
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

        ApplyGraphicSettings();

        Setup();

    }
#endif
}

// @Author: tanjinhua
// @Date: 2021/1/12  14:59


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("UI/Art Text")]
[RequireComponent(typeof(RectTransform))]
public class ArtText : MonoBehaviour
{

    public event Action onUpdateSize;


    [Serializable]
    public struct FontSetting
    {
        public string character;
        public Sprite sprite;
    }

    public enum Alignment
    {
        Top,
        Center,
        Bottom
    }

    [SerializeField]
    private string _text = string.Empty;
    public string text
    {
        get
        {
            return _text;
        }
        set
        {
            if (_text != value)
            {
                _text = value;
                Setup();
            }
        }
    }

    [SerializeField]
    private RectOffset _padding = new RectOffset();
    public RectOffset padding
    {
        get => _padding;
        set
        {
            if (_padding == value)
            {
                return;
            }
            _padding = value;
            SetupLayout();
        }
    }

    [SerializeField]
    private float _spacing = 0f;
    public float spaceing
    {
        get
        {
            return _spacing;
        }
        set
        {
            if (_spacing != value)
            {
                _spacing = value;
                SetupLayout();
            }
        }
    }

    [SerializeField]
    private Alignment _alignment = Alignment.Center;
    public Alignment alignment
    {
        get
        {
            return _alignment;
        }
        set
        {
            if (_alignment != value)
            {
                _alignment = value;
                SetupLayout();
            }
        }
    }

    [SerializeField]
    private bool _uniformWidth = false;
    public bool uniformWidth
    {
        get
        {
            return _uniformWidth;
        }
        set
        {
            if (_uniformWidth != value)
            {
                _uniformWidth = value;
                SetupLayout();
            }
        }
    }

    [SerializeField]
    private bool _adaptedSize = true;
    public bool adaptedSize
    {
        get => _adaptedSize;
        set => _adaptedSize = value;
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
    private FontSetting[] _fontSettings;
    public FontSetting[] fontSettings
    {
        get
        {
            return _fontSettings;
        }
        set
        {
            _fontSettings = value;
            Setup();
        }
    }

    [SerializeField]
    private List<SKImage> _poolList = new List<SKImage>();
    [SerializeField]
    private List<SKImage> _inuseList = new List<SKImage>();
    [SerializeField]
    private Material _customMaterial;
    internal bool masked { get; set; }
    private float _characterMaxWidth;
    private float _characterMaxHeight;


    // perspective simulating
    [SerializeField]
    private bool _simulatePerspectiveEffect = false;
    public bool simulatePerspectiveEffect
    {
        get => _simulatePerspectiveEffect;
        set
        {
            _simulatePerspectiveEffect = value;
            Setup();
        }
    }

    [SerializeField]
    private Vector2 _perspectiveOrigin = Vector2.one;
    public Vector2 perspectiveOrigin
    {
        get => _perspectiveOrigin;
        set
        {
            _perspectiveOrigin = value;
            Setup();
        }
    }

    [SerializeField]
    private Vector2 _perspectiveIncreasement = Vector2.zero;
    public Vector2 perspectiveIncreasement
    {
        get => _perspectiveIncreasement;
        set
        {
            _perspectiveIncreasement = value;
            Setup();
        }
    }

    /// <summary>
    /// 是否是字体对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool IsCharacterImage(GameObject obj)
    {
        if (obj.name != "@Art_Text_Character_Image_Unique_Name")
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
        _inuseList.ForEach(image =>
        {
            if (image != null)
            {
                image.rectTransform.SetAsFirstSibling();
            }
        });
    }

    private void Awake()
    {
        graphicSettings.onDirty += OnGraphicSettingDirty;

        Setup();

        ApplyGraphicSettings();
    }

    private void Setup()
    {
        SetupPool();

        SetupInuse();

        SetupLayout();

        CorrectSibling();
    }

    private void SetupPool()
    {
        for (int i = _poolList.Count - 1; i >= 0; i--)
        {
            SKImage image = _poolList[i];
            if (image == null || image.gameObject == null)
            {
                _poolList.RemoveAt(i);
            }
            else
            {
                image.rectTransform.anchorMax = Vector2.zero;
                image.rectTransform.anchorMin = Vector2.zero;
                image.rectTransform.pivot = Vector2.one / 2f;
                image.rectTransform.anchoredPosition = Vector2.zero;
                image.rectTransform.sizeDelta = Vector2.zero;
                image.rectTransform.localScale = Vector3.one;
                image.sprite = null;
                image.gameObject.SetActive(false);
                image.gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
        }
    }

    private void SetupInuse()
    {
        _characterMaxWidth = 0;
        _characterMaxHeight = 0;

        _inuseList.Clear();

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        int index = 0;
        for (int i = 0; i < text.Length; i++)
        {
            string c = text[i].ToString();
            Sprite s = GetSprite(c);
            if (s == null)
            {
                Debug.LogWarning($"ArtText: Can not find sprite for character '{c}'");
                continue;
            }

            SKImage image;
            if (index < _poolList.Count && !_poolList[index].gameObject.activeSelf)
            {
                image = _poolList[index];
            }
            else
            {
                image = CreateImage();
            }
            image.gameObject.SetActive(true);
            image.sprite = s;
            image.SetNativeSize();
            _inuseList.Add(image);

            Vector2 size = image.rectTransform.rect.size;
            _characterMaxWidth = size.x > _characterMaxWidth ? size.x : _characterMaxWidth;
            _characterMaxHeight = size.y > _characterMaxHeight ? size.y : _characterMaxHeight;
            index++;
        }
    }

    private void SetupLayout()
    {
        if (_inuseList.Count == 0)
        {
            return;
        }

        float px = _uniformWidth ? 0.5f : 0;

        float py = 0;
        switch (_alignment)
        {
            case Alignment.Top:
                py = 1;
                break;
            case Alignment.Center:
                py = 0.5f;
                break;
            case Alignment.Bottom:
                py = 0;
                break;
        }

        float y = _padding.bottom + _characterMaxHeight * py;
        float x = _padding.left + (_uniformWidth ? _characterMaxWidth / 2f : 0);


        float origin = _inuseList.Count * (transform as RectTransform).pivot.x;

        for (int i = 0; i < _inuseList.Count; i++)
        {
            SKImage image = _inuseList[i];
            image.rectTransform.pivot = new Vector2(px, py);
            image.rectTransform.anchoredPosition = new Vector2(x, y);

            float deltaX = _uniformWidth ? _characterMaxWidth : image.rectTransform.rect.size.x;
            float space = i < _inuseList.Count - 1 ? _spacing : 0;

            if (_simulatePerspectiveEffect)
            {
                float scaleX = Mathf.Max(0, (i - origin) * _perspectiveIncreasement.x + _perspectiveOrigin.x);
                float scaleY = Mathf.Max(0, (i - origin) * _perspectiveIncreasement.y + _perspectiveOrigin.y);
                deltaX *= scaleX;
                space *= scaleX;

                image.rectTransform.localScale = new Vector3(scaleX, scaleY, 1);
            }

            x += deltaX + space;

            if (_uniformWidth && i == _inuseList.Count - 1)
            {
                x -= _characterMaxWidth / 2;
            }
        }

        UpdateSize(new Vector2(x + _padding.right, _characterMaxHeight + _padding.top + _padding.bottom));
    }

    private void UpdateSize(Vector2 size)
    {
        if (!adaptedSize)
        {
            return;
        }

        RectTransform trs = transform as RectTransform;

        if (trs.parent is RectTransform p)
        {
            Vector2 anchoredSize = (trs.anchorMax - trs.anchorMin) * p.rect.size;
            trs.sizeDelta = size - anchoredSize;
        }
        else
        {
            trs.sizeDelta = size;
        }

        onUpdateSize?.Invoke();
    }

    private SKImage CreateImage()
    {
        string name = "@Art_Text_Character_Image_Unique_Name";
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.SetActive(false);
        obj.hideFlags = HideFlags.HideInHierarchy;

        RectTransform trs = obj.transform as RectTransform;
        trs.SetParent(transform, false);
        trs.anchorMax = Vector2.zero;
        trs.anchorMin = Vector2.zero;
        trs.pivot = Vector2.one / 2f;
        trs.anchoredPosition = Vector2.zero;
        trs.sizeDelta = Vector2.zero;

        SKImage image = obj.AddComponent<SKImage>();
        image.raycastTarget = false;
        ApplyGraphicSettings(image);
        _poolList.Add(image);

        return image;
    }

    private Sprite GetSprite(string c)
    {
        if (_fontSettings == null)
        {
            return null;
        }

        for (int i = 0; i < _fontSettings.Length; i++)
        {
            FontSetting setting = _fontSettings[i];
            if (setting.character == c)
            {
                return setting.sprite;
            }
        }

        return null;
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

#if UNITY_EDITOR
    [ContextMenu("Reset")]
    private void Reset()
    {
        _text = string.Empty;
        _spacing = 0f;
        _alignment = Alignment.Center;
        _uniformWidth = false;
        _fontSettings = null;
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

        Setup();

        ApplyGraphicSettings();
    }
#endif
}

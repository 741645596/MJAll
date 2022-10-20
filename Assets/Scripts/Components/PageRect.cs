// @Author: tanjinhua
// @Date: 2021/1/17  9:36


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[AddComponentMenu("UI/Page Rect")]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class PageRect : MonoBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public enum Direction
    {
        Horizontal,
        Vertical
    }

    [SerializeField]
    private ScrollRect _scrollRect;
    public ScrollRect scrollRect
    {
        get
        {
            return _scrollRect;
        }
    }
    public RectTransform content
    {
        get
        {
            return _scrollRect?.content;
        }
    }

    [SerializeField]
    private Direction _direction = Direction.Horizontal;
    public Direction direction
    {
        get
        {
            return _direction;
        }
        set
        {
            _direction = value;
            SetupDirection();
        }
    }

    [SerializeField]
    private int _currentIndex = 0;
    public int currentIndex
    {
        get
        {
            return _currentIndex;
        }
        set
        {
            _currentIndex = value;
            SetupCurrentIndex();
        }
    }

    [SerializeField]
    private bool _frozen = false;
    public bool frozen
    {
        get
        {
            return _frozen;
        }
        set
        {
            _frozen = value;
        }
    }

    [SerializeField]
    private bool _masking = true;
    public bool masking
    {
        get
        {
            return _masking;
        }
        set
        {
            _masking = value;
            SetupMasking();
        }
    }

    [SerializeField]
    private Mask _mask;
    private Mask mask
    {
        get
        {
            return _mask;
        }
    }

    public int pageCount
    {
        get
        {
            return _pages.Count;
        }
    }

    [SerializeField]
    private PageIndexer _pageIndexer;
    public PageIndexer pageIndexer
    {
        get
        {
            return _pageIndexer;
        }
    }

    [SerializeField]
    private List<RectTransform> _pages = new List<RectTransform>();
    public List<RectTransform> pages
    {
        get
        {
            return _pages;
        }
    }

    public event Action<int> onTurn;
    internal event Action<RectTransform> onAppend;
    internal event Action<RectTransform> onRemove;

    private bool _draging = false;
    private bool _tweening = false;
    private float _tweenDelay = 0f;

    /// <summary>
    /// 添加一页
    /// </summary>
    /// <param name="page"></param>
    /// <param name="index"></param>
    public void Append(RectTransform page, int index = -1)
    {
        if (_pages.Contains(page))
        {
            return;
        }
        index = index == -1 ? pageCount : index;
        index = Mathf.Clamp(index, 0, _pages.Count);
        _pages.Insert(index, page);
        Setup();
        onAppend?.Invoke(page);
    }

    /// <summary>
    /// 移除一页
    /// </summary>
    /// <param name="index"></param>
    public void Remove(int index)
    {
        if (index < 0 || index > _pages.Count - 1)
        {
            return;
        }

        RectTransform page = _pages[index];
        _pages.RemoveAt(index);
        Setup();
        onRemove?.Invoke(page);
        if (gameObject != null)
        {
            Destroy(page.gameObject);
        }
    }

    /// <summary>
    /// 清除所有
    /// </summary>
    public void Clear()
    {
        for (int i = pageCount - 1; i >= 0; i--)
        {
            Remove(i);
        }
    }

    /// <summary>
    /// 跳到指定页
    /// </summary>
    /// <param name="index"></param>
    /// <param name="time"></param>
    public void TurnTo(int index, float time = 0.5f)
    {
        if (_scrollRect == null || frozen)
        {
            return;
        }
        DOTween.Kill(_scrollRect.gameObject);
        Vector2 pos = GetPosByIndex(index);
        Tween tween = DOTween.To(() => _scrollRect.normalizedPosition, x => _scrollRect.normalizedPosition = x, pos, time);
        tween.SetTarget(_scrollRect.gameObject);
        tween.SetLink(_scrollRect.gameObject);
        tween.SetAutoKill(true);
        tween.Play();
    }

    #region Inherit
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (frozen)
        {
            return;
        }
        _scrollRect?.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _scrollRect?.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _scrollRect?.OnEndDrag(eventData);
        _tweenDelay = Mathf.Max(0f, 0.1f - 1f / _scrollRect.velocity.magnitude);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (frozen)
        {
            return;
        }
        _scrollRect?.OnInitializePotentialDrag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DOTween.Kill(_scrollRect.gameObject);
        _tweening = false;
        _draging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _draging = false;
        _tweenDelay = 0f;
    }
    #endregion

    private void OnValueChanged(Vector2 pos)
    {
        int index = 0;
        float unit = 1f / (_pages.Count - 1);
        switch (direction)
        {
            case Direction.Horizontal:
                index = Mathf.RoundToInt(pos.x / unit);
                break;
            case Direction.Vertical:
                index = Mathf.RoundToInt((1f - pos.y) / unit);
                break;
        }

        index = Mathf.Clamp(index, 0, _pages.Count - 1);
        index = Mathf.Max(0, index);

        if (index != _currentIndex)
        {
            _currentIndex = index;
            onTurn?.Invoke(_currentIndex);
            if (_pageIndexer != null)
            {
                _pageIndexer.selectedIndex = index;
            }
        }
    }

    private void Start()
    {
        _scrollRect.onValueChanged.AddListener(OnValueChanged);
    }

    private void LateUpdate()
    {
        if (_scrollRect != null && !_draging && !_tweening)
        {
            _tweening = true;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(_tweenDelay);
            sequence.AppendCallback(new TweenCallback(() =>
            {
                Vector2 pos = GetPosByIndex(_currentIndex);
                Tween tween = DOTween.To(() => _scrollRect.normalizedPosition, x => _scrollRect.normalizedPosition = x, pos, 0.3f);
                tween.SetEase(Ease.OutExpo);
                sequence.Append(tween);
            }));
            sequence.SetTarget(_scrollRect.gameObject);
            sequence.SetLink(_scrollRect.gameObject);
            sequence.SetAutoKill(true);
            sequence.Play();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        Setup();
    }

    private void Setup()
    {
        SetupDirection();

        SetupContentSize();

        SetupPages();

        SetupCurrentIndex();

        SetupMasking();
    }

    private void SetupDirection()
    {
        if (_scrollRect == null)
        {
            return;
        }

        _scrollRect.horizontal = false;
        _scrollRect.vertical = false;
        switch (_direction)
        {
            case Direction.Horizontal:
                _scrollRect.horizontal = true;
                break;
            case Direction.Vertical:
                _scrollRect.vertical = true;
                break;
        }
    }

    private void SetupContentSize()
    {
        if (_scrollRect == null)
        {
            return;
        }

        Vector2 viewSize = (transform as RectTransform).rect.size;
        float contentSizeX = direction == Direction.Horizontal ? viewSize.x * Mathf.Max(1, _pages.Count) : viewSize.x;
        float contentSizeY = direction == Direction.Vertical ? viewSize.y * Mathf.Max(1, _pages.Count) : viewSize.y;
        _scrollRect.content.sizeDelta = new Vector2(contentSizeX, contentSizeY);
        _scrollRect.content.anchoredPosition = Vector2.zero;
    }

    private void SetupPages()
    {
        if (_scrollRect == null)
        {
            return;
        }

        Vector2 viewSize = (transform as RectTransform).rect.size;
        Vector2 contentSize = (content.transform as RectTransform).rect.size;
        for (int i = 0; i < _pages.Count; i++)
        {
            RectTransform page = _pages[i];
            if (page == null)
            {
                continue;
            }

            page.SetParent(_scrollRect.content, false);
            page.pivot = Vector2.one / 2f;
            float an = i / (float)_pages.Count;
            float ax = direction == Direction.Horizontal ? an + viewSize.x / 2f / contentSize.x : 0.5f;
            float ay = direction == Direction.Vertical ? 1f - an - viewSize.y / 2f / contentSize.y : 0.5f;
            Vector2 anchor = new Vector2(ax, ay);
            page.anchorMin = anchor;
            page.anchorMax = anchor;
            page.anchoredPosition = Vector2.zero;
        }

        if (_pageIndexer != null)
        {
            _pageIndexer.count = _pages.Count;
        }
    }

    private void SetupCurrentIndex()
    {
        if (_scrollRect == null)
        {
            return;
        }

        _currentIndex = Mathf.Clamp(_currentIndex, 0, _pages.Count - 1);
        _scrollRect.normalizedPosition = GetPosByIndex(_currentIndex);
        OnValueChanged(_scrollRect.normalizedPosition);
        if (_pageIndexer != null)
        {
            _pageIndexer.selectedIndex = _currentIndex;
        }
    }

    private void SetupMasking()
    {
        if (mask != null)
        {
            mask.enabled = _masking;
            mask.graphic.enabled = _masking;
        }
    }

    private Vector2 GetPosByIndex(int index)
    {
        float pn = index / (float)(_pages.Count - 1);
        float px = direction == Direction.Horizontal ? pn : 0f;
        float py = direction == Direction.Vertical ? 1f - pn : 0f;
        return new Vector2(px, py);
    }

    public static GameObject CreateTemplate()
    {
        GameObject obj = new GameObject("Page View", typeof(PageRect));
        RectTransform trs = obj.transform as RectTransform;
        trs.sizeDelta = new Vector2(200, 200);
        Image image = obj.GetComponent<Image>();
        image.color = new Color(1, 1, 1, 0.3f);
        image.type = Image.Type.Sliced;

        GameObject scrollObj = new GameObject("ScrollRect", typeof(RectTransform));
        RectTransform strs = scrollObj.transform as RectTransform;
        strs.SetParent(trs);
        strs.anchorMin = Vector2.zero;
        strs.anchorMax = Vector2.one;
        strs.offsetMin = Vector2.zero;
        strs.offsetMax = Vector2.zero;
        ScrollRect scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.vertical = false;
        scrollRect.decelerationRate = 0.01f;

        GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform));
        RectTransform vtrs = viewportObj.transform as RectTransform;
        vtrs.SetParent(strs);
        vtrs.anchorMin = Vector2.zero;
        vtrs.anchorMax = Vector2.one;
        vtrs.offsetMin = Vector2.zero;
        vtrs.offsetMax = Vector2.zero;
        viewportObj.AddComponent<Image>().raycastTarget = false;
        viewportObj.AddComponent<Mask>().showMaskGraphic = false;
        scrollRect.viewport = vtrs;
        Mask mask = viewportObj.GetComponent<Mask>();

        GameObject contentObj = new GameObject("Content", typeof(RectTransform));
        RectTransform ctrs = contentObj.transform as RectTransform;
        ctrs.anchorMin = Vector2.up;
        ctrs.anchorMax = Vector2.up;
        ctrs.pivot = Vector2.up;
        ctrs.sizeDelta = new Vector2(200, 200);
        ctrs.SetParent(vtrs);
        scrollRect.content = ctrs;

        GameObject indexerObj = PageIndexer.CreateTemplate();
        RectTransform indexerTrs = indexerObj.transform as RectTransform;
        indexerTrs.anchorMin = new Vector2(0.5f, 0);
        indexerTrs.anchorMax = new Vector2(0.5f, 0);
        indexerTrs.sizeDelta = new Vector2(200, 8);
        indexerTrs.anchoredPosition = new Vector2(0, -20);
        indexerTrs.SetParent(trs, false);
        PageIndexer indexer = indexerObj.GetComponent<PageIndexer>();

        PageRect pageRect = obj.GetComponent<PageRect>();
        pageRect._scrollRect = scrollRect;
        pageRect._pageIndexer = indexer;
        pageRect._mask = mask;

        return obj;
    }

#if UNITY_EDITOR
    [ContextMenu("Reset")]
    private void Reset()
    {
        direction = Direction.Horizontal;
        currentIndex = 0;
        _frozen = false;
        _masking = true;

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
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 双指缩放控件，比如棋牌地图选择地区功能。
/// 使用方法：
/// 1. 创建Scroll View控件
/// 2. 删除Scroll Rect组件，添加Zoom Scroll Rect组件
/// 3. 新组件需要重新配置Content和Viewport
/// 4. Conetnt节点添加Image组件，配置需要的地图，Anchors都改为0.5，设置Width和Height滚动区域大小
/// </summary>
public class ZoomScrollRect : ScrollRect, IPointerClickHandler
{
    public float minScale = 1f;
    public float maxScale = 2f;
    private int touchNum = 0;

    protected override void Start()
    {
        movementType = MovementType.Clamped;
        horizontalScrollbarSpacing = 0;
        verticalScrollbarSpacing = 0;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
        {
            return;
        }

        base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1)
        {
            touchNum = Input.touchCount;
            return;
        }
        else if (Input.touchCount == 1 && touchNum > 1)
        {
            touchNum = Input.touchCount;
            base.OnBeginDrag(eventData);
            return;
        }

        base.OnDrag(eventData);
    }

    private float lastDistance = -1;
    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 1)
        {
            var t1 = Input.GetTouch(0);
            var t2 = Input.GetTouch(1);

            var p1 = t1.position;
            var p2 = t2.position;

            if ((t1.phase == UnityEngine.TouchPhase.Moved && t2.phase == UnityEngine.TouchPhase.Moved)
                || (t1.phase == UnityEngine.TouchPhase.Stationary && t2.phase == UnityEngine.TouchPhase.Moved)
                || (t1.phase == UnityEngine.TouchPhase.Moved && t2.phase == UnityEngine.TouchPhase.Stationary))
            {
                var rt = base.content;

                Vector2 center;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, (p1 + p2) / 2, CameraUtil.GetUICamera(), out center);
                var pivot = new Vector2((center.x / rt.localScale.x + rt.sizeDelta.x * rt.pivot.x) / rt.sizeDelta.x, (center.y / rt.localScale.y + rt.sizeDelta.y * rt.pivot.y) / rt.sizeDelta.y);

                UpdatePivot(rt, pivot);

                var distance = Vector2.Distance(p1, p2);
                if (lastDistance == -1)
                {
                    lastDistance = distance;
                }
                var change = (distance - lastDistance) / 100f * scrollSensitivity;
                var scale = new Vector3(change, change, 0) + rt.localScale;

                scale.x = Mathf.Clamp(scale.x, minScale, maxScale);
                scale.y = Mathf.Clamp(scale.y, minScale, maxScale);

                rt.localScale = scale;

                lastDistance = distance;
            }
            else if (t1.phase == UnityEngine.TouchPhase.Ended || t1.phase == UnityEngine.TouchPhase.Canceled
                || t2.phase == UnityEngine.TouchPhase.Ended || t2.phase == UnityEngine.TouchPhase.Canceled)
            {
                lastDistance = -1;
            }
        }
    }

    private void UpdatePivot(RectTransform rectTransform, Vector2 pivot)
    {
        var pos = rectTransform.anchoredPosition;
        pos.x -= rectTransform.rect.width * (rectTransform.pivot.x - pivot.x);
        pos.y -= rectTransform.rect.height * (rectTransform.pivot.y - pivot.y);
        rectTransform.pivot = pivot;
        rectTransform.anchoredPosition = pos;
    }

    private float doubleClickLastTime;
    private Vector2 doubleClickLastPosition;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(Input.touchCount > 1)
        {
            return;
        }

        //Debug.Log(eventData.clickTime - doubleClickLastTime);
        //Debug.Log(Vector2.Distance(doubleClickLastPosition, eventData.position));
        if ((eventData.clickTime - doubleClickLastTime < 0.25f)
            &&(Vector2.Distance(doubleClickLastPosition, eventData.position) < 50f))
        {
            var rt = base.content;

            Vector2 p;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, CameraUtil.GetUICamera(), out p);
            var pivot = new Vector2((p.x / rt.localScale.x + rt.sizeDelta.x * rt.pivot.x) / rt.sizeDelta.x, (p.y / rt.localScale.y + rt.sizeDelta.y * rt.pivot.y) / rt.sizeDelta.y);

            UpdatePivot(rt, pivot);

            var scale = rt.localScale.x < ((maxScale + minScale) / 2f) ? maxScale : minScale;
            rt.DOScale(new Vector3(scale, scale, 1), 0.3f);
        }

        doubleClickLastTime = eventData.clickTime;
        doubleClickLastPosition = eventData.position;
    }
}

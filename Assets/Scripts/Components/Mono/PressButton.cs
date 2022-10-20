using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WLCore.Helper;
using static UnityEngine.UI.Button;

/// <summary>
/// 按钮按下去缩小效果，可以添加到空的RectTransform
/// 记得需要有Image并且Raycast Target勾选点击才能生效
/// </summary>
public class PressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 按下去缩放比例
    public float pressScale = 0.95f;
    public ButtonClickedEvent onClick;
    // 限制重复点击时间间隔
    public float clickInterval = 0.5f;  
    

    private Vector3 _originalScale;
    private RectTransform _rectTransform;
    private long _clickLastTime = 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.localScale = new Vector3(pressScale * _originalScale.x, pressScale * _originalScale.y, 1); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.localScale = _originalScale;

        var clicktTime = DateHelper.GetMilliSeconds();
        if (clicktTime - _clickLastTime < clickInterval * 1000)
        {
            return;
        }
        _clickLastTime = clicktTime;
        onClick?.Invoke();
    }

    protected void Start()
    {
        _rectTransform = gameObject.transform as RectTransform;
        if (_rectTransform == null)
        {
            Debug.LogWarning($"错误提示：挂载了PressButton脚本，但是组件不是RectTransform，请检查代码{gameObject.name}");
        }
        else
        {
            _originalScale = _rectTransform.localScale;
        }
    }
}

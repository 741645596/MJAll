using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// 长按按钮控件
/// </summary>
public class LongPressButton : Button
{
    public float timeLongPress = 0.4f;    // 长按多久开始计算回调
    public float timeTick = 0.2f;     // 长按之后，每隔几秒算一次点击
    public float timeUpstep = 0.02f;  // 长按之后，每次递增多少秒，用来表现越来越快
    public float timeMinTick = 0.0f;  // 递增之后，每隔几秒最小不能小于这个值 

    private bool _isPress = false;
    private float _elapseTime = 0;
    private float _curTick;

    public void Update()
    {
        if (!_isPress)
        {
            return;
        }

        _elapseTime += Time.deltaTime;
        if (_elapseTime < timeLongPress)
        {
            return;
        }

        var elpTime = _elapseTime - timeLongPress;
        if (elpTime > _curTick)
        {
            onClick.Invoke();

            _elapseTime -= _curTick;

            _curTick -= timeUpstep;
            _curTick = Mathf.Max(_curTick, timeMinTick);
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _isPress = true;
        _elapseTime = 0;
        _curTick = timeTick;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        _isPress = false;
    }
}

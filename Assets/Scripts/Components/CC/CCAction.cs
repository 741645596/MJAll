// CCAction.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/26
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Widget;
using UnityEngine;

/// <summary>
/// Ease动画效果：https://blog.csdn.net/qq_44119541/article/details/107397494
/// </summary>
public enum CCActionType
{
    Sequence,
    Spawn,

    EaseExponentialIn,
    EaseExponentialOut,
    MoveTo,
    MoveBy,
    ScaleTo,
    ScaleBy,
    RotateBy,
    RotateTo,
    TintTo,
    TintBy,
    FadeIn,
    FadeOut,
    FadeTo,
    DelayTime,
    CallFunc,
    Empty,
    RepeatForever,
    Jump,
    JumpBy,
}

public class CCActionData
{
    public CCActionType type;
    public object[] args;
    public Action callback;
    public List<CCActionData> subActions = new List<CCActionData>();
    public float delayTime;
    public void pushAction(CCActionData data)
    {
        subActions.Add(data);
    }
}

public class CCAction
{
    private CCActionData _actionData;

    public static CCAction NewSequence()
    {
        return new CCAction(CCActionType.Sequence);
    }

    public static CCAction NewSpawn()
    {
        return new CCAction(CCActionType.Spawn);
    }

    public static CCAction NewRepeatForever()
    {
        return new CCAction(CCActionType.RepeatForever);
    }

    public static CCAction NewEaseExponentialIn()
    {
        return new CCAction(CCActionType.EaseExponentialIn);
    }

    public static CCAction NewEaseExponentialOut()
    {
        return new CCAction(CCActionType.EaseExponentialOut);
    }

    public CCAction(CCActionType actionType)
    {
        _actionData = new CCActionData
        {
            type = actionType
        };
    }

    public CCAction AddAction(CCActionData actionData)
    {
        _actionData.pushAction(actionData);
        return this;
    }

    public CCActionData ToAction()
    {
        return _actionData;
    }

    public CCAction Jump(float duration,
        Vector2 pos,
        float power,
        int numJumps,
        bool snapping = false,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var data = new CCActionData();
        data.type = CCActionType.Jump;
        data.args = new object[] { duration, pos, power, numJumps, snapping, easeType };
        _actionData.pushAction(data);
        return this;
    }

    public CCAction JumpBy(float duration,
        Vector2 pos,
        float power,
        int numJumps,
        bool snapping = false,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var data = new CCActionData();
        data.type = CCActionType.JumpBy;
        data.args = new object[] { duration, pos, power, numJumps, snapping, easeType };
        _actionData.pushAction(data);
        return this;
    }

    public CCAction MoveTo(float duration,
        Vector2 pos,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.MoveTo;
        move.args = new object[] { duration, pos, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction MoveBy(float duration,
        Vector2 pos,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.MoveBy;
        move.args = new object[] { duration, pos, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction ScaleTo(float duration,
        float scale,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.ScaleTo;
        move.args = new object[] { duration, new Vector2(scale, scale), easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction ScaleTo(float duration,
        float sx,
        float sy,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.ScaleTo;
        move.args = new object[] { duration, new Vector2(sx, sy), easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction ScaleBy(float duration,
        float scale,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.ScaleBy;
        move.args = new object[] { duration, new Vector2(scale, scale), easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction RotateTo(float duration,
        float rotate,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.RotateTo;
        move.args = new object[] { duration, rotate, easeType};
        _actionData.pushAction(move);
        return this;
    }

    public CCAction RotateBy(float duration,
        float rotate,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.RotateBy;
        move.args = new object[] { duration, rotate, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction TintTo(float duration,
        Color color,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.TintTo;
        move.args = new object[] { duration, color, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction FadeIn(float duration,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.FadeIn;
        move.args = new object[] { duration, easeType};
        _actionData.pushAction(move);
        return this;
    }

    public CCAction FadeOut(float duration,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.FadeOut;
        move.args = new object[] { duration, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction FadeTo(float duration,
        int endValue,
        DG.Tweening.Ease easeType = DG.Tweening.Ease.Linear)
    {
        var move = new CCActionData();
        move.type = CCActionType.FadeTo;
        move.args = new object[] { duration, endValue, easeType };
        _actionData.pushAction(move);
        return this;
    }

    public CCAction CallFunc(Action callback)
    {
        var action = new CCActionData();
        action.type = CCActionType.Empty;
        action.callback = callback;
        _actionData.pushAction(action);
        return this;
    }

    public CCAction DelayTime(float seconds)
    {
        var action = new CCActionData();
        action.type = CCActionType.Empty;
        action.delayTime = seconds;
        _actionData.pushAction(action);
        return this;
    }

    public void Run(WNode nodee)
    {
        nodee.RunAction(_actionData);
    }

    public void Run(RectTransform transform)
    {
        var sequence = CCActionHelper.GetSequence(transform, _actionData, _actionData.type);
        sequence.Play();
    }
}

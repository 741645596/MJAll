// CCActionHelper.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2019/6/27
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public static class CCActionHelper
{
    public static Sequence GetSequence(RectTransform transform, CCActionData action, CCActionType type)
    {
        Sequence sequence = DOTween.Sequence();
        if (type == CCActionType.RepeatForever)
        {
            sequence.SetLoops(-1, LoopType.Restart);
        }
        if (type == CCActionType.EaseExponentialIn)
        {
            sequence.SetEase(Ease.InExpo);
        }
        if (type == CCActionType.EaseExponentialOut)
        {
            sequence.SetEase(Ease.OutExpo);
        }

        for (int i = 0; i < action.subActions.Count; i++)
        {
            var subAction = action.subActions[i];
            if (subAction == null)
            {
                continue;
            }

            switch (subAction.type)
            {
                case CCActionType.Empty:
                    {
                        Tween tweener = transform.DOLocalRotate(Vector3.zero, 0.0001f, RotateMode.LocalAxisAdd);
                        Action callback = subAction.callback;
                        if (callback != null)
                        {
                            tweener.onComplete += () =>
                            {
                                try
                                {
                                    callback?.Invoke();
                                }
                                catch (Exception ex)
                                {
                                    WLDebug.LogError(ex.ToString());
                                }
                            };
                        }

                        float delayTime = subAction.delayTime;
                        if (delayTime > 0)
                        {
                            tweener.SetDelay(delayTime);
                        }
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }
                case CCActionType.MoveTo:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 pos = (Vector2)subAction.args[1];
                        var easeType = (Ease)subAction.args[2];
                        Tweener tweener = transform.DOAnchorPos(pos, duration).SetEase(easeType);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.MoveBy:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 pos = (Vector2)subAction.args[1];
                        var easeType = (Ease)subAction.args[2];
                        Tweener tweener = transform.DOAnchorPos(pos, duration)
                            .SetEase(easeType)
                            .SetRelative(true);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.ScaleTo:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 scale = (Vector2)subAction.args[1];
                        var easeType = (Ease)subAction.args[2];
                        Tweener tweener = transform.DOScale(new Vector3(scale.x, scale.y, 1), duration);
                        tweener.SetEase(easeType);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }
                case CCActionType.ScaleBy:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 scale = (Vector2)subAction.args[1];
                        var easeType = (Ease)subAction.args[2];
                        Tweener tweener = transform.DOScale(new Vector3(scale.x, scale.y, 1), duration)
                            .SetEase(easeType)
                            .SetRelative(true);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.RotateTo:
                    {
                        float duration = (float)subAction.args[0];
                        float rotate = (float)subAction.args[1];
                        var r = transform.localEulerAngles;
                        var easeType = (Ease)subAction.args[2];
                        Tween tweener = transform.DOLocalRotate(new Vector3(r.x, r.y, transform.localEulerAngles.z),
                            duration,
                            RotateMode.FastBeyond360);
                        tweener.SetEase(easeType);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.RotateBy:
                    {
                        float duration = (float)subAction.args[0];
                        float rotate = (float)subAction.args[1];
                        var r = transform.localEulerAngles;
                        var easeType = (Ease)subAction.args[2];
                        Tween tweener = transform.DOLocalRotate(new Vector3(r.x, r.y, rotate), duration, RotateMode.LocalAxisAdd);
                        tweener.SetEase(easeType);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.TintTo:
                    {
                        Tween empty = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.0001f, RotateMode.LocalAxisAdd);
                        PushActionToSequence(sequence, empty, type);

                        float duration = (float)subAction.args[0];
                        Color c = (Color)subAction.args[1];
                        var easeType = (Ease)subAction.args[2];
                        var images = transform.GetComponentsInChildren<Image>();
                        for (int j = 0; j < images.Length; j++)
                        {
                            Tween tweener = images[j].DOColor(c, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                           
                        var texts = transform.GetComponentsInChildren<Text>();
                        for (int k = 0; k < texts.Length; k++)
                        {
                            Tween tweener = texts[k].DOColor(c, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                        break;
                    }
                case CCActionType.TintBy:
                    break;
                case CCActionType.FadeIn:
                    {
                        Tween empty = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.0001f, RotateMode.LocalAxisAdd);
                        PushActionToSequence(sequence, empty, type);

                        float duration = (float)subAction.args[0];
                        var easeType = (Ease)subAction.args[1];

                        var canvsGroup = transform.GetComponent<CanvasGroup>();
                        if (canvsGroup != null)
                        {
                            Tween tweener = canvsGroup.DOFade(1, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                            break;
                        }

                        var images = transform.GetComponentsInChildren<Image>();
                        for (int j = 0; j < images.Length; j++)
                        {
                            Tween tweener = images[j].DOFade(1, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                          
                        var texts = transform.GetComponentsInChildren<Text>();
                        for (int k = 0; k < texts.Length; k++)
                        {
                            Tween tweener = texts[k].DOFade(1, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }

                        break;
                    }
                case CCActionType.FadeOut:
                    {
                        Tween empty = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.0001f, RotateMode.LocalAxisAdd);
                        PushActionToSequence(sequence, empty, type);

                        float duration = (float)subAction.args[0];
                        var easeType = (Ease)subAction.args[1];
                        var images = transform.GetComponentsInChildren<Image>();
                        for (int j = 0; j < images.Length; j++)
                        {
                            Tween tweener = images[j].DOFade(0, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                           
                        var texts = transform.GetComponentsInChildren<Text>();
                        for (int k = 0; k < texts.Length; k++)
                        {
                            Tween tweener = texts[k].DOFade(0, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                        break;
                    }

                case CCActionType.FadeTo:
                    {
                        Tween empty = transform.DOLocalRotate(new Vector3(0, 0, 0), 0.0001f, RotateMode.LocalAxisAdd);
                        PushActionToSequence(sequence, empty, type);

                        float duration = (float)subAction.args[0];
                        float endValue = ((int)subAction.args[1]) / 255.0f;
                        var easeType = (Ease)subAction.args[2];
                        var images = transform.GetComponentsInChildren<Image>();
                        for (int j = 0; j < images.Length; j++)
                        {
                            Tween tweener = images[j].DOFade(endValue, duration).SetEase(easeType);
                            PushActionToSequence(sequence, tweener, CCActionType.Spawn);
                        }
                           
                        break;
                    }

                case CCActionType.Jump:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 pos = (Vector2)subAction.args[1];
                        float power = (float)subAction.args[2];
                        int numJumps = (int)subAction.args[3];
                        bool snapping = (bool)subAction.args[4];
                        var easeType = (Ease)subAction.args[5];
                        Tween tweener = transform.DOJumpAnchorPos(pos, power, numJumps, duration, snapping);
                        tweener.SetEase(easeType);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }
                case CCActionType.JumpBy:
                    {
                        float duration = (float)subAction.args[0];
                        Vector2 pos = (Vector2)subAction.args[1];
                        float power = (float)subAction.args[2];
                        int numJumps = (int)subAction.args[3];
                        bool snapping = (bool)subAction.args[4];
                        var easeType = (Ease)subAction.args[5];
                        Tween tweener = transform.DOJumpAnchorPos(pos, power, numJumps, duration, snapping);
                        tweener.SetEase(easeType);
                        tweener.SetRelative(true);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                case CCActionType.Sequence:
                case CCActionType.RepeatForever:
                case CCActionType.Spawn:
                case CCActionType.EaseExponentialIn:
                case CCActionType.EaseExponentialOut:
                    {
                        Tween tweener = GetSequence(transform, subAction, subAction.type);
                        PushActionToSequence(sequence, tweener, type);
                        break;
                    }

                default:
                    WLDebug.LogWarning("不支持的Action类型：", subAction.type);
                    break;
            }

        }
        sequence.SetTarget(transform.gameObject);
        sequence.SetLink(transform.gameObject);
        sequence.SetAutoKill(true);
        return sequence;
    }

    private static void PushActionToSequence(Sequence sequence, Tween tweener, CCActionType type)
    {
        if (type == CCActionType.Sequence
            || type == CCActionType.RepeatForever
            || type == CCActionType.EaseExponentialIn
            || type == CCActionType.EaseExponentialOut)
        {
            sequence.Append(tweener);
        }

        if (type == CCActionType.Spawn)
        {
            sequence.Join(tweener);
        }
    }
}


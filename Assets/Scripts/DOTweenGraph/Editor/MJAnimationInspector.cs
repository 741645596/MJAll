
#if UNITY_EDITOR 

// 使用using UnityEditor时注意：该库只能在编辑器模式下使用，记得宏定义防止真机编译不通过
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(AnimationT))]
public class MJAnimationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("播放动画"))
        {
            //Logic
            AnimationT ctr = target as AnimationT;
            ctr.Play();
        }

        if (GUILayout.Button("向骨骼插入麻将牌"))
        {
            //Logic
            AnimationT ctr = target as AnimationT;
            ctr.InitCards();
        }
    }
}

[CustomEditor(typeof(HandSetAnimationT))]
public class HandSetAnimationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("播放动画"))
        {
            //Logic
            HandSetAnimationT ctr = target as HandSetAnimationT;
            ctr.Play();
        }
        if (GUILayout.Button("重置"))
        {
            //Logic
            HandSetAnimationT ctr = target as HandSetAnimationT;
            ctr.Reset();
        }
    }
}

[CustomEditor(typeof(DOTweenAnimation))]
public class DOTweenAnimationInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("播放动画"))
        {
            DOTweenAnimation ctr = target as DOTweenAnimation;
            ctr.Play();
        }

        if (GUILayout.Button("重置"))
        {
            DOTweenAnimation ctr = target as DOTweenAnimation;
            ctr.ResetTransform();
        }
    }
}

#endif
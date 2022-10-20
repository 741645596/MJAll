// @Author: tanjinhua
// @Date: 2021/1/19  20:58


using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressIndicator), true)]
public class ProgressIndicatorEditor : Editor
{
    SerializedProperty spIndicateMode;
    SerializedProperty spHorizontalOrigin;
    SerializedProperty spVerticalOrigin;
    SerializedProperty spRadialOrigin;
    SerializedProperty spClockwise;

    SerializedProperty spProgressGraphic;
    SerializedProperty spProgress;

    SerializedProperty spUseMask;
    SerializedProperty spMask;

    private void OnEnable()
    {
        spIndicateMode = serializedObject.FindProperty("_indicateMode");
        spHorizontalOrigin = serializedObject.FindProperty("_horizontalOrigin");
        spVerticalOrigin = serializedObject.FindProperty("_verticalOrigin");
        spRadialOrigin = serializedObject.FindProperty("_radialOrigin");
        spClockwise = serializedObject.FindProperty("_clockwise");

        spProgressGraphic = serializedObject.FindProperty("_progressGraphic");
        spProgress = serializedObject.FindProperty("_progress");

        spUseMask = serializedObject.FindProperty("_useMask");
        spMask = serializedObject.FindProperty("_mask");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(spIndicateMode, new GUIContent("Indicate Mode", "进度显示方式"));
        if (spIndicateMode.enumValueIndex == 0 || spIndicateMode.enumValueIndex == 2)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spHorizontalOrigin, new GUIContent("Horizontal Origin", "水平进度开始位置"));
            EditorGUILayout.EndHorizontal();
        }
        else if (spIndicateMode.enumValueIndex == 1 || spIndicateMode.enumValueIndex == 3)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spVerticalOrigin, new GUIContent("Vertical Origin", "垂直进度开始位置"));
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spRadialOrigin, new GUIContent("Radial Origin", "圆形进度开始位置"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spClockwise, new GUIContent("Clockwise", "是否顺时针方向填充"));
            EditorGUILayout.EndHorizontal();
        }


        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(spProgressGraphic);
        spProgress.floatValue = EditorGUILayout.Slider(new GUIContent("Progress", "进度"), spProgress.floatValue, 0f, 1f);


        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(spUseMask, new GUIContent("Use Mask", "是否使用遮罩(模版裁剪)"));
        if (spUseMask.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spMask);
            EditorGUILayout.EndHorizontal();
        }


        serializedObject.ApplyModifiedProperties();
    }
}

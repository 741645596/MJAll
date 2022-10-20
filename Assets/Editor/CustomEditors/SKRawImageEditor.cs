// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SKRawImage), true)]
[CanEditMultipleObjects]
public class SKRawImageEditor : RawImageEditor
{
    SerializedProperty _skew;
    SerializedProperty _rtSystem;

    protected override void OnEnable()
    {
        base.OnEnable();

        _skew = serializedObject.FindProperty("_skew");
        _rtSystem = serializedObject.FindProperty("_rtSystem");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_skew);

        EditorGUILayout.PropertyField(_rtSystem);

        serializedObject.ApplyModifiedProperties();
    }
}
// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SKText), true)]
[CanEditMultipleObjects]
public class SKTextEditor : TextEditor
{
    SerializedProperty _skew;
    SerializedProperty _adaptedSize;
    SerializedProperty _dimensions;

    protected override void OnEnable()
    {
        base.OnEnable();

        _skew = serializedObject.FindProperty("_skew");
        _adaptedSize = serializedObject.FindProperty("_adaptedSize");
        _dimensions = serializedObject.FindProperty("_dimensions");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_adaptedSize);

        EditorGUILayout.PropertyField(_dimensions);

        EditorGUILayout.PropertyField(_skew);

        serializedObject.ApplyModifiedProperties();
    }
}

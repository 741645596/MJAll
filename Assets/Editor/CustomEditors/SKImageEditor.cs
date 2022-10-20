// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SKImage), true)]
[CanEditMultipleObjects]
public class SKImageEditor : ImageEditor
{
    SerializedProperty _skew;


    protected override void OnEnable()
    {
        base.OnEnable();

        _skew = serializedObject.FindProperty("_skew");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(_skew);

        serializedObject.ApplyModifiedProperties();
    }
}

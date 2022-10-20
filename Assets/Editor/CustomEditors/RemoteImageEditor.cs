// @Author: tanjinhua
// @Date: 2021/4/24  23:15


using UnityEditor;

[CustomEditor(typeof(RemoteImage), true)]
[CanEditMultipleObjects]
public class RemoteImageEditor : SKImageEditor
{
    SerializedProperty spUrl;
    SerializedProperty spAutoUpdateSize;

    protected override void OnEnable()
    {
        base.OnEnable();

        spUrl = serializedObject.FindProperty("_url");
        spAutoUpdateSize = serializedObject.FindProperty("autoUpdateSize");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        spUrl.stringValue = EditorGUILayout.TextField("Url", spUrl.stringValue);

        EditorGUILayout.PropertyField(spAutoUpdateSize);

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}

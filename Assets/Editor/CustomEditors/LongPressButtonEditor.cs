// @Author: tanjinhua
// @Date: 2020/12/26  18:27


using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(LongPressButton), true)]
[CanEditMultipleObjects]
public class LongPressButtonEditor : ButtonEditor
{
    SerializedProperty timeLongPress;
    SerializedProperty timeTick;
    SerializedProperty timeUpstep;
    SerializedProperty timeMinTick;

    protected override void OnEnable()
    {
        base.OnEnable();

        timeLongPress = serializedObject.FindProperty("timeLongPress");
        timeTick = serializedObject.FindProperty("timeTick");
        timeUpstep = serializedObject.FindProperty("timeUpstep");
        timeMinTick = serializedObject.FindProperty("timeMinTick");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        timeLongPress.floatValue = EditorGUILayout.FloatField("timeLongPress", timeLongPress.floatValue);
        timeTick.floatValue = EditorGUILayout.FloatField("timeTick", timeTick.floatValue);
        timeUpstep.floatValue = EditorGUILayout.FloatField("timeUpstep", timeUpstep.floatValue);
        timeMinTick.floatValue = EditorGUILayout.FloatField("timeMinTick", timeMinTick.floatValue);

        serializedObject.ApplyModifiedProperties();
    }
}

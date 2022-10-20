// @Author: tanjinhua
// @Date: 2021/1/19  10:33


using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PageIndexer), true)]
public class PageIndexerEditor : Editor
{
    SerializedProperty spLayoutAxis;
    SerializedProperty spSelectMode;
    SerializedProperty spSelectedColor;
    SerializedProperty spNormalSprite;
    SerializedProperty spSelectedSprite;
    SerializedProperty spIndexerSize;
    SerializedProperty spSpacing;
    SerializedProperty spSelectedIndex;
    SerializedProperty spSelectedScale;
    SerializedProperty spCount;
    SerializedProperty spGraphicSettings;

    private void OnEnable()
    {
        spLayoutAxis = serializedObject.FindProperty("_layoutAxis");
        spSelectMode = serializedObject.FindProperty("_selectMode");
        spSelectedColor = serializedObject.FindProperty("_selectedColor");
        spNormalSprite = serializedObject.FindProperty("_normalSprite");
        spSelectedSprite = serializedObject.FindProperty("_selectedSprite");
        spIndexerSize = serializedObject.FindProperty("_indexerSize");
        spSpacing = serializedObject.FindProperty("_spacing");
        spSelectedIndex = serializedObject.FindProperty("_selectedIndex");
        spSelectedScale = serializedObject.FindProperty("_selectedScale");
        spCount = serializedObject.FindProperty("_count");
        spGraphicSettings = serializedObject.FindProperty("_graphicSettings");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(spLayoutAxis);

        EditorGUILayout.PropertyField(spSelectMode);

        if (spSelectMode.enumValueIndex == 0)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spNormalSprite);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spSelectedColor);
            EditorGUILayout.EndHorizontal();
        }
        else if (spSelectMode.enumValueIndex == 1)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spNormalSprite);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spSelectedSprite);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.PropertyField(spIndexerSize);

        EditorGUILayout.PropertyField(spSpacing);

        EditorGUILayout.PropertyField(spSelectedIndex);

        EditorGUILayout.PropertyField(spSelectedScale);

        EditorGUILayout.PropertyField(spCount);

        EditorGUILayout.PropertyField(spGraphicSettings);

        serializedObject.ApplyModifiedProperties();
    }
}

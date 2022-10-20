// @Author: tanjinhua
// @Date: 2021/1/18  10:06


using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PageRect), true)]
public class PageRectEditor : Editor
{
    SerializedProperty spScrollRect;
    SerializedProperty spDirection;
    SerializedProperty spCurrentIndex;
    SerializedProperty spFrozen;
    SerializedProperty spMasking;
    SerializedProperty spPageIndexer;
    SerializedProperty spPages;

    private void OnEnable()
    {
        spScrollRect = serializedObject.FindProperty("_scrollRect");
        spDirection = serializedObject.FindProperty("_direction");
        spCurrentIndex = serializedObject.FindProperty("_currentIndex");
        spFrozen = serializedObject.FindProperty("_frozen");
        spMasking = serializedObject.FindProperty("_masking");
        spPageIndexer = serializedObject.FindProperty("_pageIndexer");
        spPages = serializedObject.FindProperty("_pages");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(spScrollRect);

        EditorGUILayout.PropertyField(spDirection, new GUIContent("Direction", "翻页方向"));

        EditorGUILayout.PropertyField(spCurrentIndex, new GUIContent("Current Index", "当前页索引"));

        EditorGUILayout.PropertyField(spFrozen, new GUIContent("Frozen", "是否屏蔽触摸操作"));

        EditorGUILayout.PropertyField(spMasking, new GUIContent("Masking", "是否裁剪掉视窗范围以外的内容"));

        EditorGUILayout.PropertyField(spPageIndexer, new GUIContent("Page Indexer", "索引指示器(page tips)"));

        DrawPagesField();


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPagesField()
    {
        EditorGUILayout.PropertyField(spPages);
        // TODO: 支持拖拽牌排序等操作
    }
}

// @Author: tanjinhua
// @Date: 2021/1/12  15:05


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ArtText), true)]
public class ArtTextEditor : Editor
{

    SerializedProperty spText;
    SerializedProperty spPadding;
    SerializedProperty spSpacing;
    SerializedProperty spAlignment;
    SerializedProperty spUniformWidth;
    SerializedProperty spAdaptedSize;
    SerializedProperty spGraphicSettings;
    SerializedProperty spFontSettings;

    SerializedProperty spSimulatePerspectiveEffect;
    SerializedProperty spPerspectiveOrigin;
    SerializedProperty spPerspectiveIncreasement;

    bool fontSettingsFoldState = true;

    private void OnEnable()
    {
        spText = serializedObject.FindProperty("_text");
        spPadding = serializedObject.FindProperty("_padding");
        spSpacing = serializedObject.FindProperty("_spacing");
        spAlignment = serializedObject.FindProperty("_alignment");
        spUniformWidth = serializedObject.FindProperty("_uniformWidth");
        spAdaptedSize = serializedObject.FindProperty("_adaptedSize");
        spGraphicSettings = serializedObject.FindProperty("_graphicSettings");
        spFontSettings = serializedObject.FindProperty("_fontSettings");

        spSimulatePerspectiveEffect = serializedObject.FindProperty("_simulatePerspectiveEffect");
        spPerspectiveOrigin = serializedObject.FindProperty("_perspectiveOrigin");
        spPerspectiveIncreasement = serializedObject.FindProperty("_perspectiveIncreasement");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawTextField();

        DrawAlignmentField();

        DrawPaddingField();

        DrawSpaceField();

        DrawUniformWidthField();

        DrawAdaptedSizeField();

        DrawGraphicSettingsField();

        DrawPerspectiveSimulateField();

        DrawFontSettings();


        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTextField()
    {
        EditorGUILayout.LabelField(new GUIContent("Text", "输入字符串"));
        spText.stringValue = EditorGUILayout.TextField(spText.stringValue, GUILayout.Height(55));
        EditorGUILayout.Space(5);
    }

    private void DrawPaddingField()
    {
        GUIContent label = new GUIContent("Padding", "上下左右缩进");
        EditorGUILayout.PropertyField(spPadding, label);
    }

    private void DrawAlignmentField()
    {
        GUIContent label = new GUIContent("Alignment", "垂直方向对齐方式，字符精灵图片高度不统一时可用。默认居中");
        spAlignment.enumValueIndex = (int)(ArtText.Alignment)EditorGUILayout.EnumPopup(label, (ArtText.Alignment)spAlignment.enumValueIndex);
    }

    private void DrawSpaceField()
    {
        GUIContent label = new GUIContent("Spacing", "X轴字符间距");
        spSpacing.floatValue = EditorGUILayout.FloatField(label, spSpacing.floatValue);
    }

    private void DrawUniformWidthField()
    {
        GUIContent label = new GUIContent("Use Uniform Width", "是否要使用统一字符宽度(以最宽的字符为准)");
        spUniformWidth.boolValue = EditorGUILayout.Toggle(label, spUniformWidth.boolValue);
    }

    private void DrawAdaptedSizeField()
    {
        GUIContent label = new GUIContent("Adapted Size", "是否自动更新节点尺寸");
        spAdaptedSize.boolValue = EditorGUILayout.Toggle(label, spAdaptedSize.boolValue);
        EditorGUILayout.Space(5);
    }

    private void DrawGraphicSettingsField()
    {
        EditorGUILayout.PropertyField(spGraphicSettings);
        EditorGUILayout.Space(5);
    }

    private void DrawPerspectiveSimulateField()
    {
        GUIContent label = new GUIContent("Simulate Perspective Effect", "是否模拟透视效果");
        spSimulatePerspectiveEffect.boolValue = EditorGUILayout.Toggle(label, spSimulatePerspectiveEffect.boolValue);

        if (spSimulatePerspectiveEffect.boolValue)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spPerspectiveOrigin);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(15);
            EditorGUILayout.PropertyField(spPerspectiveIncreasement);
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.Space(5);
    }

    private void DrawFontSettings()
    {
        fontSettingsFoldState = EditorGUILayout.Foldout(fontSettingsFoldState, new GUIContent("Font Settings", "字符与精灵图片的映射关系配置"), true, EditorStyles.foldout);

        if (fontSettingsFoldState)
        {
            if (spFontSettings.arraySize == 0)
            {
                Rect rect = EditorGUILayout.BeginVertical();
                GUILayout.Box("\n拖拽精灵图片到这里", GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(55), GUILayout.ExpandWidth(true));
                EditorGUILayout.EndVertical();

                Event e = Event.current;
                if (rect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    else if (e.type == EventType.DragPerform)
                    {
                        e.Use();
                        string[] parths = DragAndDrop.paths;
                        List<Sprite> sprites = new List<Sprite>();
                        for (int i = 0; parths != null && i < parths.Length; i++)
                        {
                            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(parths[i]);
                            foreach (Object o in objects)
                            {
                                if (o is Sprite s)
                                {
                                    sprites.Add(s);
                                }
                            }
                        }

                        spFontSettings.arraySize = sprites.Count;
                        for (int i = 0; i < sprites.Count; i++)
                        {
                            SerializedProperty spFontSetting = spFontSettings.GetArrayElementAtIndex(i);
                            SerializedProperty spCharacter = spFontSetting.FindPropertyRelative("character");
                            spCharacter.stringValue = sprites[i].name;
                            SerializedProperty spSprite = spFontSetting.FindPropertyRelative("sprite");
                            spSprite.objectReferenceValue = sprites[i];
                        }
                    }
                }
            }
            else
            {
                int size = EditorGUILayout.IntField("   Count", spFontSettings.arraySize);
                spFontSettings.arraySize =  size < 0 ? 0 : size;

                EditorGUILayout.Space(6);

                for (int i = 0; i < spFontSettings.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty spFontSetting = spFontSettings.GetArrayElementAtIndex(i);

                    EditorGUILayout.LabelField("Character", GUILayout.Width(60));
                    SerializedProperty spCharacter = spFontSetting.FindPropertyRelative("character");
                    spCharacter.stringValue = EditorGUILayout.TextField(spCharacter.stringValue, GUILayout.Width(40));

                    GUILayout.Space(5);

                    SerializedProperty spSprite = spFontSetting.FindPropertyRelative("sprite");
                    EditorGUILayout.PropertyField(spSprite, GUIContent.none);

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}

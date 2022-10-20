// EffectShader.cs
// Author: shihongyang shihongyang@weile.com
// Data: 2021/11/11

using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class EffectShader : ShaderGUI
{
    private const int bigFontSize = 13, smallFontSize = 11;

    private CompareFunction zTestMode = CompareFunction.LessEqual;
    private CullMode cullMode = CullMode.Back;
    private Material targetMat;
    private MaterialEditor matEditor;
    private MaterialProperty[] matProperties;
    private string[] oldKeyWords;
    private GUIStyle style, bigLabelStyle, smallLabelStyle;
    private SrcBlendMode srcMode;
    private DstBlendMode dstMode;

    private enum SrcBlendMode
    {
        Additive = BlendMode.SrcAlpha,
        Multiply = BlendMode.Zero
    }

    private enum DstBlendMode
    {
        Alpha = BlendMode.OneMinusSrcAlpha,
        Additive = BlendMode.One,
        Multiply = BlendMode.SrcColor
    }


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        matEditor = materialEditor;
        matProperties = properties;
        bigLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        bigLabelStyle.fontSize = bigFontSize;

        style = new GUIStyle(EditorStyles.helpBox);
        smallLabelStyle = new GUIStyle(EditorStyles.boldLabel);
        smallLabelStyle.fontSize = smallFontSize;

        targetMat = materialEditor.target as Material;
        oldKeyWords = targetMat.shaderKeywords;

        GUILayout.Label("基础属性", bigLabelStyle);
        DrawProperty(0);
        DrawProperty(1);

        EditorGUILayout.Separator();
        GUILayout.Label("菲涅尔", smallLabelStyle);
        DrawProperty(19);
        DrawProperty(20);
        DrawProperty(21);

        EditorGUILayout.Separator();
        Blending("自定义 Blend", "Blend");
        Cull("自定义 Cull", "Cull");
        ZWrite("自定义 Z Write");
        ZTest("自定义 Z Test", "ZTest");

        DrawLine(Color.grey, 1, 3);
        GUILayout.Label("功能属性", bigLabelStyle);

        GenericEffect("UV动画", "TEXTURESCROLL_ON", 17, 18);
        GenericEffect("扰动", "DISTORT_ON", 2, 5);
        GenericEffect("溶解", "DISSOLVE_ON", 6, 8);
        GenericEffect("模糊", "BLUR_ON", 9, 9);
        GenericEffect("边缘发光", "OUTLINE_ON", 10, 16);
    }

    private void DrawProperty(int index, bool noReset = false)
    {
        MaterialProperty targetProperty = matProperties[index];

        EditorGUILayout.BeginHorizontal();
        {
            GUIContent propertyLabel = new GUIContent();
            propertyLabel.text = targetProperty.displayName;

            matEditor.ShaderProperty(targetProperty, propertyLabel);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void GenericEffect(string inspector, string keyword, int first, int last, bool effectCounter = true, string preMessage = null, int[] extraProperties = null)
    {
        bool toggle = oldKeyWords.Contains(keyword);
        bool ini = toggle;

        GUIContent effectNameLabel = new GUIContent();
        effectNameLabel.tooltip = keyword + " (C#)";
        if (effectCounter)
        {
            effectNameLabel.text = inspector;
            toggle = EditorGUILayout.BeginToggleGroup(effectNameLabel, toggle);
        }
        else
        {
            effectNameLabel.text = inspector;
            toggle = EditorGUILayout.BeginToggleGroup(effectNameLabel, toggle);
        }

        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(keyword);
            if (first > 0)
            {
                EditorGUILayout.BeginVertical(style);
                {
                    if (preMessage != null) GUILayout.Label(preMessage, smallLabelStyle);
                    for (int i = first; i <= last; i++) DrawProperty(i);
                    if (extraProperties != null) foreach (int i in extraProperties) DrawProperty(i);
                }
                EditorGUILayout.EndVertical();
            }
        }
        else targetMat.DisableKeyword(keyword);
        EditorGUILayout.EndToggleGroup();
    }

    private void Blending(string inspector, string keyword)
    {
        bool toggle = oldKeyWords.Contains(keyword);
        MaterialProperty srcM = FindProperty("_MySrcMode", matProperties);
        MaterialProperty dstM = FindProperty("_MyDstMode", matProperties);
        if (srcM.floatValue == 0 && dstM.floatValue == 0)
        {
            srcM.floatValue = 5;
            dstM.floatValue = 10;
        }
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(keyword);
            EditorGUILayout.BeginVertical(style);
            {
                if (GUILayout.Button("重置为默认值"))
                {
                    srcM.floatValue = 5;
                    dstM.floatValue = 10;
                }
                srcMode = (SrcBlendMode)srcM.floatValue;
                dstMode = (DstBlendMode)dstM.floatValue;
                srcMode = (SrcBlendMode)EditorGUILayout.EnumPopup("SrcMode", srcMode);
                dstMode = (DstBlendMode)EditorGUILayout.EnumPopup("DstMode", dstMode);
                srcM.floatValue = (float)(srcMode);
                dstM.floatValue = (float)(dstMode);
            }
            EditorGUILayout.EndVertical();
        }
        else
        {
            srcM.floatValue = 5;
            dstM.floatValue = 10;
            targetMat.DisableKeyword(keyword);
        }
        EditorGUILayout.EndToggleGroup();
    }

    private void DrawLine(Color color, int thickness = 2, int padding = 10)
    {
        Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
        r.height = thickness;
        r.y += (padding / 2);
        r.x -= 2;
        r.width += 6;
        EditorGUI.DrawRect(r, color);
    }

    private void ZWrite(string inspector)
    {
        MaterialProperty zWrite = ShaderGUI.FindProperty("_ZWrite", matProperties);
        bool toggle = zWrite.floatValue > 0.9f ? true : false;
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            zWrite.floatValue = 1.0f;
        }
        else
        {
            zWrite.floatValue = 0.0f;
        }
        EditorGUILayout.EndToggleGroup();
    }

    private void ZTest(string inspector, string keyword)
    {
        bool toggle = oldKeyWords.Contains(keyword);
        MaterialProperty zTestM = ShaderGUI.FindProperty("_ZTestMode", matProperties);
        if (zTestM.floatValue == 0) zTestM.floatValue = 4;
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(keyword);
            EditorGUILayout.BeginVertical(style);
            {
                zTestMode = (CompareFunction)zTestM.floatValue;
                zTestMode = (CompareFunction)EditorGUILayout.EnumPopup("Z Test Mode", zTestMode);
                zTestM.floatValue = (float)(zTestMode);
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword(keyword);
        EditorGUILayout.EndToggleGroup();
    }

    private void Cull(string inspector, string keyword)
    {
        bool toggle = oldKeyWords.Contains(keyword);
        MaterialProperty cullM = ShaderGUI.FindProperty("_CullMode", matProperties);
        bool ini = toggle;
        toggle = EditorGUILayout.BeginToggleGroup(inspector, toggle);
        if (ini != toggle && !Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        if (toggle)
        {
            targetMat.EnableKeyword(keyword);
            EditorGUILayout.BeginVertical(style);
            {
                cullMode = (CullMode)cullM.floatValue;
                cullMode = (CullMode)EditorGUILayout.EnumPopup("Cull Mode", cullMode);
                cullM.floatValue = (float)(cullMode);
            }
            EditorGUILayout.EndVertical();
        }
        else targetMat.DisableKeyword(keyword);
        EditorGUILayout.EndToggleGroup();
    }
}


using System;
using UnityEditor;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using static UnityEditor.Rendering.Universal.ShaderGUI.ParticleGUI;

public class PariclesShaderGUI : BaseShaderGUI
{
    private MaterialProperty colorMode;

    public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
    {
        if (materialEditorIn == null)
            throw new ArgumentNullException("materialEditorIn");

        FindProperties(properties); // MaterialProperties can be animated so we do not cache them but fetch them every event to ensure animated values are updated correctly

        materialEditor = materialEditorIn;
        Material material = materialEditor.target as Material;

        if (m_FirstTimeApply)
        {
            OnOpenGUI(material, materialEditorIn);
            m_FirstTimeApply = false;
        }

        DrawShaderGUI(material);
    }

    public override void DrawSurfaceOptions(Material material)
    {
        DoPopup(Styles.surfaceType, surfaceTypeProp, Enum.GetNames(typeof(SurfaceType)));
        if ((SurfaceType)material.GetFloat("_Surface") == SurfaceType.Transparent)
            DoPopup(Styles.blendingMode, blendModeProp, Enum.GetNames(typeof(BlendMode)));

        EditorGUI.showMixedValue = false;

        EditorGUI.BeginChangeCheck();
        EditorGUI.showMixedValue = alphaClipProp.hasMixedValue;
        var alphaClipEnabled = EditorGUILayout.Toggle(Styles.alphaClipText, alphaClipProp.floatValue == 1);
        if (EditorGUI.EndChangeCheck())
            alphaClipProp.floatValue = alphaClipEnabled ? 1 : 0;
        EditorGUI.showMixedValue = false;

        if (alphaClipProp.floatValue == 1)
            materialEditor.ShaderProperty(alphaCutoffProp, Styles.alphaClipThresholdText, 1);

        if (receiveShadowsProp != null)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = receiveShadowsProp.hasMixedValue;
            var receiveShadows =
                EditorGUILayout.Toggle(Styles.receiveShadowText, receiveShadowsProp.floatValue == 1.0f);
            if (EditorGUI.EndChangeCheck())
                receiveShadowsProp.floatValue = receiveShadows ? 1.0f : 0.0f;
            EditorGUI.showMixedValue = false;
        }
    }

    public override void FindProperties(MaterialProperty[] properties)
    {
        base.FindProperties(properties);
        colorMode = FindProperty("_ColorMode", properties);
    }

    public void DrawShaderGUI(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        EditorGUI.BeginChangeCheck();

        DrawSurfaceInputs(material);
        DrawEmissionProperties(material, true);
        EditorGUILayout.Space();

        DoPopup(ParticleGUI.Styles.colorMode, colorMode, Enum.GetNames(typeof(ColorMode)));
        DrawSurfaceOptions(material);
        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in materialEditor.targets)
            {
                MaterialChanged((Material)obj);
            }
        }
    }

    public override void MaterialChanged(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        SetMaterialKeywords(material, null, ParticleGUI.SetMaterialKeywords);
    }
}

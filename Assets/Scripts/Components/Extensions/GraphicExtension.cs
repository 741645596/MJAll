// @Author: tanjinhua
// @Date: 2021/1/22  9:00


using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public static class GraphicExtension
{
    /// <summary>
    /// 提取图像设置数据
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static GraphicSettings FetchGraphicSettings(this Graphic graphic, GraphicSettings settings = null)
    {
        string shaderName = "UI/Custom";
        settings = settings == null ? new GraphicSettings
        {
            color = graphic.color,
            maskable = (graphic is MaskableGraphic mg) && mg.maskable,
            greyScaleOnly = graphic.material.shader.name == shaderName && graphic.material.GetInt("_GreyShade") == 1,
            darkenAmount = graphic.material.shader.name == shaderName ? graphic.material.GetFloat("_DarkenAmount") : 0f,
            blendModeSrc = graphic.material.shader.name == shaderName ? (BlendMode)graphic.material.GetInt("_SrcBlendMode") : BlendMode.SrcAlpha,
            blendModeDst = graphic.material.shader.name == shaderName ? (BlendMode)graphic.material.GetInt("_DstBlendMode") : BlendMode.OneMinusSrcAlpha,
            skew = Vector2.zero
        } : settings;

        if (graphic is SKImage image)
        {
            settings.skew = image.skew;
        }
        if (graphic is SKText text)
        {
            settings.skew = text.skew;
        }
        if (graphic is SKRawImage rawImage)
        {
            settings.skew = rawImage.skew;
        }
        return settings;
    }

    /// <summary>
    /// 应用图像设置数据
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="settings"></param>
    /// <param name="useNewMaterial"></param>
    public static void ApplyGraphicSettings(this Graphic graphic, GraphicSettings settings, bool useNewMaterial)
    {
        WLDebug.LogWarning("ApplyGraphicSettings 该接口已废弃，请勿使用");
        //Material material = graphic.GetMaterialFromCustomShader(useNewMaterial);
        //graphic.ApplyGraphicSettings(settings, material);
    }

    /// <summary>
    /// 同上
    /// </summary>
    /// <param name="graphic"></param>
    /// <param name="settings"></param>
    /// <param name="material"></param>
    /// <param name="setsShaderParams"></param>
    public static void ApplyGraphicSettings(this Graphic graphic, GraphicSettings settings, Material material, bool setsShaderParams = true)
    {
        graphic.material = material;
        graphic.color = settings.color;
        if (graphic is MaskableGraphic mg)
        {
            mg.maskable = settings.maskable;
        }
        if (graphic is SKImage image)
        {
            image.skew = settings.skew;
        }
        if (graphic is SKText text)
        {
            text.skew = settings.skew;
        }
        if (graphic is SKRawImage rawImage)
        {
            rawImage.skew = settings.skew;
        }
        if (setsShaderParams)
        {
            //material?.SetUICustomShaderParams(settings);
        }
    }

    //private static Material GetMaterialFromCustomShader(this Graphic graphic, bool useNewMaterial = false)
    //{
    //    if (graphic.material.shader.name != "UI/Custom" || useNewMaterial)
    //    {
    //        graphic.material = new Material(Shader.Find("UI/Custom"));
    //    }

    //    return graphic.material;
    //}
}

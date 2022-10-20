// @Author: futianfu
// @Date: 2021/9/22 17:03:19


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 预制UI检测逻辑
/// </summary>
public class PrefabUIChecker : AssetsCheckerBase<PrefabUIAssetInfo>
{
    private AssetsCheckEditorWindow.Settings _settings;

    public PrefabUIChecker(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// 收集文件信息
    /// </summary>
    /// <returns>例：文件的项目路径，文件全路径，投射阴影是否打开，接收阴影是否打开，光照探针是否打开，反射探针是否打开</returns>
    public override List<PrefabUIAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, _settings.assetsRootPath);

        var list = new List<PrefabUIAssetInfo>();

        var      pattern = "*.prefab";
        string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories); //获取所有预制文件

        foreach (var file in files) //遍历所有预制文件路径集合
        {
            var info = new PrefabUIAssetInfo(); //信息对象初始化

            _SetFileSize(file, info); //设置文件大小

            var projectPath = file.Replace(Application.dataPath, "Assets"); //项目路径，Asset/开头
            info.foldName = projectPath;                                    //文件名【项目路径Asset开头】
            info.filePath = file;                                           //文件路径

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(projectPath).transform; //预制的文件信息
            if (prefab == null)
            {
                continue;
            }

            if (prefab.GetComponentsInChildren<RectTransform>(true).Length < 1) //排除非UI组件预制
            {
                continue;
            }

            _Check(prefab, info); //检测是否需要修复

            list.Add(info);
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }

    /// <summary>
    /// 查找所有节点射线检测情况
    /// </summary>
    /// <param name="prefab"> 预制 </param>
    /// <param name="info"> 文件的检测信息 </param>
    private static void _Check(Transform tra, PrefabUIAssetInfo info)
    {
        if (_IsSliderType(tra)) //组件是Slider || Toggle
        {
            // 不处理，不递归子节点
        }
        else if (_IsButtonType(tra)) //如果预制内有 Button InputField ScrollRect 组件，再查找内部
        {
            //如果有按钮上的图片没打开，就开下Ray
            if (_IsCloseUIRay(tra))
                info.btnOpenRay = true;

            // 不处理当前节点，但是递归子节点

            for (int i = 0; i < tra.childCount; i++)
            {
                Transform childTra = tra.GetChild(i);
                if (_IsImaTxtType(childTra)) //is image or text 相关类型组件
                {
                    if (_IsOpenUIRay(childTra))   //是否开启射线
                        info.needCloseRay = true; // 需要关闭Raycast Target;
                }

                _Check(childTra, info); //递归子节点
            }
        }
        else
        {
            if (_IsImaTxtType(tra)) //is image or text 相关类型组件
            {
                if (_IsOpenUIRay(tra))       //是否开启射线
                    info.tipCloseRay = true; // 需要提示是否关闭Raycast Target
            }

            for (int i = 0; i < tra.childCount; i++)
            {
                Transform childTra = tra.GetChild(i);
                _Check(childTra, info); //递归子节点
            }
        }
    }

    /// <summary>
    /// 如果预制内有 Slider Toggle 组件，再查找内部
    /// </summary>
    /// <param name="cell"> 所有子节点 </param>
    /// <returns></returns>
    private static bool _IsSliderType(Transform cell)
    {
        List<string> list = new List<string>() {"Slider", "Toggle"};
        foreach (var type in list)
        {
            var r = cell.GetComponent(type);
            if (r != null)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 如果预制内有 Button InputField ScrollRect 组件，再查找内部
    /// </summary>
    /// <param name="cell"> 所有子节点 </param>
    /// <returns></returns>
    private static bool _IsButtonType(Transform cell)
    {
        List<string> list = new List<string>()
        {
            "Button",
            "PressButton",
            "LongPressButton",
            "InputField",
            "ScrollRect"
        };
        foreach (var type in list)
        {
            var r = cell.GetComponent(type);
            if (r != null)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 如果预制内有 Image SKImage SKRawImage RawImage Text SKText 组件，再查找内部
    /// </summary>
    /// <param name="cell"> 所有子节点 </param>
    /// <returns></returns>
    private static bool _IsImaTxtType(Transform cell)
    {
        List<string> list = new List<string>()
        {
            "Image",
            "SKImage",
            "SKRawImage",
            "RawImage",
            "Text",
            "SKText"
        };
        foreach (var type in list)
        {
            var r = cell.GetComponent(type);
            if (r != null)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// 是否打开射线
    /// </summary>
    /// <param name="cell"> 节点 </param>
    /// <returns></returns>
    private static bool _IsOpenUIRay(Transform cell)
    {
        // 实际UI组件中带有射线的只剩 Image、Text、RawImage、SKText、SKImage、SKRawImage
        var types = new List<string>()
        {
            "Image",
            "RawImage",
            "SKImage",
            "SKRawImage",
            "Text",
            "SKText"
        };

        foreach (var value in types)
        {
            var r = cell.GetComponent(value) as Graphic;
            if (r != null && r.raycastTarget == true)
            {
                return  true; //判定需要关闭射线
            }
        }

        return false;
    }

    /// <summary>
    /// 是否打开射线
    /// </summary>
    /// <param name="cell"> 节点 </param>
    /// <returns></returns>
    private static bool _IsCloseUIRay(Transform cell)
    {
        // 实际UI组件中带有射线的只剩 Image、Text、RawImage、SKText、SKImage、SKRawImage
        var types = new List<string>()
        {
            "Image",
            "RawImage",
            "SKImage",
            "SKRawImage",
            "Text",
            "SKText"
        };

        foreach (var value in types)
        {
            var r = cell.GetComponent(value) as Graphic;
            if (r != null && r.raycastTarget == false)
            {
                return  true; //判定需要打开射线
            }
        }

        return false;
    }

    /// <summary>
    /// 设置文件大小
    /// </summary>
    /// <param name="file"> 文件路径 </param>
    /// <param name="info"> 文件的检测信息 </param>
    private static void _SetFileSize(string file, PrefabUIAssetInfo info)
    {
        var fileInfo = new FileInfo(file);
        if (fileInfo != null)
        {
            info.filesize = fileInfo.Length; //文件空间大小
        }
    }
}

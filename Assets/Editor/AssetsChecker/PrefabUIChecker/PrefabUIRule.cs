// @Author: futianfu
// @Date: 2021/9/22 17:03:19


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 预制UI检测规则
/// </summary>
public class PrefabUIRule : AssetRuleBase
{
    private List<string> _res; //缓存判定的结果要输出tips的内容

    private static Transform _prefab;

    public PrefabUIRule()
    {
        _res = new List<string>();
    }

    public override string Description()
    {
        return "组件是Slider或Toggle不做任何处理;" +
               "  组件Button或InputField或ScrollRect,当前节点不处理,递归子节点的Raycast;" +
               "  其他情况都会检查Raycast;";
    }

    public override bool Pass(AssetInfoBase assetInfo)
    {
        if (assetInfo is PrefabUIAssetInfo == false)
        {
            return true;
        }

        PrefabUIAssetInfo info  = assetInfo as PrefabUIAssetInfo; //文件的检测信息
        List<string>      check = _Check(info);                   //缓存判定的结果要输出tips的内容
        if (check.Count > 0)                                      //如果要输出的内容数量大于0，则存在错误
        {
            string str = "";
            tips = string.Join(str, check); //合并字符串
            return false;                   //返回错误
        }

        return  true;
    }

    public override void Fix(AssetInfoBase assetInfo)
    {
        if (assetInfo is PrefabUIAssetInfo == false)
        {
            return ;
        }

        PrefabUIAssetInfo info = assetInfo as PrefabUIAssetInfo; //文件的检测信息

        _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName).transform; //加载预制
        if (_prefab == null)
        {
            return;
        }

        _FixCheck(_prefab, info); //检测哪部分需要修复，然后修复

        info.needCloseRay = false;       //修复状态
        info.tipCloseRay  = false;       //修复状态
        EditorUtility.SetDirty(_prefab); //【保存】覆盖预制状态
        PrefabUtility.SavePrefabAsset(_prefab.gameObject);
        AssetDatabase.Refresh(); //刷新界面
    }

    /// <summary>
    /// 缓存判定的结果要输出tips的内容
    /// </summary>
    /// <param name="assetInfo"> 文件的检测信息 </param>
    /// <returns> 要输出tips的内容 </returns>
    private List<string> _Check(PrefabUIAssetInfo assetInfo)
    {
        _res.Clear();
        if (assetInfo.needCloseRay)
        {
            _res.Add("子节点中Raycast需要关闭;");
        }

        if (assetInfo.tipCloseRay)
        {
            _res.Add("请确认是否关闭Raycast;");
        }

        if (assetInfo.btnOpenRay)
        {
            _res.Add("按钮需要打开Raycast;");
        }

        return  _res;
    }

    /// <summary>
    /// 查找所有节点射线检测情况
    /// </summary>
    /// <param name="prefab"> 预制 </param>
    /// <param name="info"> 文件的检测信息 </param>
    private static void _FixCheck(Transform tra, PrefabUIAssetInfo info)
    {
        if (_IsSliderType(tra)) //组件是Slider || Toggle
        {
            // 不处理，不递归子节点
        }
        else if (_IsButtonType(tra)) //如果预制内有 Button InputField ScrollRect 组件，再查找内部
        {
            //如果有按钮上的图片没打开，就开下Ray
            _OpenBtnRay(tra);

            // 不处理当前节点，但是递归子节点

            for (int i = 0; i < tra.childCount; i++)
            {
                Transform childTra = tra.GetChild(i);
                if (_IsImaTxtType(childTra)) //is image or text 相关类型组件
                {
                    if (_IsOpenUIRay(childTra)) //是否开启射线
                        _CloseRay(childTra);    //关闭射线
                }

                _FixCheck(childTra, info); //递归子节点
            }
        }
        else
        {
            if (_IsImaTxtType(tra)) //is image or text 相关类型组件
            {
                if (_IsOpenUIRay(tra)) //是否开启射线
                    _CloseRay(tra);    //关闭射线
            }

            for (int i = 0; i < tra.childCount; i++)
            {
                Transform childTra = tra.GetChild(i);
                _FixCheck(childTra, info); //递归子节点
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
    /// 关闭UI组件射线
    /// </summary>
    /// <param name="cell"> 节点 </param>
    private static void _CloseRay(Transform cell)
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
            if (r != null)
            {
                r.raycastTarget = false;
            }
        }
    }

    /// <summary>
    /// 打开按钮上UI组件的射线
    /// </summary>
    /// <param name="cell"> 所有子节点 </param>
    /// <returns></returns>
    private static void _OpenBtnRay(Transform cell)
    {
        List<string> types = new List<string>()
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
            if (r != null)
            {
                r.raycastTarget = true;
            }
        }
    }
}

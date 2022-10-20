﻿// @Author: futianfu
// @Date: 2021/8/2 10:45:19


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 图集检测,EditorUI界面
/// </summary>
public class AtlasCheckEditorWindow : EditorWindow
{
    #region 变量

    private Vector2                          _scrollPos;              //滚动条
    private bool                             _isOnlyShowError = true; //单选按钮,只显示问题资源
    private AssetsCheckEditorWindow.Settings _settings;               //配置文件，后期会使用先留着
    private AtlasChecker                     _checker;                //检测逻辑

    #endregion

    #region Unity生命周期

    private void OnEnable()
    {
        titleContent = new GUIContent("合图资源");
    }

    /// <summary>
    /// UI渲染
    /// </summary>
    private void OnGUI()
    {
        if (_checker == null)
        {
            return;
        }

        var atlasList = _checker.GetAssetInfoList(); //AtlasChecker.CollectAssetInfoList()在这里查找要检测的文件夹信息
        if (atlasList == null)
        {
            Debug.Log("model_list is null");
            return;
        }

        _CheckRule();                                 //规则描述文本创建
        _OnlyShow_FixAll(atlasList);                  //只显示错误和修复全部
        _SetTitle();                                  //设置标题名称
        long totalFileSize = _SetFileInfo(atlasList); //设置文件信息
        _BottomBar(atlasList, totalFileSize);         //底部栏
    }

    /// <summary>
    /// 底部栏
    /// </summary>
    /// <param name="atlasList"> 所有文件信息 </param>
    /// <param name="totalFileSize"> 所有文件占用空间 </param>
    private static void _BottomBar(List<AtlasAssetInfo> atlasList, long totalFileSize)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"文件数量：{atlasList.Count} | 总大小：{totalFileSize / 1048576f:n2}MB");
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 设置文件信息
    /// </summary>
    /// <param name="atlasList"> 所有文件的信息 </param>
    /// <returns> 所有文件占用空间 </returns>
    private long _SetFileInfo(List<AtlasAssetInfo> atlasList)
    {
        long totalFileSize = 0;
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true);
        for (int i = 0; i < atlasList.Count; i++)
        {
            var info = atlasList[i];
            totalFileSize += info.filesize;

            bool matchRule = _checker.Check(info); //检测规则,matchRule 表示当前文件夹是否违反规则

            if (matchRule && _isOnlyShowError) //没有违反规则且只显示错误资源
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            if (matchRule == false)
            {
                GUI.color = Color.yellow;
            }

            EditorGUILayout.LabelField(info.foldName.Replace($"Assets/{_settings.assetsRootPath}/", ""));
            if (matchRule == false)
            {
                string checkInfo = _checker.GetCheckInfo(info);
                EditorGUILayout.LabelField(checkInfo);
            }

            GUI.color = Color.white;

            _Fix(matchRule, info); //修复
            _FindRes(info);        //检视

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        return totalFileSize;
    }

    /// <summary>
    /// 只显示错误和修复全部
    /// </summary>
    /// <param name="atlasList"> 检测的文件夹信息 </param>
    private void _OnlyShow_FixAll(List<AtlasAssetInfo> atlasList)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        _OnlyShowErrorRes(); //单选按钮创建,只显示错误资源
        // _FixAll(atlasList);  //修复全部

        GUI.color = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 修复全部
    /// </summary>
    /// <param name="atlasList"> 检测的文件夹信息 </param>
    private void _FixAll(List<AtlasAssetInfo> atlasList)
    {
        GUI.color = Color.green;
        if (GUILayout.Button("全部修复", GUILayout.Width(100)))
        {
            for (int i = 0; i < atlasList.Count; i++)
            {
                var  info      = atlasList[i];
                bool matchRule = _checker.Check(info); //检测规则,matchRule 表示当前文件夹是否违反规则
                if (matchRule && _isOnlyShowError)     //没有违反规则且只显示错误资源
                {
                    continue;
                }

                if (!_checker.CanFix(info)) //CanFix某种规则能否修复，默认能修复
                {
                    continue;
                }

                _checker.Fix(info);
            }
        }
    }

    #endregion

    #region 公有函数

    public void InitConfig(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;                        //配置文件，后期会使用先留着
        _checker  = new AtlasChecker(settings);      //检测逻辑
        _checker.AddRule(new AtlasFormatRule());     //压缩格式检测规范
        _checker.AddRule(new AtlasFilterTypeRule()); //贴图格式检测规范
        _checker.AddRule(new AtlasPackingTagRule()); //合图标签名检测规范
    }

    /// <summary>
    /// ASTC多选弹窗
    /// </summary>
    /// <param name="atlasFormatRule"></param>
    /// <param name="info"></param>
    public void SetASTC(AtlasFormatRule atlasFormatRule, AssetInfoBase info)
    {
        int buttonID = EditorUtility.DisplayDialogComplex("请选择ASTC格式", "请选择ASTC格式", "ASTC_4x4", "ASTC_5x5", "ASTC_6x6");
        switch (buttonID)
        {
            case 0:
                atlasFormatRule.Set_ASTC_4x4(info);
                break;
            case 1:
                atlasFormatRule.Set_ASTC_5x5(info);
                break;
            case 2:
                atlasFormatRule.Set_ASTC_6x6(info);
                break;
            default:
                Debug.Log("Error!");
                break;
        }
    }

    #endregion

    #region 私有函数 /********************** 华丽分割线 **************************/

    /// <summary>
    /// 规则描述文本创建
    /// </summary>
    private void _CheckRule()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("检查规则：");
        var rules = _checker.GetRuleList();
        for (int i = 0; i < rules.Count; i++)
        {
            var rule = rules[i];
            EditorGUILayout.LabelField($"{i + 1}. {rule.Description()}");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 单选按钮创建,只显示错误资源
    /// </summary>
    private void _OnlyShowErrorRes()
    {
        _isOnlyShowError = EditorGUILayout.ToggleLeft("只显示问题资源", _isOnlyShowError);
    }


    /// <summary>
    /// 设置标题名称
    /// </summary>
    private void _SetTitle()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("文件");
        EditorGUILayout.LabelField("问题描述");
        EditorGUILayout.LabelField("操作", GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 修复
    /// </summary>
    /// <param name="matchRule"> 表示当前文件夹是否违反规则 </param>
    /// <param name="info"> 文件夹信息 </param>
    private void _Fix(bool matchRule, AtlasAssetInfo info)
    {
        if (matchRule || !_checker.CanFix(info)) //CanFix某种规则能否修复，默认能修复
        {
            return;
        }

        GUI.color = Color.green;

        if (GUILayout.Button("修复", GUILayout.Width(80)))
        {
            _checker.Fix(info);
        }

        GUI.color = Color.white;
    }

    /// <summary>
    /// 检视
    /// </summary>
    /// <param name="info"> 文件夹信息 </param>
    private void _FindRes(AtlasAssetInfo info)
    {
        if (GUILayout.Button("检视", GUILayout.Width(80)))
        {
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(info.filePath);
        }
    }

    #endregion
}

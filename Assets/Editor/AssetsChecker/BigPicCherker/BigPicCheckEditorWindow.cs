// @Author: futianfu
// @Date: 2021/9/17 10:49:33


using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 大图检测界面
/// </summary>
public class BigPicCheckEditorWindow : EditorWindow
{
    #region 变量

    private BigPicSizeRule                   sizeRule ;  //尺寸规则
    private Vector2                          _scrollPos; //滚动条
    private AssetsCheckEditorWindow.Settings _settings;  //配置文件，后期会使用先留着
    private BigPicChecker                    _checker;   //检测逻辑

    private static int _customSize = 512; //单选按钮,只显示问题资源【static保证关闭后设置保存】

    #endregion

    #region Unity生命周期

    private void OnEnable()
    {
        titleContent = new GUIContent("大图资源");
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
        _OnlyShowTrue();                              //用户自定义尺寸输入创建,只显示满足条件资源
        _SetTitle();                                  //设置标题名称
        long totalFileSize = _SetFileInfo(atlasList); //设置文件信息
        _BottomBar(atlasList, totalFileSize);         //底部栏
    }

    /// <summary>
    /// 设置文件信息
    /// </summary>
    /// <param name="atlasList"> 所有文件的信息 </param>
    /// <returns> 所有文件占用空间 </returns>
    private long _SetFileInfo(List<BigPicAssetInfo> atlasList)
    {
        long totalFileSize = 0;
        _scrollPos = GUILayout.BeginScrollView(_scrollPos, false, true);
        for (int i = 0; i < atlasList.Count; i++)
        {
            var info = atlasList[i];
            totalFileSize += info.filesize;

            bool matchRule = _checker.Check(info); //检测规则,matchRule 表示当前文件夹是否违反规则
            if (matchRule)                         //没有违反规则且只显示错误资源
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();
            if (matchRule == false && !info.isFixed) //没被修复过且检出超过尺寸
            {
                GUI.color = Color.yellow; //文件路径和问题描述变黄
            }

            EditorGUILayout.LabelField(info.foldName.Replace($"Assets/{_settings.assetsRootPath}/", "")); //文件路径

            if (matchRule == false) //检出超过尺寸
            {
                string checkInfo = _checker.GetCheckInfo(info); //获取问题描述内容
                EditorGUILayout.LabelField(checkInfo);          //问题描述
            }

            _Fix(matchRule, info); //修复
            _FindRes(info);        //检视

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        return totalFileSize;
    }

    /// <summary>
    /// 底部栏
    /// </summary>
    /// <param name="atlasList"> 所有文件信息 </param>
    /// <param name="totalFileSize"> 所有文件占用空间 </param>
    private static void _BottomBar(List<BigPicAssetInfo> atlasList, long totalFileSize)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"文件数量：{atlasList.Count} | 总大小：{totalFileSize / 1048576f:n2}MB");
        EditorGUILayout.EndHorizontal();
    }

    #endregion

    /// <summary>
    /// 初始化数据和逻辑
    /// </summary>
    /// <param name="settings"> 本地缓存信息 </param>
    public void InitConfig(AssetsCheckEditorWindow.Settings settings)
    {
        _settings = settings;
        _checker  = new BigPicChecker(_settings);
        sizeRule  = new BigPicSizeRule(_customSize);
        _checker.AddRule(sizeRule);
    }

    /// <summary>
    /// ASTC多选弹窗
    /// </summary>
    /// <param name="picRule"></param>
    /// <param name="info"></param>
    public void SetASTC(BigPicSizeRule picRule, AssetInfoBase info)
    {
        int buttonID = EditorUtility.DisplayDialogComplex("请选择ASTC格式", "请选择ASTC格式", "ASTC_4x4", "ASTC_5x5", "ASTC_6x6");
        switch (buttonID)
        {
            case 0:
                picRule.Set_ASTC_4x4(info);
                break;
            case 1:
                picRule.Set_ASTC_5x5(info);
                break;
            case 2:
                picRule.Set_ASTC_6x6(info);
                break;
            default:
                Debug.Log("Error!");
                break;
        }
    }


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
    /// 用户自定义尺寸输入创建,只显示满足条件资源
    /// </summary>
    private void _OnlyShowTrue()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        _customSize          = EditorGUILayout.IntField("查找尺寸超过范围的大图", _customSize); //用户自定义尺寸输入
        sizeRule._customSize = _customSize;
        GUI.color            = Color.white;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
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
    private void _Fix(bool matchRule, BigPicAssetInfo info)
    {
        GUI.color = Color.white;
        if (matchRule || !_checker.CanFix(info)) //CanFix某种规则能否修复，默认能修复
        {
            return;
        }

        if (info.isFixed)
        {
            GUI.color = Color.white; //被修复过就设置成白色
        }
        else
        {
            GUI.color = Color.green; //没被修复设置成绿色
        }

        if (GUILayout.Button("修复", GUILayout.Width(80)))
        {
            _checker.Fix(info);
        }
    }

    /// <summary>
    /// 检视
    /// </summary>
    /// <param name="info"> 文件夹信息 </param>
    private void _FindRes(BigPicAssetInfo info)
    {
        GUI.color = Color.white; //检视按钮设置成白色

        if (GUILayout.Button("检视", GUILayout.Width(80)))
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture>(info.foldName);
        }
    }
}

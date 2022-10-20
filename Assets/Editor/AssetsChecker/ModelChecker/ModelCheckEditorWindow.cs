// ModelCheckEditorWindow.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ModelCheckEditorWindow : EditorWindow
{
    private Vector2                          scroll_pos;
    private bool                             filter_list = true; // 是否过滤资源
    public  AssetsCheckEditorWindow.Settings settings;
    public  ModelChecker                     checker;   // 当前文件的检测器(检测器上可以挂载规则)
    public  List<ModelAssetInfo>             modelList; // 重新收集信息


    public void InitConfig(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
        checker       = new ModelChecker(settings);

        checker.AddRule("ModelRWRule", new ModelRWRule());
        checker.AddRule("ModelMeshColorRule", new ModelMeshColorRule());
        checker.AddRule("ModelMeshNormalRule", new ModelMeshNormalRule());
        checker.AddRule("ModelMeshTangentRule", new ModelMeshTangentRule());
        checker.AddRule("ModelUV2Rule", new ModelUV2Rule());
        checker.AddRule("ModelBlendShapesRule", new ModelBlendShapesRule());
        checker.AddRule("ModelOptimizeGameObjectRule", new ModelOptimizeGameObjectRule());
        checker.AddRule("ModelAnimCompressionRule", new ModelAnimCompressionRule());
    }

    /// <summary>
    /// 重收集信息
    /// </summary>
    public void GetAssetInfoList()
    {
        modelList = checker.GetAssetInfoList();
    }

    private void OnEnable()
    {
        titleContent = new GUIContent("模型资源");
    }

    private void OnGUI()
    {
        if (checker != null)
        {
            modelList = checker.GetAssetInfoList();
            if (modelList == null)
            {
                Debug.Log("model_list is null");
                return;
            }

            _RuleDescription(); // 规则描述
            _OnlyShow();        // 只显示错误
            _SetTitle();        // 标题栏
            long total_file_size = 0;
            total_file_size = _SetFileInfo(total_file_size); // 设置文件信息
            _BottomBar(total_file_size);                     // 底部栏
        }
    }

    /// <summary>
    /// 只显示错误
    /// </summary>
    private void _OnlyShow()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        filter_list = EditorGUILayout.ToggleLeft("只显示问题资源", filter_list);
        EditorGUILayout.EndHorizontal();
        GUI.color = Color.white;

        EditorGUILayout.Space();
    }


    /// <summary>
    /// 底部栏
    /// </summary>
    /// <param name="total_file_size"></param>
    private void _BottomBar(long total_file_size)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"文件数量：{modelList.Count} | 总大小：{total_file_size / 1048576f:n2}MB");
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 设置文件信息
    /// </summary>
    /// <param name="total_file_size"></param>
    /// <returns></returns>
    private long _SetFileInfo(long total_file_size)
    {
        scroll_pos = GUILayout.BeginScrollView(scroll_pos, false, true);
        for (int i = 0; i < modelList.Count; i++)
        {
            var info = modelList[i];
            total_file_size += info.filesize;

            var match_rule = checker.Check(info);

            if (match_rule && filter_list)
            {
                continue;
            }

            EditorGUILayout.BeginHorizontal();

            if (match_rule == false)
            {
                GUI.color = Color.yellow;
            }

            EditorGUILayout.LabelField(info.foldName.Replace($"Assets/{settings.assetsRootPath}/", ""));
            if (match_rule == false)
            {
                var checkInfo = checker.GetCheckInfo(info);
                EditorGUILayout.LabelField(checkInfo);
            }

            GUI.color = Color.white;

            if (match_rule == false && checker.CanFix(info))
            {
                GUI.color = Color.green;
                if (GUILayout.Button("修复", GUILayout.Width(80)))
                {
                    // checker.Fix(info);

                    _OpenSelectWindow(info); // 打开选择修复 子窗口
                }

                GUI.color = Color.white;
            }

            if (GUILayout.Button("检视", GUILayout.Width(80)))
            {
                Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(info.foldName);
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        return total_file_size;
    }

    /// <summary>
    /// 标题栏
    /// </summary>
    private static void _SetTitle()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文件");
        EditorGUILayout.LabelField("问题描述");
        EditorGUILayout.LabelField("操作", GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 规则描述
    /// </summary>
    private void _RuleDescription()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("检查规则,以下规则需要根据实际情况选择性优化：");
        int                               count          = 0;
        Dictionary<string, AssetRuleBase> assetRuleBases = checker.GetRuleDictionary();
        foreach (var rule in assetRuleBases)
        {
            count += 1;
            EditorGUILayout.LabelField($"{count}.{rule.Value.Description()}");
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 打开选择修复 子窗口
    /// </summary>
    /// <param name="info"></param>
    private static void _OpenSelectWindow(ModelAssetInfo info)
    {
        Vector2 currentMousePosition = Event.current.mousePosition; // 鼠标坐标
        PopupWindow.Show(new Rect(currentMousePosition.x, currentMousePosition.y, 0, 0),
            new SelectFixWindow(info)); // 显示子窗口 弹窗
    }

    [MenuItem("Test/DisplayDialog")]
    public void DisplayDialog(string titleTxt, string message)
    {
       EditorUtility.DisplayDialog(titleTxt, message, "ok");
    }
}

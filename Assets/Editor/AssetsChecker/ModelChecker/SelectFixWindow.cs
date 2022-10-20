using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SelectFixWindow : PopupWindowContent
{
    private Dictionary<string, bool> _keysVules;          // 从外部传入多选名称列表
    private ModelAssetInfo           _info;               // 从外部传入当前要修改文件信息
    private bool                     _filterList = false; // 单选

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="info"> 当前要修复文件的信息 </param>
    public SelectFixWindow(ModelAssetInfo info)
    {
        _keysVules = new Dictionary<string, bool>();
        _info      = info;

        foreach (var ruleName in  _info.ruleAndErrorDescriptions.Values)
        {
            _AddRuleDescription(ruleName); // 添加一个 子窗口要显示的 可选规则描述
        }
    }

    /// <summary>
    /// 开启弹窗时调用
    /// </summary>
    public override void OnOpen()
    {
        Debug.Log("OnOpen");
    }

    /// <summary>
    /// 关闭弹窗时调用
    /// </summary>
    public override void OnClose()
    {
        Debug.Log("OnClose");
    }

    /// <summary>
    /// 设置弹窗尺寸
    /// </summary>
    /// <returns></returns>
    public override Vector2 GetWindowSize()
    {
        //设置弹窗的尺寸
        return new Vector2(200, 300);
    }

    /// <summary>
    /// 绘制弹窗内容
    /// </summary>
    /// <param name="rect"> 弹窗位置 </param>
    public override void OnGUI(Rect rect)
    {
        GUILayout.Label("选择修复:", EditorStyles.boldLabel); // 标题

        // 定义多个单选按钮
        for (int i = 0; i < _keysVules.Count; i++)
        {
            KeyValuePair<string, bool> keyValuePair = _keysVules.ElementAt(i);
            _FixBtn(keyValuePair.Key); // 修复一个
        }

        _FixAllBtn(); // 修复所有按钮
    }

    /// <summary>
    /// 添加一个规则描述,到要显示的单选列表中
    /// </summary>
    private void _AddRuleDescription(string rule)
    {
        _keysVules.Add(rule, false);
    }

    /// <summary>
    /// 点击 单个修复按钮
    /// </summary>
    /// <param name="selectFixTxt"> 按钮上Txt描述 </param>
    private void _FixBtn(string selectFixTxt)
    {
        if (GUILayout.Button(selectFixTxt, GUILayout.Width(194), GUILayout.Height(20)))
        {
            ModelCheckEditorWindow modelWindow =
                EditorWindow.GetWindow<ModelCheckEditorWindow>(true, "Send Event Window");
            _Fix(selectFixTxt, modelWindow);              // 一次修复逻辑
            modelWindow.InitConfig(modelWindow.settings); // 初始化
            modelWindow.GetAssetInfoList();               // 重新收集数据
            modelWindow.Show();                           // 显示父窗口
        }
    }

    /// <summary>
    /// 点击 全部修复按钮
    /// </summary>
    private void _FixAllBtn()
    {
        if (GUILayout.Button("全部修复", GUILayout.Width(194), GUILayout.Height(20)))
        {
            ModelCheckEditorWindow modelWindow =
                EditorWindow.GetWindow<ModelCheckEditorWindow>(true, "Send Event Window");
            _FixAll(modelWindow);                         // 全部修复逻辑
            modelWindow.InitConfig(modelWindow.settings); // 初始化
            modelWindow.GetAssetInfoList();               // 重新收集数据
            modelWindow.Show();                           // 显示父窗口
        }
    }

    /// <summary>
    /// 一次修复逻辑
    /// </summary>
    /// <param name="selectFixTxt"> 选择要修复的问题文本 </param>
    /// <param name="modelWindow"> 父窗口 </param>
    private void _Fix(string selectFixTxt, ModelCheckEditorWindow modelWindow)
    {
        for (var index = 0; index < _info.ruleAndErrorDescriptions.Count; index++)
        {
            string ruleName = _info.ruleAndErrorDescriptions.ElementAt(index).Key;
            var    errorTxt = _info.ruleAndErrorDescriptions.ElementAt(index).Value;
            if (selectFixTxt == errorTxt)
            {
                Dictionary<string, AssetRuleBase>
                    assetRuleBases = modelWindow.checker.GetRuleDictionary(); // 获得规则列表
                var rule = assetRuleBases[ruleName];
                rule.Fix(_info); // 就调用规则中的Fix方式
            }
        }
    }

    /// <summary>
    /// 全部修复逻辑
    /// </summary>
    /// <param name="modelWindow"> 获得窗口对象 </param>
    private void _FixAll(ModelCheckEditorWindow modelWindow)
    {
        for (int i = 0; i < _keysVules.Count; i++)
        {
            string key = _keysVules.ElementAt(i).Key;
            _Fix(key, modelWindow);
        }
    }
}

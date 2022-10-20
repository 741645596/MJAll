// TextureCheckEditorWindow.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/19

using UnityEditor;
using UnityEngine;

public class TextureCheckEditorWindow : EditorWindow
{
    private Vector2                          scroll_pos;
    private bool                             filter_list = true;
    private AssetsCheckEditorWindow.Settings settings;
    private TextureChecker                   checker;

    public void InitConfig(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
        checker       = new TextureChecker(settings);
        checker.AddRule(new TextureRWRule());
        // checker.AddRule(new TextureSizeRule());
        checker.AddRule(new TextureMipmapRule());
        checker.AddRule(new TextureFilterModeRule());
        checker.AddRule(new TextureFormatRule());
    }

    private void OnEnable()
    {
        titleContent = new GUIContent("图片资源");
    }

    private void OnGUI()
    {
        if (checker != null)
        {
            var texture_list = checker.GetAssetInfoList();
            if (texture_list == null)
            {
                Debug.Log("model_list is null");
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("检查规则：");
            var rules = checker.GetRuleList();
            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];
                EditorGUILayout.LabelField($"{i + 1}. {rule.Description()}");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            filter_list = EditorGUILayout.ToggleLeft("只显示问题资源", filter_list);
            GUI.color   = Color.green;
            if (GUILayout.Button("全部修复", GUILayout.Width(100)))
            {
                checker.FixAll();
            }

            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("文件");
            EditorGUILayout.LabelField("问题描述");
            EditorGUILayout.LabelField("操作", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            long total_file_size = 0;
            scroll_pos = GUILayout.BeginScrollView(scroll_pos, false, true);
            for (int i = 0; i < texture_list.Count; i++)
            {
                var info = texture_list[i];
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
                        checker.Fix(info);
                        texture_list = checker.GetAssetInfoList();
                    }

                    GUI.color = Color.white;
                }

                if (GUILayout.Button("检视", GUILayout.Width(80)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture>(info.foldName);
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField($"文件数量：{texture_list.Count} | 总大小：{total_file_size / 1048576f:n2}MB");
            EditorGUILayout.EndHorizontal();
        }
    }
}

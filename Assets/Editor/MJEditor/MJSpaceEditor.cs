using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace MJEditor
{
    public class MJSpaceEditor : EditorWindow
    {

        private MJSpaceConfigSO space_config;
        private SerializedObject serializedObject;
        private SerializedProperty sp_wallConfig;
        private SerializedProperty sp_deskConfig;
        private SerializedProperty sp_meldConfig;
        private SerializedProperty sp_handConfig;
        private SerializedProperty sp_winSetConfig;

        private MJSpaceMockSO mock_data;
        private SerializedObject mock_serializedObject;


        private Vector2 scroll_pos;

        [MenuItem("Tools/MJ/SpcaeEditor", false)]
        public static void ShowSpcaeEditor()
        {
            var window = GetWindow<MJSpaceEditor>();
            window.titleContent = new GUIContent("MJ Space Editor");
            window.Show();
        }

        private void OnEnable()
        {
            space_config = AssetDatabase.LoadAssetAtPath<MJSpaceConfigSO>("Assets/Editor/MJEditor/MJSpaceConfig.asset");
            serializedObject = new SerializedObject(space_config);
            sp_wallConfig = serializedObject.FindProperty("wallConfig");
            sp_deskConfig = serializedObject.FindProperty("deskConfig");
            sp_meldConfig = serializedObject.FindProperty("meldConfig");
            sp_handConfig = serializedObject.FindProperty("handConfig");
            sp_winSetConfig = serializedObject.FindProperty("winSetConfig");

            mock_data = AssetDatabase.LoadAssetAtPath<MJSpaceMockSO>("Assets/Editor/MJEditor/MJSpaceMockData.asset");
            mock_serializedObject = new SerializedObject(mock_data);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新", GUILayout.Width(100), GUILayout.Height(40)))
            {
                var root = GameObject.Find("MJSpace");
                if(root != null)
                {
                    GameObject.DestroyImmediate(root);
                }

                var appdomain = AppDomainManager.AppDomain;
                var it = appdomain.LoadedTypes["MJCommon.MJSpace"];
                var type = it.ReflectionType;
                var ctor = type.GetConstructor(new System.Type[1] { typeof(string) });

                var json = JsonMapper.ToJson(space_config);
                Debug.Log(json);

                var instance = ctor.Invoke(new object[] { json });

                for (int i = 0; i < mock_data.deskCards.Count; i++)
                {
                    var token = mock_data.deskCards[i].Split(',');
                    var cards = new List<int>();
                    for (int j = 0; j < token.Length; j++)
                    {
                        cards.Add(int.Parse(token[j]));
                    }
                    type.GetMethod("ResumeOutCard", new Type[] { typeof(int), typeof(int[]) })
                        .Invoke(instance, new object[] { i, cards.ToArray() });
                }

                for (int i = 0; i < mock_data.handCards.Count; i++)
                {
                    var token = mock_data.handCards[i].Split(',');
                    var cards = new List<int>();
                    for (int j = 0; j < token.Length; j++)
                    {
                        cards.Add(int.Parse(token[j]));
                    }
                    type.GetMethod("ResumeHandCard", new Type[] { typeof(int), typeof(int[]) })
                        .Invoke(instance, new object[] { i, cards.ToArray() });
                }

                for (int i = 0; i < mock_data.winCards.Count; i++)
                {
                    var token = mock_data.winCards[i].Split(',');
                    var cards = new List<int>();
                    for (int j = 0; j < token.Length; j++)
                    {
                        cards.Add(int.Parse(token[j]));
                    }
                    type.GetMethod("ResumeWinCard", new Type[] {typeof(int), typeof(int[])}).Invoke(instance, new object[] {i, cards.ToArray()});
                }

                ResumeMeld(mock_data.meldCards0, type, instance, 0);
                ResumeMeld(mock_data.meldCards1, type, instance, 1);
                ResumeMeld(mock_data.meldCards2, type, instance, 2); 
                ResumeMeld(mock_data.meldCards3, type, instance, 3);

                var wallConfigJson = JsonMapper.ToJson(space_config.wallConfig);
                type.GetMethod("ResumeWallCard", new Type[] { typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) })
                    .Invoke(instance, new object[] { wallConfigJson, 0, mock_data.dice1, mock_data.dice2, mock_data.takeWallCount, mock_data.takeWallCountBack });
            }

            if (GUILayout.Button("导出Json配置", GUILayout.Width(100), GUILayout.Height(40)))
            {
                var path = Path.Combine(Application.dataPath, "GameAssets");
                var file = EditorUtility.SaveFilePanel("导出Json配置", path, "space", "json");
                if(string.IsNullOrEmpty(file))
                {
                    return;
                }
                var json = JsonMapper.ToJson(space_config);
                File.WriteAllText(file, json);
            }

            if (GUILayout.Button("读取Json配置", GUILayout.Width(100), GUILayout.Height(40)))
            {
                var path = Path.Combine(Application.dataPath, "GameAssets");
                var file_path = EditorUtility.OpenFilePanel("导入Json配置", path, "json");
                if (string.IsNullOrEmpty(file_path))
                {
                    return;
                }
                var json = File.ReadAllText(file_path);
                Debug.Log(json);
                space_config.LoadFromJson(json);
            }

            EditorGUILayout.EndHorizontal();

            scroll_pos = GUILayout.BeginScrollView(scroll_pos, false, true);
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.PropertyField(sp_wallConfig, new GUIContent("牌墙位置配置"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.PropertyField(sp_deskConfig, new GUIContent("桌面牌位置配置"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.PropertyField(sp_meldConfig, new GUIContent("副子牌位置配置"));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.PropertyField(sp_handConfig, new GUIContent("手牌位置配置"));
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.PropertyField(sp_winSetConfig, new GUIContent("胡牌位置配置"));
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();

            mock_serializedObject.Update();
            GUILayout.Space(40);
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("模拟数据：");
            SerializedProperty iterator = mock_serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
            mock_serializedObject.ApplyModifiedProperties();

            GUILayout.EndScrollView();
        }


        private void ResumeMeld(List<string> meld_cards, Type type, object instance, int o)
        {
            for (int i = 0; i < meld_cards.Count; i++)
            {
                var token = meld_cards[i].Split(',');
                var cards = new List<int>();
                for (int j = 0; j < token.Length; j++)
                {
                    cards.Add(int.Parse(token[j]));
                }
                type.GetMethod("ResumeMeldCard", new Type[] { typeof(int), typeof(int[]) })
                    .Invoke(instance, new object[] { o, cards.ToArray() });
            }
        }
    }
}
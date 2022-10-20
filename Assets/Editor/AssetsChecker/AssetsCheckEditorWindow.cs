// AssetsCheckEditorWindow.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/08

using System.Collections.Generic;
using System.IO;
using System.Net;
using DG.Tweening.Plugins.Core.PathCore;
using Unity.Utility;
using UnityEditor;
using UnityEngine;
using WLCore.Helper;
using Path = System.IO.Path;

public partial class AssetsCheckEditorWindow : EditorWindow
{
    private        Settings _settings;                   //配置信息
    private static string   _path = "Assets/GameAssets"; //路径

    /// <summary>
    /// 配置信息
    /// </summary>
    public  struct Settings
    {
        public string assetsRootPath ; //需要检测的资源根目录
        public string audioType      ; //需要检测的音频后缀
        public string modelType      ; //需要检测的模型后缀
        public string textureType    ; //需要检测的贴图后缀
    }

    [MenuItem("Tools/资源检查", false)]
    private static void DoIt()
    {
        var window = GetWindow<AssetsCheckEditorWindow>();
        window.titleContent = new GUIContent("资源检查");
        window.minSize      = new Vector2(1000, 800);
        window.Show();
    }

    private void OnEnable()
    {
        _settings = new Settings
        {
            assetsRootPath = "Assets/GameAssets",
            audioType      = "map3",
            modelType      = "fbx,FBX",
            textureType    = "png,jpg,bmp,tif,gif,tga,svg,psd,cdr,eps,ai"
        };
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        _path = EditorGUILayout.TextField("检测路径", _path);
        if (_path.Length > 7)
            _settings.assetsRootPath = _path.Substring(7);
        EditorGUILayout.Space();

        if (GUILayout.Button("音频资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var audioTab = GetWindow<AudioCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            audioTab.InitConfig(_settings);
            audioTab.Show();
        }

        if (GUILayout.Button("模型资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var modelTab = GetWindow<ModelCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            modelTab.InitConfig(_settings);
            modelTab.Show();
        }

        if (GUILayout.Button("图片资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var textureTab = GetWindow<TextureCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            textureTab.InitConfig(_settings);
            textureTab.Show();
        }

        if (GUILayout.Button("材质资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var materialTab = GetWindow<MaterialCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            materialTab.InitConfig(_settings);
            materialTab.Show();
        }

        if (GUILayout.Button("合图资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var atlasTab = GetWindow<AtlasCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            atlasTab.InitConfig(_settings);
            atlasTab.Show();
        }

        if (GUILayout.Button("大图资源", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var bigPicTab = GetWindow<BigPicCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            bigPicTab.InitConfig(_settings);
            bigPicTab.Show();
        }

        if (GUILayout.Button("预制网格", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var prefabMeshTab = GetWindow<PrefabMeshCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            prefabMeshTab.InitConfig(_settings);
            prefabMeshTab.Show();
        }


        if (GUILayout.Button("预制蒙皮网格", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var prefabMeshTab = GetWindow<PrefabSkinnedMeshCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            prefabMeshTab.InitConfig(_settings);
            prefabMeshTab.Show();
        }

        if (GUILayout.Button("预制粒子", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var prefabParticleTab = GetWindow<PrefabParticleCheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            prefabParticleTab.InitConfig(_settings);
            prefabParticleTab.Show();
        }

        if (GUILayout.Button("预制UI", GUILayout.Width(100), GUILayout.Height(40)))
        {
            var prefabUITab = GetWindow<PrefabUICheckEditorWindow>(typeof(AssetsCheckEditorWindow));
            prefabUITab.InitConfig(_settings);
            prefabUITab.Show();
        }

        EditorGUILayout.EndVertical();
    }
}

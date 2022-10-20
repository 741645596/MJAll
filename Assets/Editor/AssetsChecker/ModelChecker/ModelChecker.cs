// ModelChecker.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/07/09

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ModelChecker : AssetsCheckerBaseNew<ModelAssetInfo>
{
    private readonly string _rwRuleName                 = "ModelRWRule";                 //可读报错描述
    private readonly string _meshColorRuleName          = "ModelMeshColorRule";          //可读报错描述
    private readonly string _meshNormalRuleName         = "ModelMeshNormalRule";         //导入法线报错描述
    private readonly string _meshTangentRuleName        = "ModelMeshTangentRule";        //导入切线报错描述
    private readonly string _uv2RuleName                = "ModelUV2Rule";                //导入UV2报错描述
    private readonly string _blendShapesRuleName        = "ModelBlendShapesRule";        //导入混合形状报错描述
    private readonly string _optimizeGameObjectRuleName = "ModelOptimizeGameObjectRule"; //优化游戏对象报错描述
    private readonly string _animCompressionRuleName    = "ModelAnimCompressionRule";    //优化动画报错描述

    private readonly string _readableErrorTxt               = "未关闭R&W";          //可读报错描述
    private readonly string _meshColorErrorTxt              = "网格包含Colors";      //可读报错描述
    private readonly string _importNormalsErrorTxt          = "网格包含Normals";     //导入法线报错描述
    private readonly string _importTangentsErrorTxt         = "网格包含Tangents";    //导入切线报错描述
    private readonly string _importUV2ErrorTxt              = "网格包含UV2";         //导入UV2报错描述
    private readonly string _importBlendShapesErrorTxt      = "网格包含BlendShapes"; //导入混合形状报错描述
    private readonly string _optimizeGameObjectsErrorTxt    = "未优化游戏对象";         //优化游戏对象报错描述
    private readonly string _animCompressionOptimalErrorTxt = "未优化动画";           //优化动画报错描述

    private AssetsCheckEditorWindow.Settings settings;

    public ModelChecker(AssetsCheckEditorWindow.Settings settings)
    {
        this.settings = settings;
    }

    public override List<ModelAssetInfo> CollectAssetInfoList()
    {
        var assets_root = Path.Combine(Application.dataPath, settings.assetsRootPath);
        var model_types = settings.modelType.Split(',');

        var list = new List<ModelAssetInfo>();

        for (int i = 0; i < model_types.Length; i++)
        {
            var      pattern = $"*.{model_types[i]}";
            string[] files   = Directory.GetFiles(assets_root, pattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                var info = new ModelAssetInfo();
                info.ruleAndErrorDescriptions = new Dictionary<string, string>();
                WLDebug.LogError($"初始化,个数:{info.ruleAndErrorDescriptions.Count}");

                var f = file.Replace(Application.dataPath, "Assets");

                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(f);
                // if (obj.name!="UV_2")
                //     continue;

                // Color
                if (obj != null)
                {
                    var mf = obj.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        info.meshColorCount = mf.sharedMesh.colors.Length;
                        if (info.meshColorCount > 0)
                        {
                            info.ruleAndErrorDescriptions.Add(_meshColorRuleName, _meshColorErrorTxt);
                        }
                    }
                }

                var importer = AssetImporter.GetAtPath(f) as ModelImporter;

                if (importer == null)
                {
                    continue;
                }

                if (fileInfo != null)
                {
                    info.filesize = fileInfo.Length;
                }

                info.foldName = f;

                // Read
                info.isReadable = importer.isReadable;
                if (info.isReadable)
                {
                    if (!info.ruleAndErrorDescriptions.ContainsKey(_rwRuleName))
                        info.ruleAndErrorDescriptions.Add(_rwRuleName, _readableErrorTxt);
                }

                // Normals
                info.isImportNormals = importer.importNormals;
                if (info.isImportNormals != ModelImporterNormals.None)
                {
                    if (!info.ruleAndErrorDescriptions.ContainsKey(_meshNormalRuleName))
                        info.ruleAndErrorDescriptions.Add(_meshNormalRuleName, _importNormalsErrorTxt);
                    WLDebug.LogError($"添加:{_meshNormalRuleName}");
                }

                // Tangents
                info.isImportTangents = importer.importTangents;
                if (info.isImportTangents != ModelImporterTangents.None)
                {
                    info.ruleAndErrorDescriptions.Add(_meshTangentRuleName, _importTangentsErrorTxt);
                }

                // UV2 MeshFilter Mesh.uv2
                info.isImportUV2 = false;
                if (obj != null)
                {
                    var mf = obj.GetComponent<MeshFilter>();
                    if (mf != null && mf.sharedMesh != null)
                    {
                        var       sharedMesh      = mf.sharedMesh;
                        Vector2[] sharedMeshUV2   = sharedMesh.uv2;
                        bool      infoIsImportUV2 = sharedMeshUV2.Length != 0;
                        info.isImportUV2 = infoIsImportUV2;
                        if (info.isImportUV2)
                        {
                            info.ruleAndErrorDescriptions.Add(_uv2RuleName, _importUV2ErrorTxt);
                        }
                    }
                }

                // BlendShapes
                bool infoIsImportBlendShapes = importer.importBlendShapes || importer.importVisibility ||
                                               importer.importCameras || importer.importLights;
                info.isImportBlendShapes = infoIsImportBlendShapes;
                if (info.isImportBlendShapes)
                {
                    info.ruleAndErrorDescriptions.Add(_blendShapesRuleName, _importBlendShapesErrorTxt);
                }

                // Optimize Game Object
                if (importer.avatarSetup == ModelImporterAvatarSetup.CreateFromThisModel &&
                    !importer.optimizeGameObjects)
                {
                    info.ruleAndErrorDescriptions.Add(_optimizeGameObjectRuleName, _optimizeGameObjectsErrorTxt);
                    info.isOptimizeGameObjects = true;
                }
                else
                {
                    info.isOptimizeGameObjects = false;
                }

                // anim.Compression
                info.isAnimCompressionOptimal =
                    importer.animationCompression != ModelImporterAnimationCompression.Optimal;
                if (info.isAnimCompressionOptimal)
                {
                    info.ruleAndErrorDescriptions.Add(_animCompressionRuleName, _animCompressionOptimalErrorTxt);
                }

                list.Add(info);
            }
        }

        list = list.OrderBy((info) => { return info.foldName; }).ToList();
        return list;
    }
}

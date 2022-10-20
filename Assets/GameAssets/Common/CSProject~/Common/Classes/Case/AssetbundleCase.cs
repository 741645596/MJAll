// @Author: futianfu
// @Date: 2021/8/9 19:04:10


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using Unity.Core;


namespace Common
{
    public class AssetbundleCase : BaseCaseStage
    {
        /// <summary>
        /// 方块
        /// </summary>
        private GameObject _game;
        /// <summary>
        /// 方块
        /// </summary>
        private GameObject _game2;

        public Action<Texture2D> action;
        public Action<UnityEngine.Object[]> action2;

        /// <summary>
        /// 初始化界面
        /// </summary>
        public override void OnInitialize()
        {
            //AssetsManager.SetLoadType(AssetsManager.LoadType.AssetBundle);

            action = new Action<Texture2D>(LoadAsyncCallBack);
            action2 = new Action<UnityEngine.Object[]>(LoadAllAsyncCallBack);

            base.OnInitialize();
        }

        /// <summary>
        /// 创建预制测试
        /// </summary>
        public void CaseCCPrefab()
        {
            _game = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _game2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _game.name = "Cube";
            _game2.name = "Cube2";
            _game.transform.position = new Vector3(27f, 6, 0);
            _game2.transform.position = new Vector3(23f, 6f, 0);
        }

        /// <summary>
        /// Load<T> 泛型加载测试
        /// </summary>
        public void CaseLoadAB()
        {
            WLDebug.Log("=====================");
            Texture2D texture = AssetsManager.Load<Texture2D>("Common/Module1/demo_atlas", "bind_icon.png");
            Texture2D texture2 = AssetsManager.Load<Texture2D>("Common/Module2/test_images", "choose_bg.png");
            _game.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", texture);
            _game2.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", texture2);
            WLDebug.Log("Click Button 1");
        }

        /// <summary>
        /// Load 加载测试
        /// </summary>
        public void CaseLoad()
        {
            
            Texture2D texture = AssetsManager.Load("MJCommon/MJ/mj_env", "textures/background_01.png") as Texture2D;
            _game2.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", texture);
        }

        /// <summary>
        /// LoadAsync<T> 泛型异步加载测试
        /// </summary>
        public void CaseLoadAsyncT()
        {
            AssetsManager.LoadAsync<Texture2D>("MJCommon/MJ/mj_env", "textures/background_01a.png", action);
        }

        /// <summary>
        /// LoadAllAsync 异步加载AssetBundle中所有的Assets资源
        /// </summary>
        public void CaseLoadAllAsync()
        {
            //AssetsManager.LoadAllAsync("Common/Module1/prefabs", action2);
        }

        /// <summary>
        /// 跨模块依赖测试 【引用预制】
        /// </summary>
        public void CaseDifModuleDepPrefabAB()
        {
            WLDebug.Log("=====================");

        }

        #region 私有函数 /*********************************** 华丽分割线 ***********************************/

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="t"></param>
        private void LoadAsyncCallBack(Texture2D t)
        {
            WLDebug.Log("贴图名称", t.name);
            _game.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", t);
            WLDebug.Log("加载完毕");
        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <param name="obj"></param>
        private void LoadAllAsyncCallBack(UnityEngine.Object[] obj)
        {
            foreach (var item in obj)
            {
                WLDebug.Log("对象名称", item.name);
            }
            WLDebug.Log("异步加载了当前Ab包的所有资源");
        }

        #endregion

    }
}

// BaseStage.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/26

using System;
using Unity.Core;
using UnityEngine.SceneManagement;

namespace WLCore.Stage
{
    /// <summary>
    /// BaseStage 子游戏/模块的基类
    /// 定义了子游戏基础的生命周期事件
    /// 由StageManager管理
    /// </summary>
    public class BaseStage
    {
        // 切换Scene成功后，强制卸载当前所有资源，包括正在使用的然后在加载新资源
        public bool forceUnloadRes = true;

        protected string stageName;
        private Action _onSceneLoad;

        /// <summary>
        /// 初始化事件
        /// </summary>
        public virtual void OnInitialize()
        {

        }

        /// <summary>
        /// 游戏循环
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 游戏关闭
        /// </summary>
        public virtual void OnShutdown()
        {

        }

        /// <summary>
        /// 切换至前台事件
        /// </summary>
        public virtual void OnForeground()
        {

        }

        /// <summary>
        /// 切换到后台事件
        /// </summary>
        public virtual void OnBackground()
        {

        }

        public virtual void OnApplicationFocus(bool bFocus)
        {

        }

        /// <summary>
        /// 保存StageName方便反射使用
        /// </summary>
        /// <param name="name"></param>
        public void SetStageName(string name)
        {
            stageName = name;
        }

        public string GetStageName()
        {
            return stageName;
        }

        /// <summary>
        /// 加载unity scenne，重写OnSceneDidLoad回调
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName, Action onload = null)
        {
            _onSceneLoad = onload;
            SceneManager.sceneLoaded += _HandleSceneLoaded;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        private void _HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.sceneLoaded -= _HandleSceneLoaded;

            Unity.Widget.WDirector.Init();

            // 强制卸载，备注：Scene不能有初始化纹理贴图否则也会被卸载
            AssetsManager.UnloadAll(forceUnloadRes);

            System.GC.Collect();

            _onSceneLoad?.Invoke();
        }
    }
}

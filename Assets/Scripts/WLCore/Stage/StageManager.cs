// StageManager.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/26

namespace WLCore.Stage
{
    /// <summary>
    /// StageManager
    /// Stage管理类，负责管理Stage及其生命周期
    /// </summary>
    public static class StageManager
    {
        private static BaseStage _persistentStage; 
        private static BaseStage _runStage;         // 当前运行的stage

        /// <summary>
        /// 初始化数据，省得还得防御性判断
        /// </summary>
        public static void Init()
        {
            _runStage = new BaseStage();
        }

        /// <summary>
        /// 启动持久化stage
        /// </summary>
        /// <param name="name"></param>
        public static void RunPersistentStage(string name)
        {
            if (_persistentStage != null)
            {
                throw new System.Exception("StageManager.RunPersistentStage: 持久化Stage已存在");
            }

            var stage = AppDomainManager.Instantiate<BaseStage>(name);
            if (stage != null)
            {
                stage.SetStageName(name);
                stage.OnInitialize();
                _persistentStage = stage;
            }
        }

        /// <summary>
        /// 运行Stage，会清理所有已存在的Stage
        /// </summary>
        /// <param name="name"></param>
        public static void RunStage(string name)
        {
            _runStage.OnShutdown();

            WLDebug.Log("开始加载Stage name = ", name);
            var stage = AppDomainManager.Instantiate<BaseStage>(name);
            if (stage != null)
            {
                stage.SetStageName(name);
                stage.OnInitialize();
                _runStage = stage;
            }
        }

        /// <summary>
        /// 同上，支持构造参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static BaseStage RunStage(string name, params object[] args)
        {
            _runStage.OnShutdown();

            WLDebug.Log("开始加载Stage name = ", name);
            var stage = AppDomainManager.Instantiate<BaseStage>(name, args);
            if (stage != null)
            {
                stage.SetStageName(name);
                stage.OnInitialize();
                _runStage = stage;
                return _runStage;
            }
            return null;
        }

        public static void RunStage(BaseStage stage)
        {
            _runStage.OnShutdown();

            _runStage = stage;
            _runStage.OnInitialize();
        }

        /// <summary>
        /// 获取RunStage，该接口性能会有些损失，如果是在update内调用的请在游戏里自己保存一份
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetRunStage<T>() where T : BaseStage
        {
            if(_runStage is T)
            {
                return _runStage as T;
            }
            return null;
        }

        /// <summary>
        /// 游戏循环
        /// </summary>
        /// <param name="deltaTime"></param>
        public static void Update(float deltaTime)
        {
            _persistentStage?.OnUpdate(deltaTime);
            _runStage.OnUpdate(deltaTime);
        }

        /// <summary>
        /// 清理所有Stage
        /// </summary>
        public static void Clean()
        {
            _runStage.OnShutdown();
            _runStage = new BaseStage();

            _persistentStage?.OnShutdown();
            _persistentStage = null;
        }

        /// <summary>
        /// 游戏前后台触发接口
        /// </summary>
        /// <param name="bPause"></param>
        public static void OnApplicationPause(bool bPause)
        {
            if (bPause)
            {
                _persistentStage?.OnBackground();
                _runStage.OnBackground();
            }
            else
            {
                _persistentStage?.OnForeground();
                _runStage.OnForeground();
            }
        }

        /// <summary>
        /// 焦点失焦回调，预先保留，预防以后可能用到
        /// </summary>
        /// <param name="bFocus"></param>
        public static void OnApplicationFocus(bool bFocus)
        {
            _persistentStage?.OnApplicationFocus(bFocus);
            _runStage.OnApplicationFocus(bFocus);
        }
    }
}

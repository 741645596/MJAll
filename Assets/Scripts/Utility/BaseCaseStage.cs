// BaseTestStage.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/29

using System.Collections.Generic;
using ILRuntime.CLR.TypeSystem;
using UnityEngine;
using WLCore.Stage;
using UnityEngine.UI;
using Unity.Widget;

namespace Unity.Core
{
    /// <summary>
    /// 用于测试的基类，方便根据函数名，自动生成按钮
    /// 所有测试函数方法以 - Case开头
    /// </summary>
    public class BaseCaseStage : BaseStage
    {
        private List<string> _objectNames;
        protected WNode root;
        public BaseCaseStage()
        {
            
        }

        /// <summary>
        /// Stage 初始化
        /// </summary>
        public override void OnInitialize()
        {
            base.OnInitialize();
            WLDebug.Log("BaseCaseStage OnInitialize");

            // 记录初始Objects的名称
            _RecordObjectNames();

            // 初始化cocos框架
            WDirector.Init();
            root = WDirector.GetRootLayer();

            // 显示测试按钮
            CaseGUI.Instance.caseNames = _GetPrefixCaseFuns();
            CaseGUI.Instance.caseStage = this;
            CaseGUI.Instance.stageName = stageName;
        }

        /// <summary>
        /// 游戏循环
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
        }

        /// <summary>
        /// Stage被其他Stage替换
        /// </summary>
        public override void OnShutdown()
        {
            base.OnShutdown();

            // 删除非原始对象
            var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var o in objs)
            {
                if (_objectNames.Contains(o.name) == false)
                {
                    GameObject.Destroy(o);
                }
            }
        }

        private void _RecordObjectNames()
        {
            _objectNames = new List<string>();
            var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var o in objs)
            {
                _objectNames.Add(o.name);
            }
        }

        // 获取前缀包含Case的函数
        private List<string> _GetPrefixCaseFuns()
        {
            var cases = new List<string>();

            // 主工程热更需要使用以下反射方式，不能直接GetType()
            IType itype = AppDomainManager.AppDomain.LoadedTypes[stageName];
            var type = itype.ReflectionType;
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public);
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (method.Name.StartsWith("Case")
                    && method.GetParameters().Length == 0)
                {
                    cases.Add(method.Name);
                }
            }
            return cases;
        }

    }
}

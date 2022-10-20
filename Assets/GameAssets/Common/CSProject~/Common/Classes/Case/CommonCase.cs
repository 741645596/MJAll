// CommonTest.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/7/29
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;
using Unity.Core;

namespace Common
{
    /// <summary>
    /// Case文件夹专门用来编写测试用例，请在该文件夹存放测试用例文件
    /// 双击Unity工程的Scens/CommonSceneCase 即可进入该模块
    /// Test是苹果的敏感关键字，测试单词统一使用Case
    /// </summary>
    public class CommonCase : BaseCaseStage
    {
        /// <summary>
        /// 初始化界面
        /// </summary>
        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            base.OnInitialize();

        }

        private void _CreateAnimatorController2()
        {
            //var txt1 = p.GetComponent<Text>("Text");
            //var ani = AssetsManager.Load<RuntimeAnimatorController>("Common/Module1/prefabs", "Button1.controller");
            //txt1.gameObject.AddComponent<Animator>().runtimeAnimatorController = ani;
        }

        public void CaseSpineAssetBundle()
        {
            //AssetsManager.SetLoadType(AssetsManager.LoadType.Local);

            //var sp = WSpineNode.Create("Common/Module1/spineboy-unity", "spineboy_Material.mat", "spineboy-unity_SkeletonData.asset");
            //sp.AddTo(root)
            //    .SetAnchor(layout.center)
            //    .Layout(layout.center);
            //sp.SetAnimation(0, "run", true);

            var obj = SpineHelper.Create("Common/Module1/spineboy-unity", "spineboy_Material.mat", "spineboy-unity_SkeletonData.asset")
                .AddTo(root.gameObject)
                .Layout(layout.center);
            SpineHelper.SetAnimation(obj, "run", true);
            WLDebug.LogWarning("=========================== ");
        }

        public void CaseSequenceAnimation()
        {
           
        }
    }
}

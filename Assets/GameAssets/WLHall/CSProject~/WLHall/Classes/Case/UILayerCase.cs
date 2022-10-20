// CommonTest.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/7/29
using System.Diagnostics;
using Unity.Widget;
using Unity.Core;
using UnityEngine;
using WLCommon;
using WLCore.Entity;
using WLCore.Stage;

namespace WLHall
{
    
    /// <summary>
    /// Case文件夹专门用来编写测试用例，请在该文件夹存放测试用例文件
    /// 双击Unity工程的Scens/WLHallSceneCase 即可进入该模块
    /// Test是苹果的敏感关键字，测试单词统一使用Case
    /// </summary>
    public class UILayerCase : BaseCaseStage
    {
        /// <summary>
        /// 初始化界面
        /// </summary>
        public override void OnInitialize()
        {
            AssetsManager.SetLoadType(AssetsManager.LoadType.Local);
            base.OnInitialize();

            CaseUILayer();
        }

        // 测试函数以Case开头
        public void CaseUILayer()
        {
    
        }


    }
}

// CommonTest.cs
// Author: shihongyang shihongyang@Unity.com
// Date: 2021/7/29
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Core;
using UnityEngine.SceneManagement;

namespace WLHall
{
    /// <summary>
    /// Case文件夹专门用来编写测试用例，请在该文件夹存放测试用例文件
    /// 双击Unity工程的Scens/WLHallSceneCase 即可进入该模块
    /// Test是苹果的敏感关键字，测试单词统一使用Case
    /// </summary>
    public class WLHallCase : BaseCaseStage
    {
        /// <summary>
        /// 初始化界面
        /// </summary>
        public override void OnInitialize()
        {
            // AssetsManager.SetLoadType(AssetsManager.LoadType.Local);
            base.OnInitialize();
        }

        // 测试函数以Case开头
        public void CaseAddStage()
        {
            var dlls = new List<string>() { "Common" };
            LoadDll.Load(dlls, (res)=>
            {
                WLDebug.Log("Load Dll res = ", res);

                SceneManager.LoadSceneAsync("SampleScene");
                //StageManager.AddStage("Common.CommonCase");
            }, true);
        }

        private Character character;
        public void CaseCharacter()
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            character = new Character();
            character.Equip(1, "WLHall/Dress", "Girl_Body.prefab");
            character.Equip(2, "WLHall/Dress", "Girl_Leg.prefab");
            character.Equip(3, "WLHall/Case", "Girl_Suit02_Hair/Girl_Suit02_Hair.prefab");
            character.Equip(4, "WLHall/Case", "Girl_Suit02_Head/Girl_Suit02_Head.prefab");
            character.Equip(5, "WLHall/Case", "Girl_Suit02_Shoes/Girl_Suit02_Shoes.prefab");

            w.Stop();
            WLDebug.Log($"CaseCharacter {w.ElapsedMilliseconds}");
        }

        public void CaseCharacter2()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            character.Equip(1, "WLHall/Dress",  "Girl_Body.prefab");
            w.Stop();

            WLDebug.Log($"CaseCharacter2 {w.ElapsedMilliseconds}");
        }

        public void CaseCharacter3()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            character.Equip(1, "WLHall/Dress", "Girl_Suit02_UpCloth.prefab");
            w.Stop();

            WLDebug.Log($"CaseCharacter3 {w.ElapsedMilliseconds}");
        }

    }
}

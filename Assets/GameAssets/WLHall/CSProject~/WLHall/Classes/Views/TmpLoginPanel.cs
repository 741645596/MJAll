// @Author: tanjinhua
// @Date: 2021/8/18  14:42


using System;
using Unity.Widget;
using UnityEngine;
using UnityEngine.UI;
using WLCore.Helper;
using WLCore.Stage;

namespace WLHall
{
    /// <summary>
    /// 临时测试用的账号登录界面
    /// </summary>
    public class TmpLoginPanel : WNode
    {
        //public Action<string, string> onClickLogin;

        private InputField _accountField;
        private InputField _passwordField;

        public TmpLoginPanel(string accnout = null, string password = null)
        {
            InitGameObject("WLHall/Main/hall_launch", "prefabs/login_panel.prefab");

            _accountField = FindReference<InputField>("Account");
            _passwordField = FindReference<InputField>("Password");

            _accountField.text = accnout;
            _passwordField.text = password;

            FindReference<Button>("Button").onClick.AddListener(() =>
            {
                var stage = StageManager.GetRunStage<LaunchStage>();
                stage.OnClickLogin(_accountField.text, _passwordField.text);
            });
        }
    }
}

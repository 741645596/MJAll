using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Widget;


namespace WLHall
{
    /// <summary>
    /// 账号登录/微信登录等按钮
    /// </summary>
    public class LoginBtnsLayer : WLayer
    {
        public static new LoginBtnsLayer Create()
        {
            return new LoginBtnsLayer();
        }

        protected LoginBtnsLayer()
        {
            InitGameObject("WLHall/Main/hall_login_other", "BtnCanvas.prefab");

            // 微乐登录按钮
            var wlBt = FindReference<Button>("wlLoginBt");
            wlBt.onClick.AddListener(() =>
            {
                _ShowAccountLoginPanel();
            });
        }

        // 显示账号登录界面
        private void _ShowAccountLoginPanel()
        {
            string cookieAccount = PlayerPrefs.GetString("CookieAccount");
            string cookiePassword = PlayerPrefs.GetString("CookiePassword");
            var loginPanel = new TmpLoginPanel(cookieAccount, cookiePassword);
            loginPanel.AddTo(WDirector.GetRootLayer());
        }

    }
}

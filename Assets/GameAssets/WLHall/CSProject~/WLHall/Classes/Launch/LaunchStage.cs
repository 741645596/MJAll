
using UnityEngine;
using Unity.Widget;
using UnityEngine.SceneManagement;
using WLCore.Stage;

namespace WLHall
{
    /// <summary>
    /// 游戏启动业务
    /// </summary>
    public class LaunchStage : BaseStage
    {
        private LoginManager _loginManager;
        private string _account;
        private string _password;

        /// <summary>
        /// 登录界面z坐标
        /// </summary>
        enum ZOrder
        {
            Bg = 800,
            Effect = 600,
            Button = 400,
        }

        /// <summary>
        /// 进入登录界面
        /// </summary>
        public static void EnterLaunchStage()
        {
            StageManager.RunStage(new LaunchStage());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public override void OnInitialize()
        {
            // Scene无法热更，所以统一使用CommonScene
            LoadScene("CommonScene", () => OnSceneDidLoad());
        }

        private void OnSceneDidLoad()
        {
            // 背景图
            WLayer.Create("WLHall/Main/hall_login_other", "BgCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), (int)ZOrder.Bg);

            // 效果层
            WLayer.Create("WLHall/Main/hall_login_other", "EffectCanvas.prefab")
                .AddTo(WDirector.GetRootLayer(), (int)ZOrder.Effect);

            // 按钮层，后续需要根据不同平台显示
            LoginBtnsLayer.Create()
                .AddTo(WDirector.GetRootLayer(), (int)ZOrder.Button);

            // TODO：版本检查、热更新等
        }

        public void OnClickLogin(string account, string password)
        {
            if (_loginManager == null)
            {
                _loginManager = new LoginManager();
                _loginManager.onLogin += OnLoginDone;
            }

            _loginManager.LoginByAccount(account, password);
            _account = account;
            _password = password;
        }

        // 登录成功回调
        private void OnLoginDone(int code, string session, uint userId, string hallServerIp, int port)
        {
            if (code != 0)
            {
                WLDebug.LogWarning("登陆失败，errorCode：", code);
                _account = null;
                _password = null;
                return;
            }

            GameAppStage.Instance.ConnectHallServer(session, userId, hallServerIp, port);

            // make cookies
            PlayerPrefs.SetString("CookieAccount", _account);
            PlayerPrefs.SetString("CookiePassword", _password);
        }

        public override void OnUpdate(float deltaTime)
        {
            _loginManager?.Update();
        }

        public override void OnShutdown()
        {
            base.OnShutdown();

            if (_loginManager != null)
            {
                _loginManager.onLogin -= OnLoginDone;
                _loginManager.Dispose();
                _loginManager = null;
            }
        }
    }
}

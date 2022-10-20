using Unity.Widget;
using WLCommon;
using WLCore.Stage;

namespace WLHall
{
    /// <summary>
    /// 启动入口类。游戏一直存活的Stage，主要用来处理业务逻辑的Update，如网络等；还有统一的前后台切换业务逻辑处理
    /// </summary>
    public class GameAppStage : BaseStage
    {
        /// <summary>
        /// 快捷访问
        /// </summary>
        public static GameAppStage Instance { get; private set; }

        /// <summary>
        /// 大厅管理器
        /// </summary>
        internal HallManager hallManager { get; private set; }
        

        private LoginManager _loginManager;


        /// <summary>
        /// 初始化
        /// </summary>
        public override void OnInitialize()
        {
            Instance = this;

            // 初始化默认字体
            TextHelper.InitDefaultFont("WLHall/Main/hall_fonts", "SourceHanSansCN.ttf");
            TextHelper.InitDefaultBlodFont("WLHall/Main/hall_fonts", "SourceHanSansCN Blod.ttf");

            // 初始化cocos框架
            WDirector.Init();

            // 初始Cocos UI适配方案
            AdaptationLogic.Init();

            // 品质设置
            QualityManager.InitQualityInfo();
            QualityManager.InitDefaultConfig();

            // 初始化Bugly
            BuglyLogic.Init();
            BuglyLogic.ShowDebugWindow();

            if (QuickConfig.autoJoinRoom)
            {
                _loginManager = new LoginManager();
                _loginManager.onLogin += (code, session, userId, serverUrl, port) => ConnectHallServer(session, userId, serverUrl, port);
                _loginManager.LoginByAccount(QuickConfig.account, QuickConfig.password);
            }
            else
            {
                // 进入启动场景
                DelayTimer.Call(0, ()=>
                {
                    LaunchStage.EnterLaunchStage();
                });
            }
            
        }

        /// <summary>
        /// 连接大厅服务器，一般在登陆成功后调用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="userId"></param>
        /// <param name="hallServerUrl"></param>
        public void ConnectHallServer(string session, uint userId, string hallServerUrl, int port)
        {
            hallManager?.Dispose();
            hallManager = new HallManager();
            hallManager.Connect(session, userId, hallServerUrl, port);
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void OnUpdate(float deltaTime)
        {
            // 屏幕旋转，UI适配检测
            //AdaptationLogic.__OnUpdate(deltaTime);

            // 网络层
            _loginManager?.Update();
            hallManager?.Update();
        }

        /// <summary>
        /// 进入后台
        /// </summary>
        public override void OnBackground()
        {

        }

        /// <summary>
        /// 进入前台
        /// </summary>
        public override void OnForeground()
        {

        }

        /// <summary>
        /// 生命周期结束，一般只有本地热更会触发
        /// </summary>
        public override void OnShutdown()
        {
            hallManager?.Dispose();
            hallManager = null;

            _loginManager?.Dispose();
            _loginManager = null;

            // 固定放在最后
            Instance = null;
        }
    }
}

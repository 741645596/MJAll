// IEntityEvent.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/27

namespace WLCore.Entity
{
    /// <summary>
    /// MonoBehaviour事件接口
    /// </summary>
    public interface IEntityEvent
    {
        /// <summary>
	    /// 启动事件
	    /// </summary>
        void OnStart();

        /// <summary>
	    /// 销毁事件
	    /// </summary>
        void OnDestroy();

        /// <summary>
	    /// 游戏循环
	    /// </summary>
        //void OnUpdate();

        /// <summary>
	    /// Enabel事件
	    /// </summary>
        void OnEnable();

        /// <summary>
	    /// Disable事件
	    /// </summary>
        void OnDisable();
    }
}

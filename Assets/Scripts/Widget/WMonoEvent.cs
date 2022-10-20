using System;
using UnityEngine;

namespace Unity.Widget
{
    /// <summary>
    /// 因为热更工程无法继承MonoBehaviour，所以需要弄个中间件桥接
    /// 比如需要注册通知事件刷新某些逻辑，对象被删除后需要注销通知事件，可以通过绑定该组件实现
    /// 继承WNode3D的类，可以使用EnableMonoEvent接口开启
    /// </summary>
    public class WMonoEvent : MonoBehaviour
    {
        public WNode3D self;
        public Action StartEvent;
        public Action OnDestroyEvent;
        public Action OnEnableEvent;
        public Action OnDisableEvent;

        protected void Start()
        {
            StartEvent?.Invoke();
        }

        protected void OnDestroy()
        {
            self = null;
            OnDestroyEvent?.Invoke();
        }

        protected void OnEnable()
        {
            OnEnableEvent?.Invoke();
        }

        protected void OnDisable()
        {
            OnDisableEvent?.Invoke();
        }

        // 对性能有一定影响，不开启
        //protected void Update()
        //{
        //}
    }
}


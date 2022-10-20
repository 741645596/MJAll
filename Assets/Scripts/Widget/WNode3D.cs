using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WLCore.Coroutine;

namespace Unity.Widget
{
    /// <summary>
    /// GameObject常用方法封装
    /// </summary>
    public class WNode3D
    {
        public GameObject gameObject;
        public Transform transform;

        private bool _isActive;
        private CoroutineProxy _coroutineProxy;
        private AnimationEventSender _animationEventSender;

        /// <summary>
        /// 创建预设控件
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetKey"></param>
        /// <returns></returns>
        public static WNode3D Create(string assetName, string assetKey)
        {
            var node = new WNode3D();
            node.InitGameObject(assetName, assetKey);
            return node;
        }

        /// <summary>
        /// 通过GameObject初始化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static WNode3D Create(GameObject obj)
        {
            var node = new WNode3D();
            node.InitGameObject(obj);
            return node;
        }

        /// <summary>
        /// 初始Object
        /// </summary>
        /// <param name="obj"></param>
        public void InitGameObject(GameObject obj)
        {
            gameObject = obj;

            transform = gameObject.transform;
            _isActive = gameObject.activeSelf;

            InitMonoEvent();
        }

        /// <summary>
        /// 通过AB包资源初始化
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="assetKey"></param>
        public void InitGameObject(string assetName, string assetKey)
        {
            gameObject = ObjectHelper.Instantiate(assetName, assetKey);
            Debug.Assert(gameObject != null, $"错误提示：{assetName}/{assetKey}资源不存在，请检查");

            transform = gameObject.transform;
            _isActive = gameObject.activeSelf;

            InitMonoEvent();
        }

        public WNode3D SetParent(GameObject obj)
        {
            transform.SetParent(obj.transform, false);
            return this;
        }

        public WNode3D SetParent(Transform t)
        {
            transform.SetParent(t, false);
            return this;
        }

        public WNode3D SetParent(WNode node)
        {
            transform.SetParent(node.transform, false);
            return this;
        }

        public void SetActive(bool b)
        {
            if (_isActive == b)
            {
                return;
            }

            gameObject.SetActive(b);
            _isActive = b;
        }

        public bool IsActive()
        {
            return _isActive && gameObject.activeInHierarchy;
        }

        public WNode3D SetName(string name)
        {
            gameObject.name = name;
            return this;
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public void RemoveFromParent()
        {
            if (gameObject != null)
            {
                GameObject.Destroy(gameObject);
                gameObject = null;
                transform = null;
            }
        }

        /// <summary>
        /// 对应MonoBehaviour的Start、OnDestory、OnEnable、OnDisable重写方法
        /// 比如UI界面需要注册通知事件，界面删除后需要删除通知，可以重写Start注册通知，重写OnDestroy注销通知
        /// </summary>
        protected virtual void Start() { }
        protected virtual void OnDestroy() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        /// <summary>
        /// 获取第一层的所有子节点
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetChildren()
        {
            return transform.GetChildren();
        }

        /// <summary>
        /// 获取所有子节点，递归所有子节点
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetAllChildren()
        {
            return transform.GetAllChildren();
        }

        /// <summary>
        /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
        /// </summary>
        /// <param name="childName"></param>
        /// <returns></returns>
        public Transform FindInChildren(string childName)
        {
            return transform.FindInChildren(childName);
        }

        /// <summary>
        /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="childName"></param>
        /// <returns></returns>
        public T FindInChildren<T>( string childName) where T : Component
        {
            return transform.FindInChildren<T>(childName);
        }

        /// <summary>
        /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Transform FindInChildren(Func<Transform, bool> condition)
        {
            return transform.FindInChildren(condition);
        }

        /// <summary>
        /// 建议使用该接口，只查找第一层子节点，不会递归子节点查询，childName可以包含子节点，如："FirstNodeName/SubNodeName"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T FindInChildren<T>(Func<Transform, bool> condition) where T : Component
        {
            return transform.FindInChildren<T>(condition);
        }

        /// <summary>
        /// 递归查找子节点
        /// </summary>
        /// <param name="name">查找对象名称</param>
        /// <returns></returns>
        public Transform FindReference(string name)
        {
            return transform.FindReference(name);
        }

        /// <summary>
        /// 递归查找符合条件的子节点
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public Transform FindReference(Func<Transform, bool> condition)
        {
            return transform.FindReference(condition);
        }

        /// <summary>
        /// 递归查找子节点的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T FindReference<T>(string name) where T : Component
        {
            return transform.FindReference<T>(name);
        }

        /// <summary>
        /// 递归查找子节点的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T FindReference<T>(Func<Transform, bool> condition) where T : Component
        {
            return transform.FindReference<T>(condition);
        }

        public T FindNode<T>(string childName) where T : WNode3D
        {
            return transform.FindNode<T>(childName);
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>() where T : Component
        {
            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// 确保需要的组件存在，不存在则创建一个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T EnsureComponent<T>() where T : Component
        {
            return gameObject.EnsureComponent<T>();
        }

        /// <summary>
        /// 开始协程
        /// </summary>
        /// <param name="routine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return _GetCoroutinProxy().StartCoroutine(routine);
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="coroutine">由StartCoroutine方法返回</param>
        public void StopCoroutine(Coroutine coroutine)
        {
            _GetCoroutinProxy().StopCoroutine(coroutine);
        }

        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Coroutine DelayInvoke(float delay, Action action)
        {
            return _GetCoroutinProxy().DelayInvoke(delay, action);
        }

        /// <summary>
        /// 重复延时调用，每隔delay时间后调用一次回调
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="initialTime"></param>
        /// <returns></returns>
        public Coroutine DelayInvokeRepeating(float delay, Action action, float initialTime = 0)
        {
            return _GetCoroutinProxy().DelayInvokeRepeating(delay, action, initialTime);
        }

        /// <summary>
        /// 延迟几秒后删除自身
        /// </summary>
        /// <param name="delay"></param>
        public void RemoveSelf(float delayTime)
        {
            _GetCoroutinProxy().DelayInvoke(delayTime, ()=>
            {
                RemoveFromParent();
            });
        }

        /// <summary>
        /// 取消延时调用
        /// </summary>
        /// <param name="coroutine">由DelayInvoke或DelayInvokeRepeating返回</param>
        public void CancelInvoke(Coroutine coroutine)
        {
            _GetCoroutinProxy().CancelInvoke(coroutine);
        }

        /// <summary>
        /// 注册动画事件。制作预设体时需要在gameObject上添加AnimationEventSender组件，并在动画系统中绑定AnimationEventSender.OnEvent方法，回传参数输入eventName
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="eventHandler"></param>
        public void RegisterAnimationEvent(string eventName, Action eventHandler)
        {
            if (_animationEventSender != null)
            {
                _animationEventSender.Bind(eventName, eventHandler);
                return;
            }

            _animationEventSender = gameObject.GetComponent<AnimationEventSender>();
            if (_animationEventSender == null)
            {
                WLDebug.LogWarning($"WNode3D.RegisterAnimationEvent: 注册动画事件失败，[{gameObject.name}]对象未添加AnimationEventSender组件，请检查预设体");
                return;
            }

            _animationEventSender.Bind(eventName, eventHandler);
        }

        /// <summary>
        /// 取消注册动画事件
        /// </summary>
        /// <param name="eventName"></param>
        public void UnregisterAnimationEvent(string eventName)
        {
            if (_animationEventSender == null)
            {
                return;
            }

            _animationEventSender.Bind(eventName, null);
        }

        protected void InitMonoEvent()
        {
            var monoEvent = gameObject.AddComponent<WMonoEvent>();
            monoEvent.self = this;

            monoEvent.StartEvent = Start;
            monoEvent.OnDestroyEvent = OnDestroy;
            monoEvent.OnEnableEvent = OnEnable;
            monoEvent.OnDisableEvent = OnDisable;
        }

        private CoroutineProxy _GetCoroutinProxy()
        {
            if (gameObject == null)
            {
                throw new Exception("WNode3D._GetCoroutinProxy: gameObject为空");
            }

            if (_coroutineProxy == null)
            {
                _coroutineProxy = gameObject.AddComponent<CoroutineProxy>();
            }

            return _coroutineProxy;
        }
    }
}

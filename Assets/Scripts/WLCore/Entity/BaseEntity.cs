// BaseEntity.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/27

using UnityEngine;
using Object = UnityEngine.Object;

namespace WLCore.Entity
{
    public class BaseEntity : IEntityEvent
    {
        public GameObject gameObject;

        private bool isActive = true;
        private EntityProxy proxy;

        public BaseEntity()
        {
        }

        /// <summary>
        /// 设置激活状态
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActive(bool isActive)
        {
            if (this.isActive == isActive)
            {
                return;
            }
            this.isActive = isActive;
            if (gameObject != null)
            {
                gameObject.SetActive(isActive);
            }
        }

        /// <summary>
        /// 是否激活
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return isActive;
        }

        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="worldPositionStays"></param>
        public void SetParent(BaseEntity parent, bool worldPositionStays = true)
        {
            if (parent == null || parent.gameObject == null)
            {
                return;
            }

            if (gameObject == null)
            {
                return;
            }

            gameObject.transform.SetParent(parent.gameObject.transform, worldPositionStays);
        }

        /// <summary>
        /// 获取MonoBehaviour事件代理
        /// </summary>
        /// <returns></returns>
        public EntityProxy GetEntityProxy()
        {
            return proxy;
        }

        /// <summary>
        /// 启用MonoBehaviour事件
        /// </summary>
        public void EnableMonoEvent()
        {
            if(gameObject == null)
            {
                return;
            }
            proxy = gameObject.GetComponent<EntityProxy>();

            if (proxy == null)
            {
                proxy = gameObject.AddComponent<EntityProxy>();
            }
            proxy.IEntity = this;
        }

        /// <summary>
        /// 销毁实体
        /// </summary>
        public void Destroy()
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
                gameObject = null;
            }
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 跨域继承适配器BaseEntityAdaptor的构造方法会调用
        /// gameObject = CreateGameObject()
        /// 子类实现CreateGameObject后会直接赋值给成员变量gameObject
        /// </summary>
        /// <returns></returns>
        protected virtual GameObject CreateGameObject()
        {
            return new GameObject("UntitledEntity");
        }
    }
}

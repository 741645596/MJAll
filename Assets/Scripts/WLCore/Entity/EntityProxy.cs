// EntityProxy.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/27
using UnityEngine;

namespace WLCore.Entity
{
    public class EntityProxy : MonoBehaviour
    {
        public IEntityEvent IEntity { private get; set; }

        private void Start()
        {
            if (IEntity != null)
            {
                IEntity.OnStart();
            }
        }

        // 即使是空函数，Update也会带来性能消耗，为防止被滥用屏蔽掉
        //private void Update()
        //{
        //    if (IEntity != null)
        //    {
        //        IEntity.OnUpdate();
        //    }
        //}

        private void OnDestroy()
        {
            if (IEntity != null)
            {
                IEntity.OnDestroy();
            }
        }

        private void OnEnable()
        {
            if (IEntity != null)
            {
                IEntity.OnEnable();
            }
        }

        private void OnDisable()
        {
            if (IEntity != null)
            {
                IEntity.OnDisable();
            }
        }
    }
}

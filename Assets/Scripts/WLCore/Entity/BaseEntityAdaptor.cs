// BaseEntityAdaptor.cs
// Author: shihongyang shihongyang@weile.com
// Date: 2021/7/27
using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using WLCore.Entity;

public class BaseEntityAdaptor : CrossBindingAdaptor
{

    static CrossBindingMethodInfo mVMethod_OnStart = new CrossBindingMethodInfo("OnStart");
    static CrossBindingMethodInfo mVMethod_OnDestroy = new CrossBindingMethodInfo("OnDestroy");
    static CrossBindingMethodInfo mVMethod_OnEnable = new CrossBindingMethodInfo("OnEnable");
    static CrossBindingMethodInfo mVMethod_OnDisable = new CrossBindingMethodInfo("OnDisable");
    //static CrossBindingMethodInfo mVMethod_OnUpdate = new CrossBindingMethodInfo("OnUpdate");
    static CrossBindingFunctionInfo<GameObject> mVMethod_CreateGameObject = new CrossBindingFunctionInfo<GameObject>("CreateGameObject");

    public BaseEntityAdaptor()
    {
    }

    public override Type BaseCLRType => typeof(BaseEntity);

    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : BaseEntity, CrossBindingAdaptorType
    {

        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance { get { return instance; } }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;

            if(gameObject == null)
            {
                gameObject = CreateGameObject();
            }
        }

        public override void OnDestroy()
        {
            if (mVMethod_OnDestroy.CheckShouldInvokeBase(instance))
                base.OnDestroy();
            else
                mVMethod_OnDestroy.Invoke(instance);
        }

        public override void OnDisable()
        {
            if (mVMethod_OnDisable.CheckShouldInvokeBase(instance))
                base.OnDisable();
            else
                mVMethod_OnDisable.Invoke(instance);
        }

        public override void OnEnable()
        {
            if (mVMethod_OnEnable.CheckShouldInvokeBase(instance))
                base.OnEnable();
            else
                mVMethod_OnEnable.Invoke(instance);
        }

        public override void OnStart()
        {
            if (mVMethod_OnStart.CheckShouldInvokeBase(instance))
                base.OnStart();
            else
                mVMethod_OnStart.Invoke(instance);
        }

        //public override void OnUpdate()
        //{
        //    if (mVMethod_OnUpdate.CheckShouldInvokeBase(instance))
        //        base.OnUpdate();
        //    else
        //        mVMethod_OnUpdate.Invoke(instance);
        //}

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }

        protected override GameObject CreateGameObject()
        {
            if (mVMethod_CreateGameObject.CheckShouldInvokeBase(instance))
                return base.CreateGameObject();
            else
                return mVMethod_CreateGameObject.Invoke(instance);
        }
    }

}

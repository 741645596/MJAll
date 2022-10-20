using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Unity.Widget;
using UnityEngine.EventSystems;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class WLayerAdaptor : CrossBindingAdaptor
{
    static CrossBindingFunctionInfo<PointerEventData, bool> mVMethod_OnTouchBegan = new CrossBindingFunctionInfo<PointerEventData, bool>("OnTouchBegan");
    static CrossBindingMethodInfo<PointerEventData> mVMethod_OnTouchMove = new CrossBindingMethodInfo<PointerEventData>("OnTouchMove");
    static CrossBindingMethodInfo<PointerEventData> mVMethod_OnTouchEnd = new CrossBindingMethodInfo<PointerEventData>("OnTouchEnd");

    static CrossBindingMethodInfo mVMethod_Start = new CrossBindingMethodInfo("Start");
    static CrossBindingMethodInfo mVMethod_OnDestroy = new CrossBindingMethodInfo("OnDestroy");
    static CrossBindingMethodInfo mVMethod_OnEnable = new CrossBindingMethodInfo("OnEnable");
    static CrossBindingMethodInfo mVMethod_OnDisable = new CrossBindingMethodInfo("OnDisable");

    public override Type BaseCLRType => typeof(WLayer);
    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public WLayerAdaptor()
    {
    }

    public class Adaptor : WLayer, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance { get { return instance; } }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        protected override bool OnTouchBegan(PointerEventData d)
        {
            if (mVMethod_OnTouchBegan.CheckShouldInvokeBase(instance))
                return base.OnTouchBegan(d);
            else
                return mVMethod_OnTouchBegan.Invoke(instance, d);
        }

        protected override void OnTouchMove(PointerEventData d)
        {
            if (mVMethod_OnTouchMove.CheckShouldInvokeBase(instance))
                base.OnTouchMove(d);
            else
                mVMethod_OnTouchMove.Invoke(instance, d);
        }

        protected override void OnTouchEnd(PointerEventData d)
        {
            if (mVMethod_OnTouchEnd.CheckShouldInvokeBase(instance))
                base.OnTouchEnd(d);
            else
                mVMethod_OnTouchEnd.Invoke(instance, d);
        }

        protected override void Start()
        {
            if (mVMethod_Start.CheckShouldInvokeBase(instance))
                base.Start();
            else
                mVMethod_Start.Invoke(instance);
        }

        protected override void OnDestroy()
        {
            if (mVMethod_OnDestroy.CheckShouldInvokeBase(instance))
                base.OnDestroy();
            else
                mVMethod_OnDestroy.Invoke(instance);
        }

        protected override void OnEnable()
        {
            if (mVMethod_OnEnable.CheckShouldInvokeBase(instance))
                base.OnEnable();
            else
                mVMethod_OnEnable.Invoke(instance);
        }

        protected override void OnDisable()
        {
            if (mVMethod_OnDisable.CheckShouldInvokeBase(instance))
                base.OnDisable();
            else
                mVMethod_OnDisable.Invoke(instance);
        }
    }

}

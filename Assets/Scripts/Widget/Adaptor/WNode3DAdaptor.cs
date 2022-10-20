using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Unity.Widget;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class WNode3DAdaptor : CrossBindingAdaptor
{
    static CrossBindingMethodInfo mVMethod_Start = new CrossBindingMethodInfo("Start");
    static CrossBindingMethodInfo mVMethod_OnDestroy = new CrossBindingMethodInfo("OnDestroy");
    static CrossBindingMethodInfo mVMethod_OnEnable = new CrossBindingMethodInfo("OnEnable");
    static CrossBindingMethodInfo mVMethod_OnDisable = new CrossBindingMethodInfo("OnDisable");

    public override Type BaseCLRType => typeof(WNode3D);
    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : WNode3D, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance { get { return instance; } }

        public Adaptor()
        {
        }

        public Adaptor(AppDomain domain, ILTypeInstance instance) 
        {
            this.appdomain = domain;
            this.instance = instance;
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

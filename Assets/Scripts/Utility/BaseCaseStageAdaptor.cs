using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using Unity.Core;

public class BaseCaseStageAdaptor : CrossBindingAdaptor
{
    private static CrossBindingMethodInfo mVMethod_OnInitialize = new CrossBindingMethodInfo("OnInitialize");
    private static CrossBindingMethodInfo mVMethod_OnShutdown = new CrossBindingMethodInfo("OnShutdown");
    private static CrossBindingMethodInfo mVMethod_OnForeground = new CrossBindingMethodInfo("OnForeground");
    private static CrossBindingMethodInfo mVMethod_OnBackground = new CrossBindingMethodInfo("OnBackground");
    private static CrossBindingMethodInfo<float> mVMethod_OnUpdate = new CrossBindingMethodInfo<float>("OnUpdate");

    public override Type BaseCLRType => typeof(BaseCaseStage);

    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : BaseCaseStage, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        public ILTypeInstance ILInstance { get { return instance; } }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public override void OnInitialize()
        {
            if (mVMethod_OnInitialize.CheckShouldInvokeBase(instance))
                base.OnInitialize();
            else
                mVMethod_OnInitialize.Invoke(instance);
        }
        
        public override void OnUpdate(float deltaTime)
        {
            if (mVMethod_OnUpdate.CheckShouldInvokeBase(instance))
                base.OnUpdate(deltaTime);
            else
                mVMethod_OnUpdate.Invoke(instance, deltaTime);
        }

        public override void OnShutdown()
        {
            if (mVMethod_OnShutdown.CheckShouldInvokeBase(instance))
                base.OnShutdown();
            else
                mVMethod_OnShutdown.Invoke(instance);
        }

        public override void OnForeground()
        {
            if (mVMethod_OnForeground.CheckShouldInvokeBase(instance))
                base.OnForeground();
            else
                mVMethod_OnForeground.Invoke(instance);
        }

        public override void OnBackground()
        {
            if (mVMethod_OnBackground.CheckShouldInvokeBase(instance))
                base.OnBackground();
            else
                mVMethod_OnBackground.Invoke(instance);
        }

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
    }
}

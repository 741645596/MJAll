// EventArgsAdaptor.cs
// Author: shihongyang <shihongyang@weile.com>
// Date: 2019/5/28
using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class EventArgsAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType => typeof(System.EventArgs);

    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    public class Adaptor : System.EventArgs, CrossBindingAdaptorType
    {
        private ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        public ILTypeInstance ILInstance { get; }

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            ILInstance = instance;
        }
    }
}
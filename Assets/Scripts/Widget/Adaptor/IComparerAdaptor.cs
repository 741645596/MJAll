using System;
using System.Collections.Generic;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IComparerAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType => typeof(IComparer<object>);

    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    static CrossBindingFunctionInfo<object, object, System.Int32> mAbCompareMethod = new CrossBindingFunctionInfo<object, object, System.Int32>("Compare");

    public class Adaptor : IComparer<object>, CrossBindingAdaptorType
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

        IMethod mCompareMethod;
        bool mCompareMethodGot;
        public int Compare(object x, object y)
        {
            if (!mCompareMethodGot)
            {
                mCompareMethod = ILInstance.Type.GetMethod("Compare", 2);
                if (mCompareMethod == null)
                {
                    mCompareMethod = ILInstance.Type.GetMethod("System.Collections.Generic.IComparer", 2);
                }
                mCompareMethodGot = true;
            }
            if (mCompareMethod != null)
            {
                return (int)appdomain.Invoke(mCompareMethod, ILInstance, x, y);
            }

            return 0;
        }
    }
}

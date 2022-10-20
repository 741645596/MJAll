using System;
using System.Runtime.CompilerServices;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

public class IAsyncStateMachineAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType => typeof(IAsyncStateMachine);

    public override Type AdaptorType => typeof(Adaptor);

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);
    }

    static CrossBindingFunctionInfo<object, object, System.Int32> mAbCompareMethod = new CrossBindingFunctionInfo<object, object, System.Int32>("Compare");

    public class Adaptor : IAsyncStateMachine, CrossBindingAdaptorType
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

        IMethod mMoveNextMethod;
        bool mMoveNextMethodGot;
        public void MoveNext()
        {
            if (!mMoveNextMethodGot)
            {
                mMoveNextMethod = ILInstance.Type.GetMethod("MoveNext", 0);
                if (mMoveNextMethod == null)
                {
                    mMoveNextMethod = ILInstance.Type.GetMethod("System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext", 0);
                }
                mMoveNextMethodGot = true;
            }
            if (mMoveNextMethod != null)
            {
                appdomain.Invoke(mMoveNextMethod, ILInstance);
            }
        }

        IMethod mSetStateMachineMethod;
        bool mSetStateMachineMethodGot;
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            if (!mSetStateMachineMethodGot)
            {
                mSetStateMachineMethod = ILInstance.Type.GetMethod("SetStateMachine", 1);
                if (mSetStateMachineMethod == null)
                {
                    mSetStateMachineMethod = ILInstance.Type.GetMethod("System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine", 1);
                }
                mSetStateMachineMethodGot = true;
            }
            if (mSetStateMachineMethod != null)
            {
                appdomain.Invoke(mSetStateMachineMethod, ILInstance, stateMachine);
            }
        }
    }
}

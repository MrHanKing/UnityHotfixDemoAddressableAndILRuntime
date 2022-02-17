using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ETModel;

namespace EnhancedUI.EnhancedScroller
{
    // 热更适配脚本 暂时删掉了
    // [ILAdapter]
    // public class EnhancedScrollerCellViewAdaptor : CrossBindingAdaptor
    // {
    // //定义访问方法的方法信息
    // public override Type BaseCLRType
    // {
    //     get
    //     {
    //         return typeof(EnhancedScrollerCellView);//这里是你想继承的类型
    //     }
    // }

    // public override Type AdaptorType
    // {
    //     get
    //     {
    //         return typeof(Adapter);
    //     }
    // }

    // public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    // {
    //     return new Adapter(appdomain, instance);
    // }

    // public class Adapter : EnhancedScrollerCellView, CrossBindingAdaptorType
    // {
    //     ILTypeInstance instance;
    //     ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    //     //必须要提供一个无参数的构造函数
    //     public Adapter()
    //     {

    //     }

    //     public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    //     {
    //         this.appdomain = appdomain;
    //         this.instance = instance;
    //     }

    //     public ILTypeInstance ILInstance { get { return instance; } }

    //     IMethod mRefreshCellView;
    //     bool isRefreshCellView = false;

    //     public override void RefreshCellView()
    //     {
    //         if (mRefreshCellView == null)
    //         {
    //             mRefreshCellView = instance.Type.GetMethod("RefreshCellView", 0);
    //         }
    //         if (mRefreshCellView != null && !isRefreshCellView)
    //         {
    //             isRefreshCellView = true;
    //             appdomain.Invoke(mRefreshCellView, instance);
    //             isRefreshCellView = false;
    //         }                    
    //     }

    //     public override string ToString()
    //     {
    //         IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
    //         m = instance.Type.GetVirtualMethod(m);
    //         if (m == null || m is ILMethod)
    //         {
    //             return instance.ToString();
    //         }
    //         else
    //             return instance.Type.FullName;
    //     }
    // }
    // }
}

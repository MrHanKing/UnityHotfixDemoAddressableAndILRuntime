using System;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.CLR.Method;
using ETModel;

namespace EnhancedUI.EnhancedScroller
{
    // [ILAdapter]
    // public class IEnhancedScrollerDelegateAdaptor : CrossBindingAdaptor
    // {
    //     //定义访问方法的方法信息
    //     public override Type BaseCLRType
    //     {
    //         get
    //         {
    //             return typeof(IEnhancedScrollerDelegate);//这里是你想继承的类型
    //         }
    //     }

    //     public override Type AdaptorType
    //     {
    //         get
    //         {
    //             return typeof(Adapter);
    //         }
    //     }

    //     public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    //     {
    //         return new Adapter(appdomain, instance);
    //     }

    //     public class Adapter : IEnhancedScrollerDelegate, CrossBindingAdaptorType
    //     {
    //         ILTypeInstance instance;
    //         ILRuntime.Runtime.Enviorment.AppDomain appdomain;

    //         //必须要提供一个无参数的构造函数
    //         public Adapter()
    //         {

    //         }

    //         public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    //         {
    //             this.appdomain = appdomain;
    //             this.instance = instance;
    //         }

    //         public ILTypeInstance ILInstance { get { return instance; } }

    //         public int GetNumberOfCells(EnhancedScroller scroller)
    //         {
    //             var mRefreshCellView = instance.Type.GetMethod("GetNumberOfCells", 1);

    //             if (mRefreshCellView != null)
    //             {
    //                 var param = new object[1];
    //                 param[0] = scroller;
    //                 var result = appdomain.Invoke(mRefreshCellView, instance, param);
    //                 return (int)result;
    //             }
    //             return 0;
    //         }
    //         public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    //         {
    //             var mRefreshCellView = instance.Type.GetMethod("GetCellViewSize", 2);

    //             if (mRefreshCellView != null)
    //             {
    //                 var param = new object[2];
    //                 param[0] = scroller;
    //                 param[1] = dataIndex;
    //                 var result = appdomain.Invoke(mRefreshCellView, instance, param);
    //                 return (float)result;
    //             }
    //             return 0f;
    //         }
    //         public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    //         {
    //             var mRefreshCellView = instance.Type.GetMethod("GetCellView", 3);

    //             if (mRefreshCellView != null)
    //             {
    //                 var param = new object[3];
    //                 param[0] = scroller;
    //                 param[1] = dataIndex;
    //                 param[2] = cellIndex;
    //                 var result = appdomain.Invoke(mRefreshCellView, instance, param);
    //                 return (EnhancedScrollerCellView)result;
    //             }
    //             return null;
    //         }

    //         public override string ToString()
    //         {
    //             IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
    //             m = instance.Type.GetVirtualMethod(m);
    //             if (m == null || m is ILMethod)
    //             {
    //                 return instance.ToString();
    //             }
    //             else
    //                 return instance.Type.FullName;
    //         }
    //     }
    // }
}
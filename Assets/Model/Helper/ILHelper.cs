using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Generated;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;

namespace ETModel
{
    public static class ILHelper
    {
        public static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain appdomain)
        {
            // 注册重定向函数

            // 注册委托
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<System.String>>((act) =>
            {
                return new System.Predicate<System.String>((obj) =>
                {
                    return ((Func<System.String, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<UnityEngine.GameObject>>((act) =>
            {
                return new System.Comparison<UnityEngine.GameObject>((x, y) =>
                {
                    return ((Func<UnityEngine.GameObject, UnityEngine.GameObject, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.GameObject>>((act) =>
            {
                return new System.Predicate<UnityEngine.GameObject>((obj) =>
                {
                    return ((Func<UnityEngine.GameObject, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<UnityEngine.Vector3>>((act) =>
            {
                return new System.Comparison<UnityEngine.Vector3>((x, y) =>
                {
                    return ((Func<UnityEngine.Vector3, UnityEngine.Vector3, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.Vector3>>((act) =>
            {
                return new System.Predicate<UnityEngine.Vector3>((obj) =>
                {
                    return ((Func<UnityEngine.Vector3, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<Lean.Touch.LeanFinger>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<Lean.Touch.LeanFinger>((arg0) =>
                {
                    ((Action<Lean.Touch.LeanFinger>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Comparison<ILRuntime.Runtime.Intepreter.ILTypeInstance>((x, y) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>)act)(x, y);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((Action<System.Single>)act)(arg0);
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<bool>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<bool>((arg0) =>
                {
                    ((Action<bool>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            appdomain.DelegateManager.RegisterDelegateConvertor<ETModel.UIBehaviours.VoidDelegate>((act) =>
            {
                return new ETModel.UIBehaviours.VoidDelegate((go) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(go);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
                {
                    ((Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.String>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.String>((arg0) =>
                {
                    ((Action<System.String>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Int32>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Int32>((arg0) =>
                {
                    ((Action<System.Int32>)act)(arg0);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>>((act) =>
            {
                return new System.Predicate<ILRuntime.Runtime.Intepreter.ILTypeInstance>((obj) =>
                {
                    return ((Func<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Video.VideoPlayer.EventHandler>((act) =>
            {
                return new UnityEngine.Video.VideoPlayer.EventHandler((source) =>
                {
                    ((Action<UnityEngine.Video.VideoPlayer>)act)(source);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.UI.Button>>((act) =>
            {
                return new System.Predicate<UnityEngine.UI.Button>((obj) =>
                {
                    return ((Func<UnityEngine.UI.Button, System.Boolean>)act)(obj);
                });
            });
            appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Video.VideoPlayer.ErrorEventHandler>((act) =>
            {
                return new UnityEngine.Video.VideoPlayer.ErrorEventHandler((source, message) =>
                {
                    ((Action<UnityEngine.Video.VideoPlayer, System.String>)act)(source, message);
                });
            });

            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String[], System.Int32, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String, System.Int32, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Collections.Hashtable>();

            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.UI.Button, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Video.VideoPlayer>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Action, System.Object>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Texture2D>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.GameObject>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Object>();
            appdomain.DelegateManager.RegisterMethodDelegate<List<object>>();
            appdomain.DelegateManager.RegisterMethodDelegate<byte[], int, int>();
            appdomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, System.String>();
            appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance, System.Int32>();

            appdomain.DelegateManager.RegisterMethodDelegate<Lean.Touch.LeanFinger>();
            appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.AsyncOperation>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3, UnityEngine.Vector3, System.Int32>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Single>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Threading.Tasks.Task>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.String>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Threading.Tasks.TaskCompletionSource<System.Boolean>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, System.Boolean>();
            appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, UnityEngine.GameObject, System.Int32>();

            appdomain.DelegateManager.RegisterFunctionDelegate<System.String, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Single, System.Boolean>();
            appdomain.DelegateManager.RegisterMethodDelegate<System.Threading.Tasks.Task<System.Boolean>>();
            appdomain.DelegateManager.RegisterFunctionDelegate<System.Object, ILRuntime.Runtime.Intepreter.ILTypeInstance>();

            CLRBindings.Initialize(appdomain);

            // 注册适配器
            Assembly assembly = typeof(Init).Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(ILAdapterAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }
                object obj = Activator.CreateInstance(type);
                CrossBindingAdaptor adaptor = obj as CrossBindingAdaptor;
                if (adaptor == null)
                {
                    continue;
                }
                appdomain.RegisterCrossBindingAdaptor(adaptor);
            }

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);
        }

        public static object CreateInstance(Type type)
        {
            object result = null;
#if ILRuntime
            if (type is ILRuntime.Reflection.ILRuntimeType)
            {
                result = ((ILRuntime.Reflection.ILRuntimeType)type).ILType.Instantiate();
            }
            else if (type is ILRuntime.Reflection.ILRuntimeWrapperType)
            {
                result = Activator.CreateInstance(((ILRuntime.Reflection.ILRuntimeWrapperType)type).RealType);
            }
            else
            {
                result = Activator.CreateInstance(type);
            }
#else
            result = Activator.CreateInstance(type);
#endif

            return result;
        }
    }
}
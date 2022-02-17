using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ETModel;
using System;

namespace ETHotfix
{
    /// <summary>
    /// UI接口调用帮助器
    /// 方便使用UI层接口
    /// 注意 所有接口要让外部不用care其他UI模块
    /// 
    /// 使用指南:
    ///     ET框架内部使用了Game.EventSystem.Run(EventIdType.UIPropFrame);的方式去切换界面 没有问题
    ///     这个系统用于比如UIPropFrame界面内部常驻的UI给其他界面方便起调 需要在ET框架的UI的生命周期内去注册和释放 可以参考UIPropFrame内的PropKuang
    /// 
    /// 2021-09-06 新增适配 该模块功能调整为只支持二级以上界面 暂不对一级界面做支持
    /// </summary>
    public static class UIManagerHelper
    {
        /// <summary>
        /// 注册在案的激活UI
        /// 注意: key 是UIType的string
        /// </summary>
        private static Dictionary<string, UIManagerUIBaseEntity> registerUIComponent = new Dictionary<string, UIManagerUIBaseEntity>();

        public static void RegisterUI(string uiType, UIManagerUIBaseEntity uiComponent)
        {
            if (uiComponent == null)
            {
                Debug.LogWarning("你正在注册一个无效的uiComponent");
                return;
            }

            UIManagerUIBaseEntity target = null;
            if (registerUIComponent.TryGetValue(uiType, out target))
            {
                if (target == null)
                {
                    registerUIComponent[uiType] = uiComponent;
                    return;
                }

                if (target.InstanceId != uiComponent.InstanceId)
                {
                    Debug.LogError("你正在覆盖一个UI的实例 理论上不应该发生这种事情");
                    registerUIComponent[uiType] = uiComponent;
                }

                return;
            }

            registerUIComponent.Add(uiType, uiComponent);
        }

        public static void UnRegisterUI(string uiType)
        {
            registerUIComponent.Remove(uiType);
        }

        private static async Task TranstionIn(UIManagerShowConfig showConfig = null)
        {
            // 二级界面过渡效果 进入
        }

        private static async Task TranstionOut(UIManagerShowConfig showConfig = null)
        {
            // 二级界面过渡效果 进入完成
        }

        /// <summary>
        /// UI实现统一Show异步接口
        /// </summary>
        /// <param name="uiType">ui的类型 具体查看UIManagerType中的SubUIType定义</param>
        /// <param name="input">对应uiType 界面需要的输入参数</param>
        /// <returns>IDelegateResultBase 对应界面会返回的结果</returns>
        public static async Task<IDelegateResultBase> Show(string uiType, IDelegateInputDataBase input = null, UIManagerShowConfig showConfig = null)
        {
            await TranstionIn(showConfig);

            UIManagerUIBaseEntity target = GetUIEntity<UIManagerUIBaseEntity>(uiType);
            if (target == null)
            {
                // 尝试自动创建
                Log.Info("尝试自动创建一个新的界面");
                await CommonApiHelper.Create(uiType);
                target = GetUIEntity<UIManagerUIBaseEntity>(uiType);
            }
            if (target == null)
            {
                Debug.LogError("逻辑错误 此时公用弹出框 没注册或不存在 且无法自动创建: " + uiType);
                await TranstionOut(showConfig);
                return new DelegateResultNull();
            }

            var task = target.PreShow(input);
            // 特殊过渡 转场 Todo 优化
            await TranstionOut(showConfig);

            var result = await task as IDelegateResultBase;

            return result;

        }

        /// <summary>
        /// 弹出公用界面
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        // public static async Task<DelegateResultUIPropKuang> ShowCommonPropKuang(DelegateInputDataUIPropKuang input)
        // {
        //     var result = await Show(SubUIType.UIPropKuang, input) as DelegateResultUIPropKuang;
        //     return result;
        // }

        /// <summary>
        /// 拿到对应uiType界面的 容器类
        /// 注意如果界面不存在 会返回null
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public static T GetUIEntity<T>(string uiType) where T : UIManagerUIBaseEntity
        {
            T result = GetUIEntity(uiType) as T;
            return result;
        }

        /// <summary>
        /// 拿到对应uiType界面的类 方便调用接口
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        private static UIManagerUIBaseEntity GetUIEntity(string uiType)
        {
            UIManagerUIBaseEntity target = null;
            registerUIComponent.TryGetValue(uiType, out target);

            return target;
        }



        /// <summary>
        /// --------------------------------------------------下面是测试接口--------------------------------------------------
        /// Test 接口
        /// 依次测试公用界面
        /// </summary>
        // public static async void TestCode()
        // {
        //     if (Application.isEditor)
        //     {
        //         var target = Enum.GetValues(typeof(EUIPropKuangType));
        //         Log.Info("Length" + target.Length);
        //         foreach (EUIPropKuangType item in target)
        //         {
        //             Log.Info("Running:" + item.ToString());
        //             await ShowCommonPropKuang(new DelegateInputDataUIPropKuang(item));
        //             await Task.Delay(1000);
        //         }
        //     }
        // }
    }

    /// <summary>
    /// UIManager调用show的时候的可选配置
    /// ILRuntime没对可空struct做支持 改用class
    /// </summary>
    public class UIManagerShowConfig
    {
        /// <summary>
        /// 界面使用完后是否自动销毁
        /// </summary>
        public bool isAutoRelease = false;
        public AnimatorType animatorType = AnimatorType.SmallToBig;
    }

}
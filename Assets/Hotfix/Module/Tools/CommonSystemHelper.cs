using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 共用的单例系统注册器
    /// 如引导手系统、场景切换效果等等
    /// 此处功能类似服务寻址器 每个服务可以按需替换
    /// </summary>
    public static class CommonSystemHelper
    {

        public static GlobalGameObjectComponent globalGameObjects { get { return GlobalGameObjectComponent.Instance; } }
        /// <summary>
        /// 正在初始化中
        /// </summary>
        private static bool isIniting = false;
        /// <summary>
        /// 初始化所有可能使用的系统
        /// 明确初始化时机！！
        /// </summary>
        /// <returns></returns>
        public static async Task Init()
        {
            if (isIniting)
            {
                Debug.LogError("重复初始化CommonSystemHelper 调用时机有问题");
                return;
            }
            isIniting = true;
            try
            {
                // await CreateUIFinger();
                Game.Scene.AddComponent<UIComponent>();
                Game.Scene.AddComponent<GlobalGameObjectComponent>();
                Game.Scene.AddComponent<FullScreenControl>();
            }
            catch (System.Exception e)
            {
                Debug.LogError("公用单例系统初始化失败");
                Debug.LogError(e);
            }
            isIniting = false;
        }

        /// <summary>
        /// 引导系统 带UI的独立系统
        /// </summary>
        /// <returns></returns>
        // private static async Task CreateUIFinger()
        // {
        //     uIFinger = UIManagerHelper.GetUIEntity<UIFingerTips>(SubUIType.UIFingerTips);
        //     if (uIFinger == null)
        //     {
        //         await CommonApiHelper.Create(SubUIType.UIFingerTips);
        //         uIFinger = UIManagerHelper.GetUIEntity<UIFingerTips>(SubUIType.UIFingerTips);
        //         uIFinger?.Hide();
        //     }
        // }


    }
}
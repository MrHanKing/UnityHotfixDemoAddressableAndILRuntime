using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    public static class Init
    {
        /// <summary>
        /// 防止多次触发
        /// </summary>
        public static bool isInit = false;

        public static async void Start()
        {
            try
            {
#if ILRuntime
                if (!Define.IsILRuntime)
                {
                    Log.Error("mono层是mono模式, 但是Hotfix层是ILRuntime模式");
                }
#else
                if (Define.IsILRuntime)
                {
                    Log.Error("mono层是ILRuntime模式, Hotfix层是mono模式");
                }
#endif

                if (isInit)
                {
                    return;
                }
                isInit = true;

                Game.Scene.ModelScene = ETModel.Game.Scene;

                // 注册热更层回调
                ETModel.Game.Hotfix.Update = () => { Update(); };
                ETModel.Game.Hotfix.LateUpdate = () => { LateUpdate(); };
                ETModel.Game.Hotfix.OnApplicationQuit = () => { OnApplicationQuit(); };

                Screen.sleepTimeout = SleepTimeout.NeverSleep;//添加息屏

                // 启动公用单例
                await CommonSystemHelper.Init();

                #region 准备完成了 启动Login
                // Game.EventSystem.Run(EventIdType.UILogin);
                await UIRouterHelper.LoadSceneRoutine(UIType.UILogin);
                // 关闭热更新界面
                ETModel.Game.Scene.GetComponent<ETModel.UIComponent>().Remove(ETModel.UIType.UIUpdateBundle);
                #endregion
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void Update()
        {
            try
            {
                Game.EventSystem.Update();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void LateUpdate()
        {
            try
            {
                Game.EventSystem.LateUpdate();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void OnApplicationQuit()
        {
            Game.Close();
        }
    }
}
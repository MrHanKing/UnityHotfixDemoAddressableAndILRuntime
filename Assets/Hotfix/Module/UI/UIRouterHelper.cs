using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    public enum RouterStatus { NONE, LOADING }
    // HUB作为中继器，当不知道去向何方时，Hub会做选择
    public enum RouterType
    {
        // 回退到上一个界面
        BACK,
        // 前进
        FORWARD,
        // 刷新
        RELOAD,
        HUB
    }
    public enum TransType { NONE, UITransitions } // 转场类型
    public class UIRouterTask : CustomTaskRun<bool>
    {
        public override void Handle(bool output)
        {
            // UIRouter啥也不用做
        }

        public override void PreSend(TaskInput body = null)
        {
            // UIRouter啥也不用做
        }
    }
    /// <summary>
    /// 一级界面的转换调用类
    /// 二级界面使用UIManagerHelp.
    /// </summary>
    public static class UIRouterHelper
    {
        /// <summary>
        /// 上个场景名字
        /// </summary>
        /// <value></value>
        public static string previousSceneName { get; private set; }
        /// <summary>
        /// 当前场景名字
        /// </summary>
        /// <value></value>
        public static string currSceneName { get; private set; }

        /// <summary>
        /// 过度类型
        /// </summary>
        // private static RouterTrans trans;
        //private AsyncOperation operation;

        private static List<string> sceneHistory = new List<string>();

        /// <summary>
        /// 转换器当前状态
        /// </summary>
        /// <value></value>
        public static RouterStatus status { get; private set; }

        private static List<string> cashUI = new List<string>();


        // private static UIRouterTask source;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="sceneName"> 热更层第一个界面</param>
        public static void CheckInit()
        {
            if (sceneHistory.Count <= 0)
            {
                // 热更层第一个场景永远是登陆界面
                sceneHistory.Add(UIType.UILogin);
                previousSceneName = UIType.UILogin;
                if (string.IsNullOrWhiteSpace(currSceneName))
                {
                    currSceneName = UIType.UILogin;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sceneName">场景名字 UIType</param>
        /// <param name="routerType">专场类型，前进-重载-后退</param>
        /// <returns>false 重复调用 </returns>
        public static async Task<bool> LoadSceneRoutine(string sceneName = "", TransType transType = TransType.NONE, RouterType routerType = RouterType.FORWARD, bool saveUs = false)
        {
            if (status == RouterStatus.LOADING)
            {
                return false;
            }

            CheckInit();

            string needRemoveSceneName = currSceneName;

            switch (routerType)
            {
                case RouterType.FORWARD:
                    previousSceneName = currSceneName;
                    currSceneName = sceneName;
                    if (!string.IsNullOrWhiteSpace(previousSceneName))
                    {
                        sceneHistory.Add(previousSceneName);
                    }
                    break;
                case RouterType.BACK:
                    if (sceneHistory.Count > 0)
                    {
                        currSceneName = sceneHistory.PopAt(sceneHistory.Count - 1);
                        previousSceneName = sceneHistory.LastOrDefault();
                    }
                    break;
                case RouterType.RELOAD:
                    // 啥都不用变
                    break;
            }

            status = RouterStatus.LOADING;

            try
            {
                //结束所有引导
                // CommonSystemHelper.uIFinger?.CloseAllFinger();
            }
            catch (System.Exception)
            {
                Debug.LogError("强制结束了引导手们");
            }

            try
            {
                // 过场动画
                if (transType == TransType.UITransitions)
                {
                    // var transition = await UITransitions.GetInstance();
                    // await transition.TransitionsIn();
                }

                // 先启用新的 防止老的没过度 黑屏
                if (!string.IsNullOrWhiteSpace(currSceneName))
                {
                    if (cashUI.FindIndex(v => v == currSceneName) >= 0)
                    {
                        // 什么都不用做
                    }
                    else
                    {
                        var result = await CommonApiHelper.Create(currSceneName);

                        // var com = result.GetComponent<AsyncInitComponent>();
                        var coms = result.GetComponents();
                        foreach (var item in coms)
                        {
                            if (item is AsyncInitComponent com)
                            {
                                /// <summary>
                                /// 异步UI会额外初始化
                                /// </summary>
                                /// <returns></returns>
                                await com.Init();
                            }
                        }
                    }
                }

                // 删除旧的
                if (!string.IsNullOrWhiteSpace(needRemoveSceneName) && needRemoveSceneName != currSceneName)
                {
                    if (saveUs)
                    {
                        cashUI.Add(needRemoveSceneName);
                    }
                    else
                    {
                        Log.Info("退出清理了界面:" + needRemoveSceneName);
                        CommonApiHelper.Remove(needRemoveSceneName);
                        cashUI.RemoveAll(v => v == needRemoveSceneName);
                    }
                }

                // 过场动画关闭
                if (transType == TransType.UITransitions)
                {
                    // var transition = await UITransitions.GetInstance();
                    // await transition.TransitionsOut();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("切换场景失败:" + e);
            }

            status = RouterStatus.NONE;

            return true;
        }

        /// <summary>
        /// 清理路由所有缓存
        /// </summary>
        public static void Clear()
        {
            currSceneName = null;
            previousSceneName = null;
            cashUI.Clear();
            sceneHistory.Clear();
        }
    }

    static class ListExtension
    {
        public static T PopAt<T>(this List<T> list, int index)
        {
            T r = list[index];
            list.RemoveAt(index);
            return r;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.Networking;

namespace ETHotfix
{
    /// <summary>
    /// 热更进程的返回结果
    /// </summary>
    public class AddressableDownloadSystemTaskOutput : TaskOutput
    {
        /// <summary>
        /// 下载结果
        /// </summary>
        public bool result;
        /// <summary>
        /// 课程名字
        /// </summary>
        public string courseName;
    }

    /// <summary>
    /// 热更层 下载单个课程
    /// </summary>
    public class AddressableDownloadSystem
    {
        /// <summary>
        /// 多个课程可以多次启动
        /// </summary>
        private static Dictionary<string, MineTaskRun<AddressableDownloadSystemTaskOutput>> allDowns = new Dictionary<string, MineTaskRun<AddressableDownloadSystemTaskOutput>>();

        /// <summary>
        /// 获取标签名字
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        private static string GetLabelByCourseName(string courseName)
        {
            return courseName;
        }
        /// <summary>
        /// 关卡地图 特殊包
        /// </summary>
        public static string levelMapBundleLabel = "LevelMapLabel";

        /// <summary>
        /// 获取课程下载包的大小
        /// </summary>
        /// <param name="courseName"></param>
        /// <returns></returns>
        public static async Task<float> GetDownLoadSize(string courseName)
        {
            var label = GetLabelByCourseName(courseName);
            var result = await AddressablesHelper.GetDownLoadOneSize(label);
            return result;
        }

        public static async Task<CheckForCatalogUpdatesOutput> CheckForCatalogUpdates()
        {
            CheckForCatalogUpdatesOutput result = await AddressablesHelper.CheckForCatalogUpdates();
            return result;
        }
        public static async Task<UpdateCatalogsOutput> UpdateCatalogs(List<string> catalogs, string clearOldKey)
        {
            var result = await AddressablesHelper.UpdateCatalogs(catalogs, clearOldKey);
            return result;
        }
        /// <summary>
        /// 下载某个课程
        /// </summary>
        /// <param name="courseName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task<AddressableDownloadSystemTaskOutput> DownLoadCourse(string courseName, Action<float, bool> action)
        {
            MineTaskRun<AddressableDownloadSystemTaskOutput> currentDown = null;
            allDowns.TryGetValue(courseName, out currentDown);
            if (currentDown != null && currentDown.isValid)
            {
                // 已存在 继续等待旧人物
                return currentDown.GetTask();
            }

            var newTask = new MineTaskRun<AddressableDownloadSystemTaskOutput>();
            allDowns.Add(courseName, newTask);

            var label = GetLabelByCourseName(courseName);
            AddressablesHelper.DownloadOneByAddressable(label, action).ContinueWith((resultTask) =>
            {
                Log.Info($"{courseName} 课程下载完成");
                allDowns.Remove(courseName);
                newTask.TryResolveCurrentTask(new AddressableDownloadSystemTaskOutput() { result = resultTask.Result, courseName = courseName });
            });

            return newTask.Run();
        }

        // private static async void RunDownload(string labelName, Action<float, bool> action, MineTaskRun<AddressableDownloadSystemTaskOutput> taskRunner)
        // {
        //     var result = await BundleHelper.DownloadOneByAddressable(labelName, action);
        //     taskRunner?.TryResolveCurrentTask()
        // }
    }
}

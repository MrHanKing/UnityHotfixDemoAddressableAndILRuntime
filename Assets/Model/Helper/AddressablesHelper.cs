using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace UnityEngine.AddressableAssets
{
    /// <summary>
    /// Addressable的帮助类
    /// </summary>
    public static class MineAddressableHelper
    {
        private static object EvaluateKey(object obj)
        {
            if (obj is IKeyEvaluator)
                return (obj as IKeyEvaluator).RuntimeKey;
            return obj;
        }
        /// <summary>
        /// 收集热更新前可能会删除的资源或label的匹配的资源列表
        /// 注意 需要在updateCatalog之前收集 否则收集到的是新的寻址
        /// </summary>
        /// <returns></returns>
        public static List<AssetBundleRequestOptions> CollectNeedRemoveAssetBundle(string key)
        {
            List<AssetBundleRequestOptions> result = new List<AssetBundleRequestOptions>();
            var loactions = CollectResourceLocationByKey(key);
            foreach (var dep in GatherDependenciesFromLocations(loactions))
            {
                if (dep.Data is AssetBundleRequestOptions)
                    result.Add(dep.Data as AssetBundleRequestOptions);
            }
            return result;
        }

        public static bool ClearAllCachedssetBundleRequest(List<AssetBundleRequestOptions> inputList)
        {
            bool result = true;
            foreach (var item in inputList)
            {
                result = result && Caching.ClearAllCachedVersions(item.BundleName);
            }


            return result;
        }

        /// <summary>
        /// 收集依赖
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static List<IResourceLocation> GatherDependenciesFromLocations(IList<IResourceLocation> locations)
        {
            var locHash = new HashSet<IResourceLocation>();
            foreach (var loc in locations)
            {
                if (loc.ResourceType == typeof(IAssetBundleResource))
                {
                    locHash.Add(loc);
                }
                if (loc.HasDependencies)
                {
                    foreach (var dep in loc.Dependencies)
                        if (dep.ResourceType == typeof(IAssetBundleResource))
                            locHash.Add(dep);
                }
            }
            return new List<IResourceLocation>(locHash);
        }

        /// <summary>
        /// 收集资源或label的匹配的寻址器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IList<IResourceLocation> CollectResourceLocationByKey(string key)
        {
            IList<IResourceLocation> locations;
            GetResourceLocationsByKey(key, typeof(object), out locations);
            return locations;
        }

        /// <summary>
        /// 收集匹配的寻址器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static bool GetResourceLocationsByKey(object key, Type type, out IList<IResourceLocation> locations)
        {
            var m_ResourceLocators = Addressables.ResourceLocators;

            key = EvaluateKey(key);

            locations = null;
            HashSet<IResourceLocation> current = null;
            foreach (var locator in m_ResourceLocators)
            {
                IList<IResourceLocation> locs;
                if (locator.Locate(key, type, out locs))
                {
                    if (locations == null)
                    {
                        //simple, common case, no allocations
                        locations = locs;
                    }
                    else
                    {
                        //less common, need to merge...
                        if (current == null)
                        {
                            current = new HashSet<IResourceLocation>();
                            foreach (var loc in locations)
                                current.Add(loc);
                        }

                        current.UnionWith(locs);
                    }
                }
            }

            if (current == null)
                return locations != null;

            locations = new List<IResourceLocation>(current);
            return true;
        }
    }
}

namespace ETModel
{
    public class CheckForCatalogUpdatesOutput
    {
        public bool isSuccess;
        public List<string> catalogs;
    }
    public class UpdateCatalogsOutput
    {
        public bool isSuccess;
        public List<IResourceLocator> needUpdateSource;
    }

    public static class AddressablesHelper
    {
        private static bool isInit = false;
        /// <summary>
        /// 启动app一定要下载的资源分类
        /// </summary>
        public static string mainAppLabel = "Preload";

        public static async Task CheckInit()
        {
            if (!isInit)
            {
                await Addressables.InitializeAsync().Task;
                isInit = true;
            }
        }

        public static async Task<CheckForCatalogUpdatesOutput> CheckForCatalogUpdates()
        {
            await CheckInit();
            var handler = Addressables.CheckForCatalogUpdates(false);
            var catalogs = await handler.Task;
            var isSuccess = handler.Status == AsyncOperationStatus.Succeeded;

            Addressables.Release(handler);
            return new CheckForCatalogUpdatesOutput() { isSuccess = isSuccess, catalogs = catalogs };
        }

        public static async Task<UpdateCatalogsOutput> UpdateCatalogs(List<string> catalogs, string clearOldKey)
        {
            await CheckInit();
            var output = new UpdateCatalogsOutput() { isSuccess = true };
            var result = new List<IResourceLocator>();
            if (catalogs.Count > 0)
            {
                // 更新前清理clearOldKey对应的缓存 catalogs更新后冗余缓存有可能清理不掉 因为映射表变了
                if (!string.IsNullOrWhiteSpace(clearOldKey))
                {
                    try
                    {
                        await Addressables.ClearDependencyCacheAsync(clearOldKey, true).Task;
                    }
                    catch (System.Exception e)
                    {
                        // 可能因为部分操作权限没有会导致清理失败
                        Debug.Log($"清理缓存失败 error:{e}");
                    }
                }

                var handler = Addressables.UpdateCatalogs(catalogs, false);
                result = await handler.Task;
                output.isSuccess = handler.Status == AsyncOperationStatus.Succeeded;

                Addressables.Release(handler);
                Debug.Log("Catalogs 更新成功");
            }

            output.needUpdateSource = result;
            return output;
        }

        /// <summary>
        /// 获得下载大小
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<float> GetDownLoadOneSize(string key)
        {
            await CheckInit();

            var op = Addressables.GetDownloadSizeAsync(key);

            long size = await op.Task;

            Addressables.Release(op);
            return (float)size;
        }

        public static async Task<bool> DownloadOneByAddressable(string key, Action<float, bool> action)
        {
            try
            {
                await CheckInit();
                AsyncOperationHandle operationHandle = Addressables.DownloadDependenciesAsync(key, false);
                action?.Invoke(0, true);
                while (!operationHandle.IsDone)
                {
                    // Todo 错误检查
                    var downStatus = operationHandle.GetDownloadStatus();
                    Debug.Log("ope down size:" + downStatus.DownloadedBytes + " | " + downStatus.TotalBytes);
                    // operationHandle.PercentComplete会包含缓存 有问题 用下载值算才正确
                    var percent = downStatus.TotalBytes <= 0 ? operationHandle.PercentComplete : ((float)downStatus.DownloadedBytes / (float)downStatus.TotalBytes);
                    action?.Invoke(percent, false);
                    await Task.Delay(100);
                }
                Debug.Log("result status:" + operationHandle.Status);

                if (operationHandle.Status == AsyncOperationStatus.Failed)
                {
                    Addressables.Release(operationHandle);
                    return false;
                }

                action?.Invoke(1, true);
                Addressables.Release(operationHandle);
            }
            catch (System.Exception e)
            {
                Debug.LogError("更新失败 :" + e);
                return false;
            }

            return true;
        }

        public static async Task<bool> DownloadMainBundleByAddressable(Action<float, bool> action)
        {
            Debug.Log("开始下载");
            return await DownloadOneByAddressable(mainAppLabel, action);
        }

        public static async Task TestDownLoad()
        {
            await CheckInit();

            Debug.Log("TestDownLoad");
            var target = Addressables.ResourceLocators;
            foreach (var item in target)
            {
                foreach (var key in item.Keys)
                {
                    Debug.Log("try down:" + key);
                }
                var sizeHandle = Addressables.GetDownloadSizeAsync(item.Keys);
                await sizeHandle.Task;
                if (sizeHandle.Result > 0)
                {
                    Debug.Log("need down:" + sizeHandle.Result);
                }
                Addressables.Release(sizeHandle);
            }
        }

        public static async Task TestDownLoad1()
        {
            await CheckInit();
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            await checkHandle.Task;
            if (checkHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var result = checkHandle.Result;
            }
            else
            {
                Debug.Log(checkHandle.Status);
            }

            Addressables.Release(checkHandle);
        }
    }
}

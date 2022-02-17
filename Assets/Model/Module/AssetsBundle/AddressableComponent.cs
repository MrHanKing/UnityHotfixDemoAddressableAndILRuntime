
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ETModel
{
    [ObjectSystem]
    public class AddressableAwakeSystem : AwakeSystem<AddressableComponent>
    {
        public override void Awake(AddressableComponent self) { self.Awake(); }
    }

    public class AddressableComponent : Component
    {
        public static AddressableComponent Instance;

        private Dictionary<string, Dictionary<string, List<UnityEngine.Object>>>
                _sublevelDic = new Dictionary<string, Dictionary<string, List<UnityEngine.Object>>>();

        public void Awake() { Instance = this; }

        /// <summary>
        /// 仅用在Load子级资源
        /// </summary>
        /// <param name="type">父级类型</param>
        /// <param name="url">url</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public async Task<T> LoadSublevelAsset<T>(string type, string url) where T : UnityEngine.Object
        {
            Dictionary<string, List<UnityEngine.Object>> dic;
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            UnityEngine.Object obj;
            try
            {
                if (this._sublevelDic.ContainsKey(type))
                {
                    dic = this._sublevelDic[type];

                    if (dic.ContainsKey(url))
                    {
                        objects = dic[url];

                        if (objects.Count > 0)
                        {
                            obj = objects[0];
                        }
                        else
                        {
                            obj = await Addressables.LoadAssetAsync<T>(url).Task;
                            objects.Add(obj);
                        }

                        dic[url] = objects;
                    }
                    else
                    {
                        obj = await Addressables.LoadAssetAsync<T>(url).Task;
                        objects.Add(obj);
                        if (!dic.ContainsKey(url)) { dic.Add(url, objects); }
                    }
                }
                else
                {
                    dic = new Dictionary<string, List<UnityEngine.Object>>();
                    obj = await Addressables.LoadAssetAsync<T>(url).Task;
                    objects.Add(obj);
                    dic.Add(url, objects);
                    if (!this._sublevelDic.ContainsKey(type)) { this._sublevelDic.Add(type, dic); }
                }
            }
            catch (Exception e)
            {
                Log.Error("?????资源有问题" + e.ToString());
                throw;
            }

            return (T)obj;
        }
        /// <summary>
        /// 同步加载 性能不高 建议全用异步
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSublevelAssetSync<T>(string type, string url) where T : UnityEngine.Object
        {
            Dictionary<string, List<UnityEngine.Object>> dic;
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            UnityEngine.Object obj;
            try
            {
                if (this._sublevelDic.ContainsKey(type))
                {
                    dic = this._sublevelDic[type];

                    if (dic.ContainsKey(url))
                    {
                        objects = dic[url];

                        if (objects.Count > 0)
                        {
                            obj = objects[0];
                        }
                        else
                        {
                            var op = Addressables.LoadAssetAsync<T>(url);
                            obj = op.WaitForCompletion();
                            objects.Add(obj);
                            // Addressables.Release(op);
                        }

                        dic[url] = objects;
                    }
                    else
                    {
                        var op = Addressables.LoadAssetAsync<T>(url);
                        obj = op.WaitForCompletion();
                        objects.Add(obj);
                        if (!dic.ContainsKey(url)) { dic.Add(url, objects); }
                        // Addressables.Release(op);
                    }
                }
                else
                {
                    dic = new Dictionary<string, List<UnityEngine.Object>>();
                    var op = Addressables.LoadAssetAsync<T>(url);
                    obj = op.WaitForCompletion();
                    objects.Add(obj);
                    dic.Add(url, objects);
                    if (!this._sublevelDic.ContainsKey(type)) { this._sublevelDic.Add(type, dic); }
                    // Addressables.Release(op);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }

            return (T)obj;
        }

        /// <summary>
        /// 同步加载 性能不高 建议全用异步 
        /// 用type和url组合作为名字读取
        /// </summary>
        /// <param name="type"></param>
        /// <param name="url"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadSublevelAssetSyncUsedTypeToSearch<T>(string type, string url) where T : UnityEngine.Object
        {
            var resultUrl = type + "/" + url;
            return LoadSublevelAssetSync<T>(type, resultUrl);
        }


        /// <summary>
        /// 仅用在Load子级资源
        /// </summary>
        /// <param name="url">url</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <typeparam name="R">父级类型</typeparam>
        /// <returns></returns>
        public async Task<T> LoadSublevelAsset<T, R>(string url) where T : UnityEngine.Object where R : Component
        {
            return await this.LoadSublevelAsset<T>(typeof(R).Name, url);
        }

        /// <summary>
        /// 仅用在实例化子级资源
        /// </summary>
        /// <param name="type">父级类型</param>
        /// <param name="url">url</param>
        /// <param name="transform">父级transform</param>
        /// <returns></returns>
        public async Task<GameObject> InstantiateSublevelAsync(string type, string url, Transform transform)
        {
            Dictionary<string, List<UnityEngine.Object>> dic;
            List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
            UnityEngine.Object obj;

            try
            {
                if (this._sublevelDic.ContainsKey(type))
                {
                    dic = this._sublevelDic[type];

                    if (dic.ContainsKey(url))
                    {
                        objects = dic[url];

                        obj = await Addressables.InstantiateAsync(url, transform).Task;
                        objects.Add(obj);
                    }
                    else
                    {
                        obj = await Addressables.InstantiateAsync(url, transform).Task;
                        objects.Add(obj);
                        dic.Add(url, objects);
                    }
                }
                else
                {
                    dic = new Dictionary<string, List<UnityEngine.Object>>();
                    obj = await Addressables.InstantiateAsync(url, transform).Task;
                    objects.Add(obj);
                    dic.Add(url, objects);
                    if (!this._sublevelDic.ContainsKey(type)) { this._sublevelDic.Add(type, dic); }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                throw;
            }

            return (GameObject)obj;
        }

        /// <summary>
        /// 仅用在实例化子级资源
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="transform">父级transform</param>
        /// <typeparam name="T">父级类型</typeparam>
        /// <returns></returns>
        public async Task<GameObject> InstantiateSublevelAsync<T>(string url, Transform transform) where T : Component
        {
            return await this.InstantiateSublevelAsync(typeof(T).Name, url, transform);
        }

        public void ReleaseSublevel(string type)
        {
            Dictionary<string, List<UnityEngine.Object>> dic;
            this._sublevelDic.TryGetValue(type, out dic);

            if (dic == null) { return; }

            foreach (List<UnityEngine.Object> objects in dic.Values)
            {
                foreach (UnityEngine.Object obj in objects)
                {
                    if (obj is GameObject gameObject)
                    {
                        Log.Debug($"Release GameObject = {gameObject.name}");
                        Addressables.ReleaseInstance(gameObject);
                        continue;
                    }
                    Log.Debug($"Release obj = {obj?.name}, from {type}");
                    Addressables.Release(obj);
                }
            }

            dic.Clear();
            this._sublevelDic.Remove(type);
        }

        /// <summary>
        /// 随父级释放所有资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ReleaseSublevel<T>() where T : Component { this.ReleaseSublevel(typeof(T).Name); }
    }
}

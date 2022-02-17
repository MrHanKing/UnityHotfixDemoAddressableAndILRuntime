using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ETModel
{
    public class GlobalConfigType
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 热更地址
        /// </summary>
        public string hotfixUrl { get; set; }
        /// <summary>
        /// 打点地址
        /// </summary>
        public string statisticsUrl { get; set; }
        /// <summary>
        /// socket连接地址
        /// </summary>
        public string wsPath { get; set; }
        /// <summary>
        /// http主地址
        /// </summary>
        /// <value></value>
        public string httpRootUrl { get; set; }

        /// <summary>
        /// 需要的客户端版本号 如果大就需要跳转链接
        /// </summary>
        public string needClientVersion { get; set; }
        /// <summary>
        /// ios app更新地址
        /// </summary>
        public string iosUrl { get; set; }
        /// <summary>
        /// 安卓 app更新地址
        /// </summary>
        public string androidUrl { get; set; }
        /// <summary>
        /// window or macos app更新地址
        /// </summary>
        public string pcUrl { get; set; }


        #region 预留bool值备用
        public string hotfixJson;
        public bool bool1 { get; set; }
        public bool bool2 { get; set; }
        public bool bool3 { get; set; }
        public bool bool4 { get; set; }
        public bool bool5 { get; set; }
        public bool bool6 { get; set; }
        # endregion
    }

    [ObjectSystem]
    public class GlobalConfigComponentAwakeSystem : AwakeSystem<GlobalConfigComponent>
    {
        public override void Awake(GlobalConfigComponent t)
        {
            t.Awake();
        }
    }

    public class GlobalConfigComponent : Component
    {
        public static GlobalConfigComponent Instance;
        // 配置的远程配置路径 唯一写死的网络配置的路径 不可热更 
        private static string configUrl = "";
        private string playerPrefsKey = "GlobalConfig";
        /// <summary>
        /// 平台
        /// </summary>
        public static string platform = "ios";
        public GlobalConfigType defaultConfig { get; private set; } = new GlobalConfigType()
        {
#if UNITY_EDITOR
            version = "1.0.0",
            hotfixUrl = "http://10.1.2.161:8080",
            statisticsUrl = "",
            wsPath = "",
            httpRootUrl = "",
            needClientVersion = "1.0",
            iosUrl = "",
            androidUrl = "",
            pcUrl = "",
#else
            version = "1.0.0",
            hotfixUrl = "",
            statisticsUrl = "",
            wsPath = "",
            httpRootUrl = "",
            needClientVersion = "1.0",
            iosUrl = "",
            androidUrl = "",
            pcUrl = "",
#endif
        };
        /// <summary>
        /// 固化版本号
        /// </summary>
        /// <value></value>
        public string originVersion { get; private set; }

        public void Awake()
        {
            Instance = this;
        }

        public async Task Run()
        {
            try
            {
                // 直接拿内部固化的客户端版本
                originVersion = this.defaultConfig.needClientVersion;

                UIUpdateBundleComponent.Instance?.DownConfig();

                var localConfig = this.GetLocalConfig();
                if (localConfig != null && !this.CheckVersion(this.defaultConfig.version, localConfig.version))
                {
                    this.defaultConfig = localConfig;
                }

                GlobalConfigType result = await this.RequestGlobalConfig();
                if (result != null && !this.CheckVersion(this.defaultConfig.version, result.version))
                {
                    // 先检查是否有大版本
                    var updateUrl = this.GetUpdateUrl(result);
                    await this.CheckUpdateApp(originVersion, result.needClientVersion, updateUrl);
                    // 没有大版本更新才存储下载的配置
                    this.defaultConfig = result;
                }

                Debug.Log("ResultGlobalConfig:" + JsonHelper.ToJson(this.defaultConfig));
            }
            catch (System.Exception e)
            {

                Debug.LogError("ResultGlobalConfig Error:" + e);
            }
            finally
            {
                this.SaveLocalConfig();
                this.UpdateStreamAssetsLocalData();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fixedVersion">本地固化版本</param>
        /// <param name="remoteVersion">远程需求客户端版本</param>
        /// <returns></returns>
        private Task<bool> CheckUpdateApp(string fixedVersion, string remoteVersion, string updateUrl)
        {
            var task = new TaskCompletionSource<bool>();
            if (!this.CheckVersion(fixedVersion, remoteVersion))
            {

                // 需要更新 弹出窗口 永不往下走
                UIUpdateBundleComponent.Instance?.ShowUpdateUrl(updateUrl);
                return task.Task;
            }
            return Task.FromResult<bool>(true);
        }

        /// <summary>
        /// 获得平台更新url
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private string GetUpdateUrl(GlobalConfigType result)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return result.androidUrl;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return result.iosUrl;
            }

            return result.pcUrl;
        }

        private GlobalConfigType GetLocalConfig()
        {
            string str = PlayerPrefs.GetString(this.playerPrefsKey);
            var result = JsonHelper.FromJson<GlobalConfigType>(str);
            if (result != null)
            {
                return result;
            }

            return null;
        }

        private void SaveLocalConfig()
        {
            PlayerPrefs.SetString(this.playerPrefsKey, JsonHelper.ToJson(this.defaultConfig));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originVersion"></param>
        /// <param name="remoteVersion"></param>
        /// <returns>true 不需要版本更新 </returns>
        private bool CheckVersion(string originVersion, string remoteVersion)
        {
            Version originV = new Version(originVersion);
            Version remoteV = new Version(remoteVersion);
            var result = originV.CompareTo(remoteV);
            return result >= 0;
        }


        private async Task<GlobalConfigType> RequestGlobalConfig()
        {
            GlobalConfigType result = null;
            using (UnityWebRequest request = UnityWebRequest.Get(configUrl))
            {
                try
                {
                    request.timeout = 15;
                    await request.SendWebRequest();

                    string str = request.downloadHandler.text;
                    Debug.Log("RequestGlobalConfig:" + str);
                    result = JsonHelper.FromJson<GlobalConfigType>(str);
                }
                catch (System.Exception e)
                {
                    Log.Error("request error:" + e);
                }
            }

            return result;
        }

        /// <summary>
        /// 运行时读取StreamAssets内置固化数据 最后执行 不存储 不更新
        /// </summary>
        private async void UpdateStreamAssetsLocalData()
        {
            try
            {
                string versionPath = Path.Combine(PathHelper.AppResPath4Web, "AppAddConfig.json");

                using (UnityWebRequest request = UnityWebRequest.Get(versionPath))
                {
                    await request.SendWebRequest();

                    var streamingVersionConfig = JsonHelper.FromJson<AppAddConfig>(request.downloadHandler.text);
#if UNITY_ANDROID
                    if (streamingVersionConfig != null)
                    {
                        GlobalConfigComponent.platform = streamingVersionConfig.androidPlatform;
                        Debug.Log("安卓平台设置成功: " + GlobalConfigComponent.platform);
                    }
#endif
                }
            }
            catch (System.Exception)
            {

                Debug.Log("加载本地固化配置失败, 某些包里面可能不存在这个文件");
            }
        }
    }

    /// <summary>
    /// app 额外附加信息
    /// </summary>
    public class AppAddConfig
    {
        public string androidPlatform;
    }
}

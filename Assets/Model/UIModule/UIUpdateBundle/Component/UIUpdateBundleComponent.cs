
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine.Video;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ETModel
{
    [ObjectSystem]
    public class UIUpdateBundleComponentSystem : AwakeSystem<UIUpdateBundleComponent>
    {
        public override void Awake(UIUpdateBundleComponent self)
        {
            self.Awake();
        }
    }

    public class UIUpdateBundleComponent : Component
    {
        private UI uiRoot;

        private TextMeshProUGUI loading;
        private Image tiao;

        public static UIUpdateBundleComponent Instance = null;

        CancellationTokenSource TokenSource;

        public Transform bg;

        # region 强制更新UI
        private RectTransform updateAppRoot;
        private TextMeshProUGUI updateInfo;
        private UIButtonMine updateBtn;
        private TextMeshProUGUI updateBtnText;
        private string updateUrl = "";
        #endregion

        # region 更新失败UI
        private RectTransform updateFailedRoot;
        private TextMeshProUGUI updateFailedInfo;
        private UIButtonMine updateFailedBtn;
        private TextMeshProUGUI updateFailedBtnText;
        #endregion

        public void Awake()
        {
            uiRoot = this.GetParent<UI>();
            Instance = this;
            bg = uiRoot.GameObject.Get<GameObject>("bg").transform;
            loading = uiRoot.GameObject.Get<GameObject>("Loading").GetComponent<TextMeshProUGUI>();
            tiao = uiRoot.GameObject.Get<GameObject>("tiao").GetComponent<Image>();

            this.updateAppRoot = uiRoot.GameObject.Get<GameObject>("UpdateVersion").GetComponent<RectTransform>();
            this.updateInfo = this.updateAppRoot.Find("Text").GetComponent<TextMeshProUGUI>();
            this.updateBtn = this.updateAppRoot.Find("CommonButton").GetComponent<UIButtonMine>();
            this.updateBtn.onClick.AddListener(this.OnUpdateVersionClick);
            this.updateBtnText = this.updateBtn.transform.Find("Next_Ch").GetComponent<TextMeshProUGUI>();
            this.updateAppRoot.gameObject.SetActive(false);

            this.updateFailedRoot = uiRoot.GameObject.Get<GameObject>("UpdateFailed").GetComponent<RectTransform>();
            this.updateFailedInfo = this.updateFailedRoot.Find("Text").GetComponent<TextMeshProUGUI>();
            this.updateFailedBtn = this.updateFailedRoot.Find("CommonButton").GetComponent<UIButtonMine>();
            this.updateFailedBtn.onClick.AddListener(this.OnUpdateFailedBtnClick);
            this.updateFailedBtnText = this.updateFailedBtn.transform.Find("Next_Ch").GetComponent<TextMeshProUGUI>();
            this.updateFailedRoot.gameObject.SetActive(false);

            ViewLoading();
        }

        /// <summary>
        /// 跳转商店更新链接
        /// </summary>
        private void OnUpdateVersionClick()
        {
            if (!string.IsNullOrWhiteSpace(this.updateUrl))
            {
                Application.OpenURL(this.updateUrl);
            }
        }
        /// <summary>
        /// 更新失败了
        /// </summary>
        private void OnUpdateFailedBtnClick()
        {
            var oldTask = this.tryDownAgainTask;
            this.tryDownAgainTask = null;

            oldTask?.TrySetResult(true);
        }

        private float oldPercentPoint = 0;
        /// <summary>
        /// 更新进度
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="isStart">是否是某一个阶段的重新开始</param>
        public void UpdatePro(float percent, bool isStart = false)
        {
            if (isStart)
            {
                oldPercentPoint = tiao.fillAmount;
            }
            else
            {
                Debug.Log("下载进度:" + percent);
                var result = Mathf.Lerp(oldPercentPoint, 1, percent);

                tiao.fillAmount = result;

            }
        }

        public void LoadFinished()
        {
            FixedFrameRate();

            tiao.fillAmount = 1;
        }

        public void FixedFrameRate()
        {
            Application.targetFrameRate = 30;
        }

        /// <summary>
        /// 强制更新界面
        /// </summary>
        /// <param name="url"></param>
        public void ShowUpdateUrl(string url)
        {
            this.updateUrl = url;
            var isShowBtn = !string.IsNullOrWhiteSpace(url);

            this.updateAppRoot.gameObject.SetActive(true);
            this.updateBtn.gameObject.SetActive(isShowBtn);

            this.updateInfo.text = "Please go to the App Store to update the App";
            this.updateBtnText.text = "OK";
        }

        public void RefreshUpdateFailed()
        {
            this.updateFailedInfo.text = "Update failed. Please check your network.";
            this.updateFailedBtnText.text = "OK";
        }

        public async Task Play()
        {
            // 显示下载
            this.ViewLoading();
            // 下载主app的资源包
            var downResult = false;
            while (!downResult)
            {
                downResult = await this.DownLoadMainBundles();
                if (!downResult)
                {
                    // 失败逻辑处理 弹出窗口 等待确认
                    await this.ShowUpdateFailedRoot();
                }
            }

            this.ViewLoadingCode();

            await Game.Hotfix.LoadHotfixAssembly();

            Game.Scene.AddComponent<MusicComponent>();

            this.ViewLoadingStart();
            this.LoadFinished();

            Game.Hotfix.GotoHotfix();
        }

        /// <summary>
        /// 下载启动app的主要依赖内容 分包内容不下载
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DownLoadMainBundles()
        {
            try
            {
                // 初始化必须 提早初始化系统
                await AddressablesHelper.CheckInit();

                if (string.IsNullOrWhiteSpace(GlobalConfigComponent.Instance.defaultConfig.hotfixUrl))
                {
                    // 没有热更地址 不用下载任何东西
                    return true;
                }
                // 劫持热更地址
                AddressableTools.SetInternalIdTransform();

                // 检查catalog是否更新
                this.UpdatePro(0, true);
                CheckForCatalogUpdatesOutput updateCatalogs = null;
                var resultCheckForCatalog = false;
                while (!resultCheckForCatalog)
                {
                    updateCatalogs = await AddressablesHelper.CheckForCatalogUpdates();
                    resultCheckForCatalog = updateCatalogs.isSuccess;
                    if (!resultCheckForCatalog)
                    {
                        // 失败逻辑处理 弹出窗口 等待确认
                        await this.ShowUpdateFailedRoot();
                    }
                }
                this.UpdatePro(0.1f);

                // 更新catalog以及清理旧缓存
                this.UpdatePro(0, true);
                var resultUpdateCatalogs = false;
                while (!resultUpdateCatalogs)
                {
                    var output = await AddressablesHelper.UpdateCatalogs(updateCatalogs.catalogs, AddressablesHelper.mainAppLabel);
                    resultUpdateCatalogs = output.isSuccess;
                    if (!resultUpdateCatalogs)
                    {
                        // 失败逻辑处理 弹出窗口 等待确认
                        await this.ShowUpdateFailedRoot();
                    }
                }
                this.UpdatePro(0.2f);

                // await BundleHelper.DownloadBundle();
                var size = await AddressablesHelper.GetDownLoadOneSize(AddressablesHelper.mainAppLabel);
                // Debug.Log("下载资源大小: " + size / (1024f * 1024f) + "MB");
                // size = await BundleHelper.GetDownLoadOneSize("Preload");
                // Debug.Log("下载资源大小: " + size / (1024f * 1024f) + "MB");
                // size = await BundleHelper.GetDownLoadOneSize("Demo1");
                // Debug.Log("下载资源大小: " + size / (1024f * 1024f) + "MB");
                // size = await BundleHelper.GetDownLoadOneSize("LevelMapLabel");
                // Debug.Log("下载资源大小: " + size / (1024f * 1024f) + "MB");

                if (size <= 0)
                {
                    Debug.Log("没有需要下载的");
                    // 检查可能网络失败 没必要return 下面还有打点
                    // return;
                }

                Debug.Log("下载资源大小: " + size / (1024f * 1024f) + "MB");

                var result = false;
                while (!result)
                {
                    result = await AddressablesHelper.DownloadOneByAddressable(AddressablesHelper.mainAppLabel, this.UpdatePro);
                    //更新失败
                    if (!result)
                    {
                        // 失败逻辑处理 弹出窗口 等待确认
                        await this.ShowUpdateFailedRoot();
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("热更下载发生未知错误!!!");
                Debug.LogError(e);
                return false;
            }
            return true;
        }

        public async Task ShowUpdateFailedRoot()
        {
            this.RefreshUpdateFailed();
            this.updateFailedRoot.gameObject.SetActive(true);
            await this.CheckTryDownAgain();
            this.updateFailedRoot.gameObject.SetActive(false);
        }

        private TaskCompletionSource<bool> tryDownAgainTask;
        public Task<bool> CheckTryDownAgain()
        {
            this.tryDownAgainTask = new TaskCompletionSource<bool>();

            return this.tryDownAgainTask.Task;
        }

        /// <summary>
        /// 下载配置中
        /// </summary>
        public void DownConfig()
        {
            loading.text = "Connecting...";
        }

        public void ViewLoading()
        {
            loading.text = "Loading...";
        }
        private void ViewLoadingCode()
        {
            loading.text = "Version confirmation";
        }

        private void ViewLoadingStart()
        {
            loading.text = "StartGame";
        }


        public void Hide()
        {
            // ts.gameObject.SetActive(false);
            Instance = null;
            // Destroy(this.gameObject);
            TokenSource?.Cancel();
            TokenSource = null;
        }

        public override void Dispose()
        {
            this.Hide();
            Instance = null;
            TokenSource?.Cancel();
            TokenSource = null;
            base.Dispose();
        }
    }
}
using System;
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
    /// 热更层 webRequest 统一调用接口
    /// </summary>
    public class WebRequestSystem
    {
        /// Todo 挪到Global去
        private static string url
        {
            get
            {
                if (GlobalConfigComponent.Instance.defaultConfig.httpRootUrl != null)
                {
                    return GlobalConfigComponent.Instance.defaultConfig.httpRootUrl;
                }

                return "https://baidu.com";
            }
        }

        private static WebRequestErrorHandler errorHandler = new WebRequestErrorHandler();
        private static int timeOutTime = 15;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="json"></param>
        /// <param name="ignoreError">无视错误 阻断但不执行任何UI逻辑 例如打点</param>
        /// <returns></returns>
        private static async Task<UnityWebRequest> WebRequest(UnityWebRequest request, string json = null, bool ignoreError = false)
        {
            request = PreCreateData(request, json);
            await request.SendWebRequest();
            if (errorHandler != null)
            {
                // 检查统一的错误 部分错误会直接阻断
                await errorHandler.CheckRequestError(request, ignoreError);
            }
            return request;
        }

        public static async Task<DownloadHandler> AsyncGetWebRequest(string sub, WebRequestErrorHandlerParam otherParams = null)
        {
            var targetUrl = url + sub;
            var isEnd = false;
            UnityWebRequest request = null;
            while (!isEnd)
            {
                request = UnityWebRequest.Get(targetUrl);
                if (errorHandler != null)
                {
                    errorHandler.SendARequest(request, otherParams);
                }

                request = await WebRequest(request);

                if (errorHandler != null)
                {
                    isEnd = await errorHandler.TryResolvedARequest(request);
                }
                else
                {
                    Debug.LogWarning("没有网络异常的处理脚本");
                    isEnd = true;
                }
            }

            return request?.downloadHandler;
        }

        public static async Task<DownloadHandler> AsyncPostWebRequest(string sub, string json, bool ignoreError = false, WebRequestErrorHandlerParam otherParams = null)
        {
            Log.Info("AsyncPostWebRequest data:" + json);
            var targetUrl = url + sub;
            var isEnd = false;
            UnityWebRequest request = null;
            while (!isEnd)
            {
                request = UnityWebRequest.Post(targetUrl, "");
                if (errorHandler != null)
                {
                    errorHandler.SendARequest(request, otherParams);
                }

                request = await WebRequest(request, json, ignoreError);

                if (errorHandler != null)
                {
                    isEnd = await errorHandler.TryResolvedARequest(request);
                }
                else
                {
                    Debug.LogWarning("没有网络异常的处理脚本");
                    isEnd = true;
                }
            }

            return request?.downloadHandler;
        }
        public static async Task<DownloadHandler> AsyncPutWebRequest(string sub, string json, WebRequestErrorHandlerParam otherParams = null)
        {
            Log.Info("AsyncPutWebRequest data:" + json);
            var targetUrl = url + sub;
            var isEnd = false;
            UnityWebRequest request = null;
            while (!isEnd)
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                request = UnityWebRequest.Put(targetUrl, body);
                if (errorHandler != null)
                {
                    errorHandler.SendARequest(request, otherParams);
                }

                request = await WebRequest(request, json);

                if (errorHandler != null)
                {
                    isEnd = await errorHandler.TryResolvedARequest(request);
                }
                else
                {
                    Debug.LogWarning("没有网络异常的处理脚本");
                    isEnd = true;
                }
            }

            return request?.downloadHandler;
        }

        private static UnityWebRequest PreCreateData(UnityWebRequest request, string json = null)
        {
            // Todo 请求需要token
            string getToken = "";

            request.timeout = timeOutTime;
            request.SetRequestHeader("Content-Type", "application/json;charset=utf-8");

            if (getToken != null && !string.IsNullOrWhiteSpace(getToken))
            {

                request.SetRequestHeader("Authorization", "Bearer " + getToken);
                //request.SetRequestHeader("Authorization", "Bearer " + "foo");
            }
            else
            {
                request.SetRequestHeader("Authorization", "");
            }

            if (json != null)
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                request.uploadHandler = new UploadHandlerRaw(body);
            }
            return request;
        }


        /// <summary>
        /// 获取服务器图片
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public static async Task<Sprite> GetTextuer(string imageUrl)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
            Sprite sprite = null;
            await request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                sprite = null;
            }
            else
            {
                Texture2D texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;
                sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
            }
            return sprite;
        }
    }


}

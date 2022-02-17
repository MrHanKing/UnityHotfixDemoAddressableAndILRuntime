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
    public class WebRequestErrorServerMsg
    {
        public int code;
        public string msg;
    }
    /// <summary>
    /// 异常回调额外参数
    /// </summary>
    public class WebRequestErrorHandlerParam
    {
        /// <summary>
        /// 重新设置超时时间
        /// </summary>
        public float showTimeOut = 0;
        public WebRequestErrorHandlerParam(float showTimeOut)
        {
            this.showTimeOut = showTimeOut;
        }
    }

    /// <summary>
    /// 热更层 webRequest 异常处理
    /// </summary>
    public class WebRequestErrorHandler
    {
        /// <summary>
        /// 转圈圈计时器
        /// </summary>
        private TimeOutComponent timer;
        /// <summary>
        /// 存活着的消息
        /// </summary>
        /// <typeparam name="UnityWebRequest"></typeparam>
        /// <returns></returns>
        private List<UnityWebRequest> lifeRequest = new List<UnityWebRequest>();
        /// <summary>
        /// 在重连界面被阻塞的连接异步
        /// </summary>
        /// <returns></returns>
        private List<TaskCompletionSource<bool>> waitChoiseReq = new List<TaskCompletionSource<bool>>();
        private int tryConnectTime = 3;
        private int currentConnectTime = 0;

        private bool isShowWaitingMask = false;
        private bool isShowTryAgainChoise = false;
        /// <summary>
        /// 网络多久没返回就出现转菊花
        /// </summary>
        private const float showTimeOut = 1.5f;

        public WebRequestErrorHandler()
        {
            // 超时计时器
            this.timer = ComponentFactory.Create<TimeOutComponent>().SetData(showTimeOut);
            this.timer.OnTimeOutEvent += this.OnTimeOut;
        }

        public void SendARequest(UnityWebRequest request, WebRequestErrorHandlerParam otherParams)
        {
            this.lifeRequest.Add(request);
            if (!this.timer.GetRunningStatus())
            {
                if (otherParams != null && otherParams.showTimeOut > 0)
                {
                    // 嵌套请求的时候不会重制超时时间
                    this.timer.SetData(otherParams.showTimeOut);
                }
                else
                {
                    this.timer.SetData(showTimeOut);
                }

                this.timer.Startime();
            }
        }

        public void OnTimeOut()
        {
            // 检查是否已经有
            if (this.isShowTryAgainChoise || this.isShowWaitingMask)
            {
                return;
            }
            this.SetHttpWaitWin(true);
        }

        private void SetHttpWaitWin(bool status)
        {
            this.isShowWaitingMask = status;
            if (status)
            {
                // Todo 打开转菊花界面
            }
            else
            {
                // Todo 关闭转菊花界面
            }
        }

        /// <summary>
        /// 断线重连提示框
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ShowWaitNetErrorWin()
        {
            if (this.isShowTryAgainChoise)
            {
                var taskSource = new TaskCompletionSource<bool>();
                this.waitChoiseReq.Add(taskSource);

                return await taskSource.Task;
            }
            this.isShowTryAgainChoise = true;
            try
            {
                if (this.isShowWaitingMask)
                {
                    this.SetHttpWaitWin(false);
                }

                // Todo 统一断线重连处理
                // DelegateInputDataUIPropKuang input = new DelegateInputDataUIPropKuang(EUIPropKuangType.LinkIsFalse);
                // var result = await UIManagerHelper.ShowCommonPropKuang(input);
                // if (result.choise)
                // {
                //     // 尝试重连
                //     // 打开waiting界面
                //     this.SetHttpWaitWin(true);
                //     // 清理重连次数
                //     this.currentConnectTime = 0;
                //     // 唤起所有等待的重连 注意这里乱序 暂无顺序需求
                //     this.waitChoiseReq.ForEach(value => value.TrySetResult(false));
                //     this.waitChoiseReq.Clear();

                //     this.isShowTryAgainChoise = false;
                //     return false;
                // }
                // else
                // {
                //     // 退出平台
                //     Application.Quit();
                // }
            }
            finally
            {
                this.isShowTryAgainChoise = false;
            }
            return false;
        }

        /// <summary>
        /// true 成功 false 需要重连
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<bool> TryResolvedARequest(UnityWebRequest request)
        {
            // 请求一次性的 一定会清理 下次是新实例
            this.lifeRequest.Remove(request);

            if (request.isNetworkError || request.isHttpError)
            {
                Log.Info(request.url + " is isNetworkError" + " error code:" + request.responseCode + request.error);

                if (this.currentConnectTime <= this.tryConnectTime)
                {
                    //计数+1
                    this.currentConnectTime += 1;
                    return false;
                }

                // 网络错误弹窗 等待用户选择
                return await this.ShowWaitNetErrorWin();
            }

            this.CloseTargetRequest(request);
            return true;
        }

        /// <summary>
        /// 统一处理请求的错误
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task CheckRequestError(UnityWebRequest request, bool ignoreError = false)
        {
            // 处理errorCode
            if (request.responseCode == 401)
            {
                throw new Exception("token 过期了");
            }
            if (request.responseCode == 400)
            {
                throw new Exception("服务器返回了错误");
            }
            if (request.responseCode == 500)
            {
                throw new Exception("服务器返回了错误");
            }
        }

        /// <summary>
        /// 强制清理请求
        /// </summary>
        /// <param name="request"></param>
        public void CloseTargetRequest(UnityWebRequest request)
        {
            // 请求一次性的 一定会清理 下次是新实例
            this.lifeRequest.Remove(request);

            Log.Info(request.url + " is end request");
            if (this.waitChoiseReq.Count <= 0 && this.isShowWaitingMask)
            {
                this.SetHttpWaitWin(false);
            }
            this.timer?.StopTime();
        }
    }
}

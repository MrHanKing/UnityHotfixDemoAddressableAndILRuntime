using System;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class TimeOutComponentAwakeSystem : AwakeSystem<TimeOutComponent>
    {
        public override void Awake(TimeOutComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class TimeOutComponentUpdateSystem : UpdateSystem<TimeOutComponent>
    {
        public override void Update(TimeOutComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 计时器
    /// 组件作用 可以监测超时
    /// </summary>
    public class TimeOutComponent : Component
    {
        /// <summary>
        /// 计时器运行时间总和
        /// </summary>
        private float runningtime = 0;
        /// <summary>
        /// 超时时间
        /// </summary>
        private float timeOutTime;
        /// <summary>
        /// 超时了
        /// </summary>
        public event Action OnTimeOutEvent;
        /// <summary>
        /// 超时后是否停止 不停止可以无限次接收超时事件
        /// </summary>
        private bool isStopAtTimeOut = false;
        /// <summary>
        /// 计时器是否启动着
        /// </summary>
        private bool isRunning = false;

        public bool GetRunningStatus()
        {
            return this.isRunning;
        }

        public void Awake()
        {
            // 默认数据
            var outTime = 5f;
            this.SetData(outTime);
        }

        public void Update()
        {
            if (!this.isRunning)
            {
                return;
            }

            this.runningtime += Time.deltaTime;
            if (this.runningtime >= this.timeOutTime)
            {
                this.TimeOut();
                this.ResetTime();
                if (this.isStopAtTimeOut)
                {
                    this.StopTime();
                }
            }
        }

        /// <summary>
        /// 设置计时器 数据
        /// </summary>
        /// <param name="timeOutTime"> 超时时间 </param>
        /// <param name="isStopAtTimeOut">超时后是否停止 false不停止可以无限次接收超时事件</param>
        /// <returns></returns>
        public TimeOutComponent SetData(float timeOutTime, bool isStopAtTimeOut = false)
        {
            this.timeOutTime = timeOutTime;

            return this;
        }

        /// <summary>
        /// 开始计时 重复调用会重置计时时间
        /// </summary>
        public void Startime()
        {
            if (this.isRunning)
            {
                this.ResetTime();
                return;
            }

            this.ResetTime();
            this.isRunning = true;
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void StopTime()
        {
            this.isRunning = false;
            this.ResetTime();
        }

        /// <summary>
        /// 重置时间
        /// </summary>
        public void ResetTime()
        {
            this.runningtime = 0;
        }

        private void TimeOut()
        {
            this.OnTimeOutEvent?.Invoke();
        }
    }
}
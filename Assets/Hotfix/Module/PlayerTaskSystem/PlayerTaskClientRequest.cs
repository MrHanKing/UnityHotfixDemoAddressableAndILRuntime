using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 任务系统和其他系统耦合的调度器
    /// 所有任务检查点起调的入口
    /// </summary>
    public static class PlayerTaskClientRequest
    {
        /// <summary>
        /// 检查用户等级
        /// </summary>
        public static void CheckLevelUpTo()
        {
            // 获取当前任务数据传入检查器
            var taskResult = new PlayerTaskDoInfoLevelUpTo()
            {
                currentLevel = "1"
            };
            PlayerTaskSystem.CheckAllTask(taskResult);

        }


        /// <summary>
        /// 一次性任务
        /// /// </summary>
        public static void CheckOnceTask(PlayerTaskType taskType)
        {
            var taskResult = new PlayerTaskDoInfoNull()
            {
                taskType = taskType
            };
            PlayerTaskSystem.CheckAllTask(taskResult);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 任务检查基类
    /// </summary>
    public abstract class PlayerTaskCheckBase
    {
        /// <summary>
        /// 任务判断条件数据 表格配置数据
        /// </summary>
        public PlayerTaskConfigData conditionData;

        public int GetCurrentProgress()
        {
            if (this.conditionData.isFinished)
            {
                return this.conditionData.maxProgress;
            }
            return this.conditionData.currentProgress;
        }

        public int GetMaxProgress()
        {
            return this.conditionData.maxProgress;
        }

        public virtual bool NeedSaveProgress()
        {
            return false;
        }

        /// <summary>
        /// 检查任务是否符合条件
        /// </summary>
        /// <param name="taskDoInfo"></param>
        /// <returns></returns>
        public virtual bool CheckTask(PlayerTaskDoInfoBase taskDoInfo)
        {

            var result = false;
            var taskType = this.GetType();
            var attrs = taskType.GetCustomAttributes(typeof(TaskInfoAttribute), false);
            if (attrs.Length == 0)
            {
                Debug.LogError($"任务{taskType.Name}没有绑定输入数据类型");
                return result;
            }

            TaskInfoAttribute taskAttribute = attrs[0] as TaskInfoAttribute;

            var taskInputType = taskDoInfo.GetType().Name;
            if (taskInputType != taskAttribute.taskDoInfoName)
            {
                Debug.LogError($"任务{taskType.Name}输入数据有问题, 输入类型{taskInputType},目标类型{taskAttribute.taskDoInfoName}");
                return result;
            }

            result = CheckCondition(taskDoInfo);

            if (this.conditionData != null)
            {
                // 改变任务状态 
                // 已完成的不改变 防止被重置回去
                if (!this.conditionData.isFinished)
                {
                    this.conditionData.isFinished = result;
                }

                // 告诉服务器
                if (result || this.NeedSaveProgress())
                {
                    PlayerTaskSystem.PostOneTaskResult(this.conditionData);
                }
            }

            return this.conditionData.isFinished;
        }

        /// <summary>
        /// 各任务检查逻辑
        /// </summary>
        /// <param name="taskDoInfo"></param>
        /// <returns></returns>
        protected abstract bool CheckCondition(PlayerTaskDoInfoBase taskDoInfo);

        /// <summary>
        /// 跳转到对应的系统模块
        /// 跳转按钮的逻辑
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> GotoModuleUI();
    }
}
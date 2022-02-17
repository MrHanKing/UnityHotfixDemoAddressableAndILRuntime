using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 任务系统多重检查器 同一种输入
    /// </summary>
    public class PlayerTaskMultiChecker
    {
        /// <summary>
        /// 存储同一种输入的所有类型检查器材 一个list存储的是同一种类型的多阶段检查器
        /// </summary>
        private Dictionary<string, List<PlayerTaskCheckBase>> oneTypeTaskCheck = new Dictionary<string, List<PlayerTaskCheckBase>>();

        /// <summary>
        /// 增加需要检查的任务
        /// </summary>
        /// <param name="checker"></param>
        public void AddTaskChecker(PlayerTaskCheckBase checker)
        {
            var checkerTypeName = checker.GetType().Name;
            List<PlayerTaskCheckBase> targetValue;
            var isExist = this.oneTypeTaskCheck.TryGetValue(checkerTypeName, out targetValue);
            if (isExist)
            {
                targetValue.Add(checker);
            }
            else
            {
                targetValue = new List<PlayerTaskCheckBase>();
                targetValue.Add(checker);
                this.oneTypeTaskCheck[checkerTypeName] = targetValue;
            }
            // sort 排序
            targetValue.Sort((value1, value2) => value2.conditionData.id - value1.conditionData.id);
        }
        /// <summary>
        /// 根据输入检查所有类型的任务是否完成
        /// </summary>
        /// <param name="taskDoInfo">输入任务信息</param>
        /// <returns>返回所有被完成的任务列表</returns>
        public PlayerTaskCheckBase[] CheckAllTask(PlayerTaskDoInfoBase taskDoInfo)
        {
            var result = new List<PlayerTaskCheckBase>();
            var keys = this.oneTypeTaskCheck.Keys;
            List<PlayerTaskCheckBase> oneResult;
            // List<PlayerTaskCheckBase> unFinishedResult = new List<PlayerTaskCheckBase>();
            foreach (var key in keys)
            {
                var isExist = this.oneTypeTaskCheck.TryGetValue(key, out oneResult);
                if (isExist)
                {
                    var isFinish = true;
                    while (oneResult.Count > 0 && isFinish)
                    {
                        // 从后往前检查 最后个是最低级的
                        var target = oneResult.PopAt(oneResult.Count - 1);
                        isFinish = target.CheckTask(taskDoInfo);
                        if (isFinish)
                        {
                            result.Add(target);
                        }
                        else
                        {
                            // 不符合条件塞回去
                            oneResult.Add(target);
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}
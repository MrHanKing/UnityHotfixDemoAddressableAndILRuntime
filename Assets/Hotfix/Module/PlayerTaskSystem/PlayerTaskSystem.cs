using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 任务系统调度器
    /// </summary>
    public static class PlayerTaskSystem
    {
        /// <summary>
        /// 所有未完成的任务
        /// </summary>
        /// <typeparam name="string">输入属性类型</typeparam>
        /// <typeparam name="PlayerTaskMultiChecker"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, PlayerTaskMultiChecker> allPlayerTask = new Dictionary<string, PlayerTaskMultiChecker>();
        public static List<PlayerTaskCheckBase> allFinishedTask = new List<PlayerTaskCheckBase>();
        /// <summary>
        /// 所有的任务列表
        /// </summary>
        /// <typeparam name="PlayerTaskCheckBase"></typeparam>
        /// <returns></returns>
        public static List<PlayerTaskCheckBase> allTasks = new List<PlayerTaskCheckBase>();
        /// <summary>
        /// 任务跟类的映射关系
        /// </summary>
        /// <typeparam name="string">任务类型</typeparam>
        /// <typeparam name="Type">单任务实现类</typeparam>
        /// <returns></returns>
        private static Dictionary<PlayerTaskType, Type> taskCheckClass = new Dictionary<PlayerTaskType, Type>();
        private static bool isInit = false;
        /// <summary>
        /// 初始化系统
        /// </summary>
        public static async Task Init()
        {
            // 类型 -> class的映射关系
            if (!isInit)
            {
                var taskClass = CommonApiHelper.GetOneAttributeList<TaskInfoAttribute>();
                foreach (var oneClass in taskClass)
                {
                    var attr = GetCustomAttributes<TaskInfoAttribute>(oneClass);

                    if (attr != null)
                    {
                        taskCheckClass[attr.taskType] = oneClass;
                    }
                }
                isInit = true;
            }

            // todo release
            var configJson = await AddressableComponent.Instance.LoadSublevelAsset<TextAsset>("PlayerTaskSystem", "PlayerTaskConfig");
            var configs = JsonHelper.FromJson<PlayerTaskConfigData[]>(configJson.text);
            // 初始化待check任务
            // 初始化完成任务列表
            allPlayerTask.Clear();
            allTasks.Clear();
            foreach (var taskData in configs)
            {
                var resultData = PreDealWithData(taskData);
                InitOneTask(resultData);
            }

            // 同步服务器数据
            var serverData = await PlayerTaskSystemRequst.GetServerTaskList();
            // Debug.LogWarning("serverData:" + serverData.Length);
            foreach (var oneServer in serverData)
            {
                var target = allTasks.Find(value => value.conditionData.id == oneServer.id);
                if (target != null)
                {
                    target.conditionData.isFinished = oneServer.isFinished;
                    Log.Info($"finished one task {target.conditionData.id}");
                    // 进度数据
                    if (oneServer.currentProgress != 0)
                    {
                        target.conditionData.currentProgress = oneServer.currentProgress;
                    }
                    if (oneServer.maxProgress != 0)
                    {
                        target.conditionData.maxProgress = oneServer.maxProgress;
                    }
                }
            }
        }

        /// <summary>
        /// 预处理表格数据
        /// </summary>
        /// <returns></returns>
        private static PlayerTaskConfigData PreDealWithData(PlayerTaskConfigData data)
        {
            var result = data;
            // 进度默认处理
            if (result.maxProgress == 0)
            {
                result.maxProgress = 1;
            }

            return result;
        }

        private static T GetCustomAttributes<T>(Type classType) where T : class
        {
            var attrs = classType.GetCustomAttributes(typeof(T), false);
            if (attrs.Length > 0)
            {
                var attr = attrs[0] as T;
                return attr;
            }
            return null;
        }

        /// <summary>
        /// 初始化一个任务
        /// </summary>
        /// <param name="taskData"></param>
        private static void InitOneTask(PlayerTaskConfigData taskData)
        {
            Type targetClass;
            if (taskCheckClass.TryGetValue((PlayerTaskType)taskData.taskType, out targetClass))
            {
                var attr = GetCustomAttributes<TaskInfoAttribute>(targetClass);
                if (attr != null)
                {
                    // 创建
                    if (!allPlayerTask.ContainsKey(attr.taskDoInfoName))
                    {
                        allPlayerTask[attr.taskDoInfoName] = new PlayerTaskMultiChecker();
                    }

                    var target = allPlayerTask[attr.taskDoInfoName];
                    var instanceClass = Activator.CreateInstance(targetClass) as PlayerTaskCheckBase;
                    if (instanceClass != null)
                    {
                        instanceClass.conditionData = taskData;
                        target.AddTaskChecker(instanceClass);
                        // 只为了UI显示
                        allTasks.Add(instanceClass);
                    }
                    else
                    {
                        Debug.LogError($"有任务没有继承基类: {targetClass.Name}");
                    }
                }
            }
        }

        /// <summary>
        /// 检查所有任务是否完成
        /// </summary>
        /// <param name="taskDoInfo">相关的完成信息输入</param>
        public static void CheckAllTask(PlayerTaskDoInfoBase taskDoInfo)
        {
            var taskDoInfoTypeName = taskDoInfo.GetType().Name;
            Log.Info($"开始检查任务:{taskDoInfoTypeName}");
            PlayerTaskMultiChecker targetChecker;
            var isExist = allPlayerTask.TryGetValue(taskDoInfoTypeName, out targetChecker);
            if (isExist)
            {
                var finishedTask = targetChecker.CheckAllTask(taskDoInfo);
                if (finishedTask.Length > 0)
                {
                    allFinishedTask.AddRange(finishedTask);
                }
            }
            else
            {
                Log.Info("没有匹配的任务检查列表");
            }

            // 跟服务器同步数据
        }

        /// <summary>
        /// 提交一个任务结果
        /// </summary>
        /// <param name="data"></param>
        public static void PostOneTaskResult(PlayerTaskConfigData data)
        {
            var updateData = new PlayerTaskPostToServer();
            updateData.title = data.des;
            updateData.taskType = (PlayerTaskType)data.taskType;
            updateData.finish = data.isFinished;
            updateData.extra = data;
            updateData.taskId = data.id;
            PlayerTaskSystemRequst.PostOneTaskResult(updateData);
        }

        /// <summary>
        /// 是否需要任务引导手出现
        /// /// </summary>
        /// <returns></returns>
        public static bool IsNeedTaskFinger()
        {
            foreach (var oneTask in allTasks)
            {
                if (oneTask.conditionData.group == 1 && !oneTask.conditionData.isFinished)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 还没完成的已解锁的任务数量
        /// </summary>
        /// <returns></returns>
        public static int UnDoTasksNum()
        {
            return allTasks.FindAll(value => !value.conditionData.isLock && !value.conditionData.isFinished).Count;
        }
    }


    public static class PlayerTaskSystemRequst
    {
        public static async Task<PlayerTaskConfigData[]> GetServerTaskList()
        {
            var handler = await WebRequestSystem.AsyncGetWebRequest("/task");
            Log.Info(handler.text);
            var result = JsonHelper.FromJson<PlayerTaskPostToServer[]>(handler.text);
            List<PlayerTaskConfigData> targetResult = new List<PlayerTaskConfigData>();
            foreach (var item in result)
            {
                // targetResult.Add(JsonHelper.FromJson<PlayerTaskConfigData>(item.extra));
                targetResult.Add(item.extra);
            }
            return targetResult.ToArray();
        }

        public static async Task PostOneTaskResult(PlayerTaskPostToServer data)
        {
            var handler = await WebRequestSystem.AsyncPostWebRequest("/task/record", JsonHelper.ToJson(data));
            Log.Info(handler.text);
            // var result = JsonHelper.FromJson<ServerPostMimiGameScoreHandler>(handler.text);
            // return result;
        }

        public static async Task<ServerTimeData> GetServerTime()
        {
            var handler = await WebRequestSystem.AsyncGetWebRequest("/system/now");
            Log.Info(handler.text);
            var result = JsonHelper.FromJson<ServerTimeData>(handler.text);
            return result;
        }
    }
}

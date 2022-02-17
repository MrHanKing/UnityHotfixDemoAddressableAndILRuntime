using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TaskInfoAttribute : Attribute
    {
        public string taskDoInfoName { get; }
        public PlayerTaskType taskType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bindingDoInfoType">不能入参type ILRuntime无法解析 直接传类名</param>
        /// <param name="taskType"></param>
        public TaskInfoAttribute(string bindingDoInfoType, PlayerTaskType taskType)
        {
            this.taskDoInfoName = bindingDoInfoType;
            this.taskType = taskType;
        }
    }
}
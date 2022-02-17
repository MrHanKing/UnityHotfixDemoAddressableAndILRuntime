using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public class ServerTimeData
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <value></value>
        public long now { get; set; }
    }

    public enum PlayerTaskType
    {
        /// <summary>
        /// 升级到对应等级
        /// </summary>
        LevelUpTo = 5,
    }

    public class PlayerTaskPostToServer
    {
        public bool finish;
        // public int id;
        public PlayerTaskType taskType;
        /// <summary>
        /// PlayerTaskConfigData 的数据
        /// </summary>
        public PlayerTaskConfigData extra;
        public string title;
        public int taskId;
    }
    /// <summary>
    /// 目标显示分类
    /// </summary>
    public enum PlayerTaskSubUIType
    {
        // 课程目标
        CourseTag = 1,
        // 竞技场目标
        PvpTag = 2,
        // 训练场目标
        TrainTag = 3,
        // 其他目标
        OtherTag = 4
    }

    /// <summary>
    /// 配置表格输入数据
    /// </summary>
    public class PlayerTaskConfigData
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public int id;
        /// <summary>
        /// 任务组 显示分类 表示第几周 从1开始
        /// </summary>
        public int group { get; set; }
        /// <summary>
        /// 周内分类 课程目标 竞技场目标等 PlayerTaskSubUIType对应
        /// </summary>
        /// <value></value>
        public int subGroup { get; set; }
        /// <summary>
        /// 任务类型 等值PlayerTaskType
        /// 枚举在ILRuntime比较失败
        /// </summary>
        /// <value></value>
        public int taskType { get; set; }
        /// <summary>
        /// 关卡岛屿 ChapterType
        /// </summary>
        /// <value></value>
        public int chapter { get; set; }
        /// <summary>
        /// 岛屿上的第几个关卡
        /// </summary>
        /// <value></value>
        public int level { get; set; }
        /// <summary>
        /// 自主学习课 第几课 
        /// </summary>
        /// <value></value>
        public int courseSequence { get; set; }
        /// <summary>
        /// 是否完成
        /// </summary>
        /// <value></value>
        public bool isFinished { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        /// <value></value>
        public string des { get; set; }
        /// <summary>
        /// 段位等级达到
        /// </summary>
        /// <value></value>
        public string needDuan { get; set; }
        /// <summary>
        /// 当前进度
        /// </summary>
        /// <value></value>
        public int currentProgress { get; set; }
        /// <summary>
        /// 总共需要达成的进度
        /// </summary>
        /// <value></value>
        public int maxProgress { get; set; }
        /// <summary>
        /// 排序坐标
        /// </summary>
        /// <value></value>
        public int sortIndex { get; set; }
        /// <summary>
        /// 初始化后确认解锁没有的状态标记
        /// </summary>
        /// <value></value>
        public bool isLock { get; set; }
    }

    /// <summary>
    /// 任务系统客户端数据输入结构定义
    /// </summary>
    public class PlayerTaskDoInfoBase
    {
        /// <summary>
        /// 任务类型
        /// </summary>
        public PlayerTaskType taskType;
    }

    /// <summary>
    /// 不需要数据 触发就完成完成
    /// </summary>
    public class PlayerTaskDoInfoNull : PlayerTaskDoInfoBase
    {

    }

    /// <summary>
    /// 当前等级
    /// </summary>
    public class PlayerTaskDoInfoLevelUpTo : PlayerTaskDoInfoBase
    {
        /// <summary>
        /// 当前等级
        /// </summary>
        public string currentLevel;
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 等级达到
    /// </summary>
    // [TaskInfo(typeof(PlayerTaskDoInfoCourse), (int)PlayerTaskType.CourseFight)]
    [TaskInfo("PlayerTaskDoInfoLevelUpTo", PlayerTaskType.LevelUpTo)]
    public class PlayerTaskCheckLevelUpTo : PlayerTaskCheckBase
    {
        public override async Task<bool> GotoModuleUI()
        {
            // 前往场景 跳转
            // await UIRouterHelper.LoadSceneRoutine(UIType.UIArena, TransType.UITransitions);
            return true;
        }

        protected override bool CheckCondition(PlayerTaskDoInfoBase taskDoInfo)
        {
            var result = false;
            // 外面检查了 安心as
            var data = taskDoInfo as PlayerTaskDoInfoLevelUpTo;
            if (!string.IsNullOrWhiteSpace(data.currentLevel))
            {
                // 任务完成判定条件
                return false;
            }

            return result;
        }
    }
}
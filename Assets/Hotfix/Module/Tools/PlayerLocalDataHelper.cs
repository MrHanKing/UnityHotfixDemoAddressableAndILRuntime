using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 本地数据存储中心
    /// </summary>
    public static class PlayerLocalDataHelper
    {
        /// <summary>
        /// 引导手相关存储key前缀
        /// </summary>
        private static string fingerPreKey = "Finger_";
        /// <summary>
        /// 存储关键key已经触发了
        /// </summary>
        /// <param name="savakey"></param>
        public static void SetFingerBoolData(string savakey)
        {
            var targetKey = fingerPreKey + savakey;
            PlayerPrefs.SetInt(targetKey, 1);
        }
        /// <summary>
        /// 获得关键key是否已经使用过了
        /// </summary>
        /// <param name="savakey"></param>
        /// <returns></returns>
        public static bool GetFingerBoolData(string savakey)
        {
            var targetKey = fingerPreKey + savakey;
            var result = PlayerPrefs.GetInt(targetKey, 0);
            return result > 0;
        }
    }
}



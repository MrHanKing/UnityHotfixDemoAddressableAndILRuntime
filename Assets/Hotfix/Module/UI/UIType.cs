using System;
using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 一级界面类型
    /// </summary>
    public static class UIType
    {
        public const string UILogin = "UILogin";
    }

    /// <summary>
    /// 主UI内部的子UI类型 方便其他系统异步调用 出现界面
    /// </summary>
    public static class SubUIType
    {
        /// <summary>
        /// 保持key的名字和string的名字以及预制体的名字相同
        /// </summary>
        public const string UIPropKuang = "UIPropKuang";

    }

    /// <summary>
    /// 效果类
    /// </summary>
    public static class EffectUIType
    {
        /// <summary>
        /// 获得金币
        /// </summary>
        public const string EffectGetCoins = "EffectGetCoins";
    }
}
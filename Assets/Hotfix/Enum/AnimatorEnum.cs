using UnityEngine;
namespace ETHotfix
{
    public enum AnimatorType
    {
        /// <summary>
        /// 放大
        /// </summary>
        SmallToBig,
        /// <summary>
        /// 从左到右
        /// </summary>
        LeftToRight,
        /// <summary>
        /// 半透明
        /// </summary>
        Translucent,
        /// <summary>
        /// 从上到下弹弹弹  
        /// </summary>
        PingPang,
        /// <summary>
        /// 转场动画 云
        /// </summary>
        Transtion,
        /// <summary>
        /// 不使用动画
        /// </summary>
        DonotUseAni,
        /// <summary>
        /// 没有遮罩
        /// </summary>
        NoMaskAni,
    }

    public enum AnimatorComponentType
    {
        /// <summary>
        /// 从小到大弹几下
        /// </summary>
        SmallToBig,
        WinnerBtn,
        ScaleZero,
        /// <summary>
        /// 从小到大
        /// </summary>
        SmallToBigOne,

    }

    public enum TextAnimatorType
    {
        /// <summary>
        /// 小到大透明
        /// </summary>
        SmallToBig,
        /// <summary>
        /// 呼吸
        /// </summary>
        Yoyo,
        /// <summary>
        /// 透明
        /// </summary>
        Translucent,
    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
namespace ETModel
{
    public enum CanvasScaleMode
    {
        None,
        PreferOuter,
        PreferCenter,
        ForceWidth,
        ForceHeight
    }
    /// <summary>
    /// Canvas Scaler 运行时修改器
    /// 当屏幕为宽屏时 -> Canvas高度不变，横向扩展
    /// 当屏幕为方屏时 -> Canvas宽度不变，纵向扩展
    /// </summary>
    [ExecuteInEditMode]
    public class CanvasFitter : MonoBehaviour
    {
        public bool autoReScale = true;
        public CanvasScaleMode scaleMode;
        // Start is called before the first frame update
        void Start()
        {
            if (autoReScale)
            {
                ReScale();
            }
        }

        public void ReScale(CanvasScaleMode _mode = CanvasScaleMode.None)
        {
            //if (_mode != CanvasScaleMode.None) { scaleMode = _mode; }
            if (_mode == CanvasScaleMode.None) _mode = scaleMode;

            float defaultRatio = 1920f / 1080f;
            float currRatio = (float)Screen.width / (float)Screen.height;
            float offset = currRatio - defaultRatio;
            if (offset == 0f) return;

            CanvasScaler scaler = GetComponent<CanvasScaler>();
            if (scaler)
            {
                switch (_mode)
                {
                    case CanvasScaleMode.PreferOuter:
                        scaler.matchWidthOrHeight = offset > 0 ? 1 : 0;
                        break;
                    case CanvasScaleMode.PreferCenter:
                        scaler.matchWidthOrHeight = offset > 0 ? 0 : 1;
                        break;
                    case CanvasScaleMode.ForceWidth:
                        scaler.matchWidthOrHeight = 0;
                        break;
                    case CanvasScaleMode.ForceHeight:
                        scaler.matchWidthOrHeight = 1;
                        break;
                }
            }
        }

        ///// <summary>
        ///// 强制横向拉伸
        ///// </summary>
        //public void ForceWidth()
        //{
        //    CanvasScaler scaler = GetComponent<CanvasScaler>();
        //    if (scaler)
        //    {
        //        scaler.matchWidthOrHeight = 0;
        //    }
        //}

        ///// <summary>
        ///// 强制纵向拉伸
        ///// </summary>
        //public void ForceHeight()
        //{
        //    CanvasScaler scaler = GetComponent<CanvasScaler>();
        //    if (scaler)
        //    {
        //        scaler.matchWidthOrHeight = 1;
        //    }
        //}
    }
}



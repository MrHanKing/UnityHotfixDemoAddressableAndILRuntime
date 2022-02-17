
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ETModel
{
    /// <summary>
    /// 配合button改变其他节点颜色
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(UIButtonMine))]
    public class UiButtonChangeColor : MonoBehaviour
    {
        public Graphic render;
        [SerializeField]
        private ColorBlock m_Colors = ColorBlock.defaultColorBlock;

        private void OnEnable()
        {
            var target = this.transform.GetComponent<UIButtonMine>();
            if (target != null)
            {
                target.OnStatusChange.AddListener(this.OnButtonStatusChanged);
            }
        }

        private void OnDisable()
        {
            var target = this.transform.GetComponent<UIButtonMine>();
            if (target != null)
            {
                target.OnStatusChange.RemoveListener(this.OnButtonStatusChanged);
            }
        }

        public void OnButtonStatusChanged(UIButtonMine.UIButtonSelectionState status)
        {
            if (this.render == null)
            {
                return;
            }
            Color resultColor = Color.white;
            switch (status)
            {
                case UIButtonMine.UIButtonSelectionState.Highlighted:
                    resultColor = this.m_Colors.highlightedColor;
                    break;
                case UIButtonMine.UIButtonSelectionState.Pressed:
                    resultColor = this.m_Colors.pressedColor;
                    break;
                case UIButtonMine.UIButtonSelectionState.Selected:
                    resultColor = this.m_Colors.selectedColor;
                    break;
                case UIButtonMine.UIButtonSelectionState.Disabled:
                    resultColor = this.m_Colors.disabledColor;
                    break;
                default:
                    resultColor = this.m_Colors.normalColor;
                    break;

            }

            this.render.CrossFadeColor(resultColor, 0f, true, true);
        }
    }
}
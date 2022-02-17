
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ETModel
{
    /// <summary>
    /// 可以监听修改的button
    /// </summary>
    public class UIButtonMine : Button
    {
        public enum UIButtonSelectionState
        {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Selected = 3,
            Disabled = 4
        }
        [Serializable]
        public class ButtonStatusChangedEvent : UnityEvent<UIButtonSelectionState> { }
        [SerializeField]
        public ButtonStatusChangedEvent OnStatusChange = new ButtonStatusChangedEvent();

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            this.OnStatusChange?.Invoke((UIButtonSelectionState)state);
        }
    }
}
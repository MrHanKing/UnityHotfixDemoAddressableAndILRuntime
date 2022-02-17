using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace ETModel
{
    /// <summary>
    /// UGUI得触发事件脚本
    /// </summary>
    public class UIBehaviours : MonoBehaviour,
                                IPointerDownHandler,
                                IPointerEnterHandler,
                                IPointerExitHandler,
                                IPointerUpHandler,
                                ISelectHandler,
                                IUpdateSelectedHandler,
                                IDeselectHandler,
                                IDragHandler,
                                IEndDragHandler,
                                IDropHandler,
                                IScrollHandler,
                                IMoveHandler,
                                IPointerClickHandler,
                                IBeginDragHandler
    {

        public delegate void VoidDelegate(GameObject go);
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onDeSelect;
        public VoidDelegate onDrag;
        public VoidDelegate onDragEnd;
        public VoidDelegate onDrop;
        public VoidDelegate onScroll;
        public VoidDelegate onMove;
        public VoidDelegate onBecameVisible;
        public VoidDelegate onBecameInvisible;
        public VoidDelegate onBeginDrag;
        public Action OneClick;
        public Action DoubleClick;
        public static UIBehaviours Get(GameObject go)
        {

            UIBehaviours listener = go.GetComponent<UIBehaviours>();
            if (listener == null) listener = go.AddComponent<UIBehaviours>();
            return listener;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null)
                onClick(gameObject);

            if (eventData.clickCount == 2 && DoubleClick != null)
            { DoubleClick();Debug.Log("双击"); }
            else if (eventData.clickCount == 1 && OneClick != null)
            { OneClick();Debug.Log("单击"); }

        }

        public void OnPointerDown(PointerEventData eventData) { if (onDown != null) onDown(gameObject); }
        public void OnPointerEnter(PointerEventData eventData) { if (onEnter != null) onEnter(gameObject); }
        public void OnPointerExit(PointerEventData eventData) { if (onExit != null) onExit(gameObject); }
        public void OnPointerUp(PointerEventData eventData) { if (onUp != null) onUp(gameObject); }
        public void OnSelect(BaseEventData eventData) { if (onSelect != null) onSelect(gameObject); }
        public void OnUpdateSelected(BaseEventData eventData) { if (onUpdateSelect != null) onUpdateSelect(gameObject); }
        public void OnDeselect(BaseEventData eventData) { if (onDeSelect != null) onDeSelect(gameObject); }
        public void OnDrag(PointerEventData eventData) { if (onDrag != null) onDrag(gameObject); }
        public void OnEndDrag(PointerEventData eventData) { if (onDragEnd != null) onDragEnd(gameObject); }
        public void OnDrop(PointerEventData eventData) { if (onDrop != null) onDrop(gameObject); }
        public void OnScroll(PointerEventData eventData) { if (onScroll != null) onScroll(gameObject); }
        public void OnMove(AxisEventData eventData) { if (onMove != null) onMove(gameObject); }
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onBeginDrag != null) onBeginDrag(gameObject);
        }
        void OnBecameVisible()
        {
            if (onBecameVisible != null)
                onBecameVisible(gameObject);
        }

        void OnBecameInvisible()
        {
            if (onBecameInvisible != null)
                onBecameInvisible(gameObject);
        }

        public void AddButtonListener(UnityAction action)
        {
            Button btn = transform.GetComponent<Button>();
            btn.onClick.AddListener(action);

        }

        public void AddSliderListener(UnityAction<float> action)
        {
            Slider slider = transform.GetComponent<Slider>();
            slider.onValueChanged.AddListener(action);

        }


        public void AddToggleListener(UnityAction<bool> action)
        {
            Toggle toggle = transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(action);


        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.LogError("进入");
        }
        private void OnTriggerExit(Collider other)
        {
            Debug.LogError("出去");
        }



        void OnDestroy()
        {
            onClick = null;
            onDown = null;
            onEnter = null;
            onExit = null;
            onUp = null;
            onSelect = null;
            onUpdateSelect = null;
            onDeSelect = null;
            onDrag = null;
            onDragEnd = null;
            onDrop = null;
            onScroll = null;
            onMove = null;

        }

       
    }

}


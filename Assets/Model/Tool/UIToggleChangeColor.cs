
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
    [RequireComponent(typeof(Toggle))]
    public class UIToggleChangeColor : MonoBehaviour
    {
        public Transform selectedShow;
        public Transform unSelectedShow;

        private void OnEnable()
        {
            var target = this.transform.GetComponent<Toggle>();
            if (target != null)
            {
                target.onValueChanged.AddListener(this.OnButtonStatusChanged);
            }
        }

        private void OnDisable()
        {
            var target = this.transform.GetComponent<Toggle>();
            if (target != null)
            {
                target.onValueChanged.RemoveListener(this.OnButtonStatusChanged);
            }
        }

        public void OnButtonStatusChanged(bool status)
        {
            this.selectedShow?.gameObject.SetActive(status);
            this.unSelectedShow?.gameObject.SetActive(!status);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public enum PoolType
    {
        Loop,    //循环
        General, //普通模式
    }
    public class LoadPoolObjectComponent : MonoBehaviour
    {
        public PoolType LoadType;
        public int LoadNum;
        Transform[] list;
        // Use this for initialization
        void Awake()
        {
            list = gameObject.GetComponentsInParent<Transform>();
            Load();


        }

        private void Load()
        {
            switch (LoadType)
            {
                case PoolType.General:
                    LoadListGameObjectGeneral();
                    break;
                case PoolType.Loop:
                    LoadListGameObjectLoop();
                    break;
            }
        }

        private void LoadListGameObjectGeneral()
        {
            for (int i = 0; i < list.Length; i++)
            {
                PoolObjectManger.Instance.CreatPool(list[i].name, LoadNum, list[i].gameObject, 0);
            }
        }

        private void LoadListGameObjectLoop()
        {
            for (int i = 0; i < list.Length; i++)
            {
                PoolObjectManger.Instance.CreatPool(list[i].name, LoadNum, list[i].gameObject, true);
            }
        }

        private void RemoveListGameObject()
        {
            for (int i = 0; i < list.Length; i++)
            {
                PoolObjectManger.Instance.RemovePoolChild(list[i].name);
            }
        }

        void OnDestroy()
        {
            RemoveListGameObject();
        }
    }
}


using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class PoolUIRootManager
    {
        Dictionary<string, Transform> uiRootPool;               //每个池的父节点
        Dictionary<string, List<GameObject>> listGoPool;        //池下的所有对象
        private Transform poolRoot;
        public Transform PoolRoot
        {
            get { return poolRoot; }
            set
            {
                if (poolRoot == null)
                {
                    poolRoot = new GameObject().transform;
                    poolRoot.name = "PoolRoot";
                    poolRoot.SetParent(value);
                }
            }
        }

        public PoolUIRootManager(Transform tran)
        {
            uiRootPool = new Dictionary<string, Transform>();
            listGoPool = new Dictionary<string, List<GameObject>>();
            PoolRoot = tran;
        }

        public void AddPoolParent(string iNames, GameObject go)
        {
            if (!uiRootPool.ContainsKey(iNames))
            {
                Transform tran = new GameObject().transform;
                tran.gameObject.name = iNames;
                tran.SetParent(poolRoot);
                tran.localScale = Vector3.one;
                tran.localPosition = Vector3.zero;
                uiRootPool.Add(iNames, tran);
                go.transform.SetParent(tran);
                AddListGoPool(iNames, go);
            }
            else
            {
                go.transform.SetParent(uiRootPool[iNames]);
            }
        }

        private void AddListGoPool(string iName,GameObject go)
        {
            if (!listGoPool.ContainsKey(iName))
            {
                listGoPool.Add(iName, new List<GameObject>() { go });
            }
            else
            {
                listGoPool[iName].Add(go);
            }
        }


        public void RemovePoolChild(string childName)
        {
            if (uiRootPool.ContainsKey(childName))
            {
                RemoveListGoPool(childName);
                GameObject.Destroy(uiRootPool[childName].gameObject);
                uiRootPool.Remove(childName);
            }
        }

        private void RemoveListGoPool(string childName)
        {
            if (listGoPool.ContainsKey(childName))
            {
                foreach(GameObject go in listGoPool[childName])
                {
                    GameObject.Destroy(go);
                }
                listGoPool.Remove(childName);
            }
        }


        public void Clear()
        {
            uiRootPool.Clear();
            listGoPool.Clear();
            uiRootPool = null;
            listGoPool = null;
            UnityEngine.Object.Destroy(poolRoot.gameObject);
            poolRoot = null;
        }

    }
}


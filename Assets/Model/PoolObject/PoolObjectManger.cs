using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    public class PoolObjectManger : MonoBehaviour
    {
        public static PoolObjectManger Instance = null;
        private Dictionary<string, PoolObject> pools = new Dictionary<string, PoolObject>();
        private PoolUIRootManager poolUIRoot;
        public Transform GetPoolUIRoot
        {
            get
            {
                return poolUIRoot.PoolRoot;
            }
        }

        public void Clear()
        {
            foreach (PoolObject po in pools.Values)
            {
                po.Clear();
            }
            pools.Clear();
            poolUIRoot.Clear();
        }

        public void CreatPool(string iNames, int num, GameObject go, float iTimes)
        {
            //把原来的设置false
            go.SetActive(false);
            if (!pools.ContainsKey(iNames))
            {
                PoolObject po = new PoolObject();
                for (int i = 0; i < num; i++)
                {
                    GameObject obj = GameObject.Instantiate(go);
                    po.AddObject(iNames, obj, iTimes);
                    poolUIRoot.AddPoolParent(iNames, obj);
                }
                pools.Add(iNames, po);
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    GameObject obj = GameObject.Instantiate(go);
                    pools[iNames].AddObject(iNames, obj, iTimes);
                    poolUIRoot.AddPoolParent(iNames, obj);
                }
            }
        }
        public void CreatPool(string iNames, int num, GameObject go, bool isLoop)
        {
            //把原来的设置false
            go.SetActive(false);
            if (!pools.ContainsKey(iNames))
            {
                PoolObject po = new PoolObject();
                for (int i = 0; i < num; i++)
                {
                    GameObject obj = Instantiate(go);
                    po.AddObject(iNames, obj, isLoop);
                    poolUIRoot.AddPoolParent(iNames, obj);
                }
                pools.Add(iNames, po);
            }
            else
            {
                for (int i = 0; i < num; i++)
                {
                    GameObject obj = Instantiate(go);
                    pools[iNames].AddObject(iNames, obj, isLoop);
                    poolUIRoot.AddPoolParent(iNames, obj);
                }
            }
        }
        public GameObject PlayPoolObject(string iNames, float iTimes, GameObject temp = null, string target = "")
        {
            GameObject obj = null;
            if (temp != null)
            {
                CreatPool(iNames, 1, temp, false);
            }
            if (pools.ContainsKey(iNames))
            {
                obj = pools[iNames].GetGameObject(iTimes, target);
                if (obj == null)
                {
                    GameObject go = Instantiate(pools[iNames].PV.Go);
                    pools[iNames].AddObject(iNames, go, iTimes, target);
                    poolUIRoot.AddPoolParent(iNames, go);
                    return go;
                }
                return obj;
            }
            //Debug.Log("没有此对象池 = " + iNames);
            return null;
        }

        /// <summary>
        /// 从池里取出播放
        /// </summary>
        /// <param name="iNames">池名字</param>
        /// <param name="isLoop">循环播放</param>
        /// <param name="target">标记</param>
        /// <returns></returns>
        public GameObject PlayPoolObject(string iNames, bool isLoop, string target = "")
        {
            GameObject obj = null;
            if (pools.ContainsKey(iNames))
            {
                obj = pools[iNames].GetGameObject(isLoop, target);
                if (obj == null)
                {
                    GameObject go = Instantiate(pools[iNames].PV.Go);
                    pools[iNames].AddObject(iNames, go, isLoop, target);
                    poolUIRoot.AddPoolParent(iNames, go);
                    return go;
                }
                return obj;
            }

            Debug.Log("没有此对象池 = " + iNames);
            return null;
        }


        public void HidePoolObject(string iNames, GameObject go)
        {
            if (pools.ContainsKey(iNames))
            {
                pools[iNames].HidePool(go, iNames);
            }
        }

        public void HidePoolObject(string iNames, string target)
        {
            if (pools.ContainsKey(iNames))
            {
                pools[iNames].HidePool(target);
            }
        }

        public void HidePoolAllObject(string iNames)
        {
            if (pools.ContainsKey(iNames))
            {
                pools[iNames].HidePoolAllObject();
            }
        }

        public GameObject FindPoolObject(string iNames, string target)
        {
            if (pools.ContainsKey(iNames))
            {
                return pools[iNames].FindPoolGameObject(target);
            }

            return null;
        }

        /// <summary>
        /// 删除iName 池
        /// </summary>
        /// <param name="iNames"></param>
        public void RemovePoolChild(string iNames)
        {
            if (pools.ContainsKey(iNames))
            {
                pools[iNames].Clear();
                pools.Remove(iNames);
                poolUIRoot.RemovePoolChild(iNames);
            }

        }

        void Awake()
        {
            Instance = this;
            poolUIRoot = new PoolUIRootManager(transform);
        }

        void Update()
        {
            if (pools.Count > 0)
            {
                foreach (PoolObject obj in pools.Values)
                {
                    obj.UpdatePool();
                }
            }

        }

        //void OnDestroy()
        //{
        //    poolUIRoot.Clear();
        //    foreach (PoolObject po in pools.Values)
        //    {
        //        po.Clear();
        //    }
        //    pools.Clear();
        //}

    }

}

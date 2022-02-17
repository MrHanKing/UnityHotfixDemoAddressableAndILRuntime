using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class IdlePoolList : IPoolListInterFace
    {
        private List<PoolValue> idleList;

        public IdlePoolList()
        {
            idleList = new List<PoolValue>();
        }

        public override void AddList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            pv.Go.SetActive(false);
            idleList.Add(pv);
        }

        public override void RemoveList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            idleList.Remove(pv);
        }

        public override void UpdateList(PoolObject obj)
        {


        }

        public GameObject GetGameObject(float iTimes, PoolObject obj = null, string target = "")
        {
            if (idleList.Count > 0 && obj != null)
            {
                GameObject go = idleList[0].Go;
                if (target != "")
                    idleList[0].target = target;
                idleList[0].Times = iTimes;
                obj.playPoolList.AddList(idleList[0]);
                RemoveList(idleList[0]);
                return go;
            }
            return null;
        }
        public GameObject GetGameObject(bool isLoop, PoolObject obj = null, string target = "")
        {
            if (idleList.Count > 0 && obj != null)
            {
                GameObject go = idleList[0].Go;
                if (target != "")
                    idleList[0].target = target;
                idleList[0].IsLoop = isLoop;
                obj.playPoolList.AddList(idleList[0]);
                RemoveList(idleList[0]);
                return go;
            }
            return null;
        }
        public override void Clear()
        {
            foreach (PoolValue poolValue in idleList)
            {
                poolValue.Clear();
            }
            idleList.Clear();
        }
    }
}


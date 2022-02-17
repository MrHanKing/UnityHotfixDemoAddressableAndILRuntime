using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    public class WaitPoolList : IPoolListInterFace
    {

        private List<PoolValue> waitList;

        public WaitPoolList()
        {
            waitList = new List<PoolValue>();
        }

        public override void AddList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            waitList.Add(pv);
        }

        public override void RemoveList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            waitList.Remove(pv);
        }

        public override void UpdateList(PoolObject obj)
        {
            for (int i = 0; i < waitList.Count; i++)
            {
                if (obj.playPoolList.Contains(waitList[i]))
                {
                    obj.playPoolList.RemoveList(waitList[i]);
                }
            }
            waitList.Clear();
        }
        public override void Clear()
        {
            foreach (PoolValue poolValue in waitList)
            {
                poolValue.Clear();
            }
            waitList.Clear();
        }
    }
}


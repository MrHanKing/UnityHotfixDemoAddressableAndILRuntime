using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class PoolValue
    {
        public PoolValue()
        {
            Times = 0;
            Go = null;
        }
        
        public GameObject Go { get; set; }
        public float Times { get; set; }
        public bool IsLoop { get; set; }
        public string PName { get; set; }
        public string target { get; set; }

        public void AddObj(GameObject obj, float iTimes, string names, bool isloop = false,string target = "")
        {
            Go = obj;
            Times = iTimes;
            IsLoop = isloop;
            PName = names;
            obj.name = names;
            this.target = target;
        }
        public void Clear()
        {
            Go = null;
        }
    }


    public class PoolObject : IPoolInterFace
    {
       
        public PoolValue PV { get; set; }
        public PlayPoolList playPoolList;
        public IdlePoolList idlePoolList;
        public WaitPoolList waitPoolList;

        public PoolObject()
        {
            playPoolList = new PlayPoolList();
            idlePoolList = new IdlePoolList();
            waitPoolList = new WaitPoolList();
        }

        public void AddObject(string iNames, GameObject go, float times = 0, string target = "")
        {
            PoolValue pv = new PoolValue();
            pv.AddObj(go, times, iNames,false, target);
            playPoolList.AddList(pv);
            if(PV == null)
            PV = pv;
        }

        public void AddObject(string iNames, GameObject go, bool isLoop = true,string target = "")
        {
            PoolValue pv = new PoolValue();
            pv.AddObj(go, 0, iNames, isLoop,target);
            playPoolList.AddList(pv);
            if (PV == null)
                PV = pv;

        }

        public void HidePool(GameObject go,string PName)
        {
            playPoolList.HideGameObject(go);
            PoolValue pv = new PoolValue();
            pv.AddObj(go, 0, PName);
            idlePoolList.AddList(pv);
        }

        public void HidePool(string target)
        {
            GameObject go = playPoolList.GetGameObject(target);
            if (go == null)
                Log.Info("找不到target:" + target);
            playPoolList.HideGameObject(target);
            PoolValue pv = new PoolValue();
            pv.AddObj(go, 0, target);
            idlePoolList.AddList(pv);
        }

        public GameObject FindPoolGameObject(string target)
        {
            return playPoolList.GetGameObject(target);
        }

        public void HidePoolAllObject()
        {
            List<GameObject> list = new List<GameObject>();
            foreach(PoolValue pv in playPoolList.PlayList)
            {
                list.Add(pv.Go);
            }
            for(int i = 0;i < list.Count; i++)
            {
                HidePool(list[i], PV.PName);
            }
        }

        public void UpdatePool()
        {
            playPoolList.UpdateList(this);
            waitPoolList.UpdateList(this);

        }

        public GameObject GetGameObject(float iTimes,string target = "")
        {
            return idlePoolList.GetGameObject(iTimes, this,target);
        }
        public GameObject GetGameObject(bool isLoop, string target = "")
        {
            return idlePoolList.GetGameObject(isLoop, this,target);
        }

        public void Clear()
        {
            PV?.Clear();
            PV = null;
            playPoolList?.Clear();
            idlePoolList?.Clear();
            waitPoolList?.Clear();
            playPoolList = null;
            idlePoolList = null;
            waitPoolList = null;
        }
    }

}

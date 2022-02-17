using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public class PlayPoolList : IPoolListInterFace
    {
        private List<PoolValue> playList;

        public List<PoolValue> PlayList
        {
            get
            {
                return playList;
            }

            set
            {
                playList = value;
            }
        }

        public PlayPoolList()
        {
            PlayList = new List<PoolValue>();
        }

        public override void AddList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            pv.Go.SetActive(true);
            PlayList.Add(pv);
        }

        public override void RemoveList(object obj)
        {
            PoolValue pv = (PoolValue)obj;
            PlayList.Remove(pv);
        }

        public void HideGameObject(GameObject obj)
        {
            int index = -1;
            for (int i = 0; i < PlayList.Count; i++)
            {
                if (PlayList[i].Go == obj)
                {
                    obj.SetActive(false);
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                PlayList.RemoveAt(index);
            }

        }

        public void HideGameObject(string target)
        {
            int index = -1;
            for (int i = 0; i < PlayList.Count; i++)
            {
                if (PlayList[i].target == target)
                {
                    index = i;
                    break;
                }
            }
            if (index != -1)
            {
                PlayList.RemoveAt(index);
            }

        }

        public GameObject GetGameObject(string target)
        {
            for (int i = 0; i < PlayList.Count; i++)
            {
                if (PlayList[i].target == target)
                {
                    return PlayList[i].Go;
                }
            }
            
            return null;
        }


        public override void UpdateList(PoolObject obj)
        {

            for (int i = 0; i < PlayList.Count; i++)
            {
                PlayList[i].Times -= Time.deltaTime;
                if (PlayList[i].Times <= 0 && !PlayList[i].IsLoop)
                {
                    obj.idlePoolList.AddList(PlayList[i]);
                    obj.waitPoolList.AddList(PlayList[i]);
                }
            }
        }

        public bool Contains(PoolValue pv)
        {
            return PlayList.Contains(pv);
        }

        public GameObject GetGameObject()
        {
            if (PlayList.Count > 0)
            {
                return PlayList[0].Go;
            }

            return null;
        }

        public override void Clear()
        {
            foreach(PoolValue poolValue  in playList)
            {
                poolValue.Clear();
            }
            PlayList.Clear();
        }
    }
}


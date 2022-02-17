using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{

    public class DurationHelp
    {
        private static DurationHelp instance;
        public static DurationHelp I
        {
            get
            {
                if (instance == null)
                {
                    instance = new DurationHelp();
                    Init();
                }
                return instance;
            }
        }

        private static Dictionary<string, float> evetDuration_dic;
        private static void Init()
        {
            evetDuration_dic = new Dictionary<string, float>();
        }

        //时间开始调用即可
        public int GetEventDuration(string _eventName)
        {
            float duration = 0;
            if (evetDuration_dic.ContainsKey(_eventName))
            {
                float startTime = evetDuration_dic[_eventName];
                duration = Time.time - startTime;
                evetDuration_dic.Remove(_eventName);
            }
            return  (int)duration;
        }

        public void AddEventDuration(string _eventName)
        {
            if (evetDuration_dic.ContainsKey(_eventName))
                evetDuration_dic.Remove(_eventName);
            evetDuration_dic.Add(_eventName, Time.time);
        }


        //使用Time.time 计时 游戏进入后台暂停 Time.time 也不会增加
        //private void OnApplicationPause(bool pause)
        //{

        //}
        //private void OnApplicationFocus(bool focus)
        //{

        //}

    }

}



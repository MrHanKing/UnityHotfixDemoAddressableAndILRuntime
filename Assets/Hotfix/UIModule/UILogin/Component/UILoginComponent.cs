
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using ETModel;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Playables;
using System.Globalization;
using UnityEngine.Video;

namespace ETHotfix
{
    [ObjectSystem]
    public class UILoginComponentSystem : AwakeSystem<UILoginComponent>
    {
        public override void Awake(UILoginComponent self)
        {
            self.Awake();
        }
    }

    public class UILoginComponent : Component
    {
        private UI rootEntity;
        public void Awake()
        {
            Debug.Log("热更层UILogin界面开始运行");
            rootEntity = this.GetParent<UI>();
            // Debug.Log("热更测试1");
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();
        }
    }
}
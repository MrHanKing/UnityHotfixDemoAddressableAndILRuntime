
using ETModel;
using Lean.Touch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class GlobalGameObjectComponentSystem : AwakeSystem<GlobalGameObjectComponent>
    {
        public override void Awake(GlobalGameObjectComponent self)
        {
            self.Awake();
        }
    }

    /// <summary>
    /// 全局【节点相关】资源的引用组件 方便管理和引用
    /// 注意：GameObject有个扩展方法Get可以直接从资源收集器中拿资源
    /// </summary>
    public class GlobalGameObjectComponent : Component
    {
        /// <summary>
        /// UI root
        /// </summary>
        /// <value></value>
        public GameObject uiRoot { get; private set; }
        public GameObject world { get; private set; }
        public GameObject camera { get; private set; }
        public LeanTouch leanTouch { get; private set; }
        /// <summary>
        /// 一级界面的
        /// </summary>
        /// <value></value>
        public GameObject oneCanvas { get; private set; }
        /// <summary>
        /// 二级界面的
        /// </summary>
        /// <value></value>
        public GameObject twoCanvas { get; private set; }
        /// <summary>
        /// 更新界面专用
        /// </summary>
        /// <value></value>
        public GameObject upCanvas { get; private set; }
        /// <summary>
        /// 用户的库存系统
        /// </summary>
        // public InventoryUser inventoryUser { get; private set; }
        /// <summary>
        /// 地图摄像机
        /// </summary>
        /// <value></value>
        public GameObject levelMapCamera
        {
            get
            {
                return this.camera.Get<GameObject>("LevelMapCamera");
            }
        }
        public static GlobalGameObjectComponent Instance { get { return Game.Scene.GetComponent<GlobalGameObjectComponent>(); } }
        public void Awake()
        {
            this.uiRoot = GameObject.Find("Global/UI/");
            this.world = GameObject.Find("Global/World/");
            this.camera = GameObject.Find("Global/Camera/");
            this.leanTouch = GameObject.Find("LeanTouch").GetComponent<LeanTouch>();
            this.oneCanvas = GameObject.Find("OneCanvas");
            this.twoCanvas = GameObject.Find("TwoCanvas");
            this.upCanvas = GameObject.Find("UpCanvas");
            // this.inventoryUser = GameObject.Find("Global/InventoryUser").GetComponent<InventoryUser>();
        }

        public Canvas GetCanvasByName(string cavasName)
        {
            return this.uiRoot.Get<GameObject>(cavasName)?.transform.GetComponent<Canvas>();
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


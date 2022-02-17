using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class UiComponentAwakeSystem : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UiComponentLoadSystem : LoadSystem<UIComponent>
    {
        public override void Load(UIComponent self)
        {
            self.Load();
        }
    }

    /// <summary>
    /// 管理所有UI
    /// </summary>
    public class UIComponent : Component
    {
        private GameObject Root;
        private readonly Dictionary<string, IUIFactory> UiTypes = new Dictionary<string, IUIFactory>();
        private readonly Dictionary<string, UI> uis = new Dictionary<string, UI>();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            foreach (string type in uis.Keys.ToArray())
            {
                UI ui;
                if (!uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                uis.Remove(type);
                ui.Dispose();
            }

            this.UiTypes.Clear();
            this.uis.Clear();
        }

        public void Awake()
        {
            this.Root = GameObject.Find("Global/UI/");
            this.Load();
        }

        /// <summary>
        /// 正在退出到登陆界面中
        /// </summary>
        private bool isExiting = false;
        /// <summary>
        /// 退出到登陆
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void ExitToLogin(object obj)
        {
            if (UIRouterHelper.currSceneName == UIType.UILogin)
            {
                Debug.LogWarning("在login界面退出个寂寞？");
                return;
            }

            if (this.isExiting)
            {
                return;
            }
            this.isExiting = true;

            //重置本地信息
            try
            {
                foreach (string type in this.uis.Keys.ToArray())
                {
                    this.Remove(type);
                }

            }
            catch (System.Exception e)
            {
                Debug.LogError("ExitToLogin 重置本地信息 error:" + e);
            }

            try
            {
                // 退出登陆
                UIRouterHelper.Clear();
                await UIRouterHelper.LoadSceneRoutine(UIType.UILogin);
            }
            finally
            {
                this.isExiting = false;
            }
        }

        public void Load()
        {
            UiTypes.Clear();

            List<Type> types = Game.EventSystem.GetTypes();

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIFactoryAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (UiTypes.ContainsKey(attribute.Type))
                {
                    Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
                    throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
                }
                object o = Activator.CreateInstance(type);
                IUIFactory factory = o as IUIFactory;
                if (factory == null)
                {
                    Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                this.UiTypes.Add(attribute.Type, factory);
            }
        }

        public async Task<UI> Create(string type)
        {
            try
            {
                if (uis.ContainsKey(type))
                {
                    Log.Info("重复加载" + type);
                    Remove(type);
                }
                UI ui = await UiTypes[type].CreateAsync(this.GetParent<Scene>(), type, Root);
                uis.Add(type, ui);

                // 设置canvas
                string cavasName = ui.GameObject.GetComponent<CanvasConfig>().CanvasName;
                ui.GameObject.transform.SetParent(this.Root.Get<GameObject>(cavasName).transform, false);

                return ui;
            }
            catch (Exception e)
            {
                throw new Exception($"{type} UI 错误: {e}");
            }
        }

        public void Add(string type, UI ui)
        {
            this.uis.Add(type, ui);
        }


        public UI PreRemove(string type)
        {
            UI ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return null;
            }
            uis.Remove(type);
            return ui;
        }
        public void Remove(string type)
        {
            var ui = this.PreRemove(type);
            UiTypes[type].Remove(type);
            ui?.Dispose();
        }


        public void RemoveAll()
        {
            foreach (string type in this.uis.Keys.ToArray())
            {
                UI ui;
                if (!this.uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                UiTypes[type].Remove(type);
                this.uis.Remove(type);
                ui.Dispose();
            }
        }

        public UI Get(string type)
        {
            UI ui;
            this.uis.TryGetValue(type, out ui);
            return ui;
        }

        public List<string> GetUITypeList()
        {
            return new List<string>(this.uis.Keys);
        }
    }
}
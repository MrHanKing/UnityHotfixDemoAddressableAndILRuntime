using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ETModel
{
    public class Init : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                if (!Application.unityVersion.StartsWith("2019"))
                {
                    Log.Warning($"请使用Unity2019LTS版本,减少跑demo遇到的问题! ");
                }

                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

                DontDestroyOnLoad(gameObject);
                Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

                Game.Scene.AddComponent<GlobalConfigComponent>();
                Game.Scene.AddComponent<AddressableComponent>();
                Game.Scene.AddComponent<UIComponent>();

                // Todo UI和GlobalConfigComponent要解耦
                Game.EventSystem.Run(EventIdType.UIUpdateBundle);
                // 配置数据加载 大版本检查
                await GlobalConfigComponent.Instance.Run();

                // 更新流程
                await UIUpdateBundleComponent.Instance.Play();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void Update()
        {
            OneThreadSynchronizationContext.Instance.Update();
            Game.Hotfix.Update?.Invoke();
            Game.EventSystem.Update();

        }

        private void LateUpdate()
        {
            Game.Hotfix.LateUpdate?.Invoke();
            Game.EventSystem.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            Game.Hotfix.OnApplicationQuit?.Invoke();
            Game.Close();
        }

        private void OnApplicationPause(bool pause)
        {
            Game.Hotfix.OnApplicationPause?.Invoke(pause);
        }
    }
}
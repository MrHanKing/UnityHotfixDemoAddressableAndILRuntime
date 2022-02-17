using System;
using UnityEngine;
using System.Threading.Tasks;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UILogin)]
    public class UILoginFactory : IUIFactory
    {
        public async Task<UI> CreateAsync(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                GameObject bundleGameObject = await AddressableComponent.Instance.LoadSublevelAsset<GameObject>($"{type}.unity3d", $"{type}");
                GameObject login = UnityEngine.Object.Instantiate(bundleGameObject);
                login.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(login);
                ui.AddComponent<UILoginComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e.ToStr());
                return null;
            }
        }


        public void Remove(string type)
        {
            AddressableComponent.Instance.ReleaseSublevel($"{type}.unity3d");
        }
    }
}

using System;
using UnityEngine;
using System.Threading.Tasks;
namespace ETModel
{
    /// <summary>
    /// Todo 可以优化到父类去 减少重复代码
    /// </summary>
    [UIFactory(UIType.UIUpdateBundle)]
    public class UIUpdateBundleFactory : IUIFactory
    {
        GameObject bundleGameObject;
        GameObject go;

        public UI Create(Scene scene, string type, GameObject gameObject)
        {
            try
            {
                bundleGameObject = (GameObject)ResourcesHelper.Load("UI/UIUpdateBundle");
                go = UnityEngine.Object.Instantiate(bundleGameObject);
                go.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI ui = ComponentFactory.Create<UI, GameObject>(go);
                ui.AddComponent<UIUpdateBundleComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return null;
            }
        }


        public void Remove(string type)
        {

        }
    }
}


namespace ETModel
{
    [Event(EventIdType.UIUpdateBundle)]
    public class UIUpdateBundle_CreatUIUpdateBundle : AEvent
    {
        public override void Run()
        {
            Game.Scene.GetComponent<UIComponent>().Create(UIType.UIUpdateBundle);
        }
    }
}
    
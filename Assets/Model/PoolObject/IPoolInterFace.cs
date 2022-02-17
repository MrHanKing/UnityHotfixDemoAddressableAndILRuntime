using UnityEngine;
namespace ETModel
{
    public interface IPoolInterFace
    {
        void AddObject(string iName, GameObject go, float times = 0,string target = "");
        void HidePool(GameObject go,string PName);
        void UpdatePool();
    }
    
}

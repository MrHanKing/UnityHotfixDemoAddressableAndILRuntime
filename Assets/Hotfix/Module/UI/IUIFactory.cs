using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    public interface IUIFactory
    {
        // UI Create(Scene scene, string type, GameObject parent);
        Task<UI> CreateAsync(Scene scene, string type, GameObject parent);
        void Remove(string type);
    }
}
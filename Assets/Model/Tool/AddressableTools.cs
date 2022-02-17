

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using System;
using System.IO;

namespace ETModel
{
    public class AddressableTools
    {
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            float temeS = ((int)Convert.ToInt64(ts.TotalSeconds)) / 30000f;
            return temeS.ToString();
        }
        //Implement a method to transform the internal ids of locations
        static string MyCustomTransform(IResourceLocation location)
        {
            if (string.IsNullOrWhiteSpace(GlobalConfigComponent.Instance.defaultConfig.hotfixUrl))
            {
                return location.InternalId;
            }

            if ((location.ResourceType == typeof(IAssetBundleResource) || location.PrimaryKey == "AddressablesMainContentCatalogRemoteHash")
            && location.InternalId.StartsWith("http"))
            {
                // Debug.Log("all:" + location.InternalId);
                Uri myUri = new Uri(location.InternalId);
                var result = GlobalConfigComponent.Instance.defaultConfig.hotfixUrl + myUri.PathAndQuery + "?v=" + GetTimeStamp();
                // Debug.Log("result:" + result);
                return result;
            }

            return location.InternalId;
        }

        //Override the Addressables transform method with your custom method.  This can be set to null to revert to default behavior.
        [RuntimeInitializeOnLoadMethod]
        public static void SetInternalIdTransform()
        {
            // 劫持网络资源的寻址
            Addressables.InternalIdTransformFunc = MyCustomTransform;
        }
    }
}



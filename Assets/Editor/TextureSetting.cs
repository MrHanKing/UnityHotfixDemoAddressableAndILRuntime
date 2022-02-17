using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 图片导入自动处理成精灵图片
/// AssetPostprocessor 接口官网
/// https://docs.unity3d.com/cn/current/ScriptReference/AssetPostprocessor.html
/// </summary>
public class TextureSetting : AssetPostprocessor
{
    private static List<string> texturesWithoutMetaFile = new List<string>();
    private void OnPreprocessTexture()
    {
#if UNITY_2018_1_OR_NEWER
        bool customTextureSettingsExist = !assetImporter.importSettingsMissing;
#else
		bool customTextureSettingsExist = System.IO.File.Exists(assetImporter.assetPath + ".meta");
#endif
        if (!customTextureSettingsExist)
        {
            // 第一次导入的存起来
            texturesWithoutMetaFile.Add(assetImporter.assetPath);

        }
    }

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // 判断导入的文件不存在其他结尾的
        foreach (var assetPath in texturesWithoutMetaFile)
        {
            var result = Array.FindAll(importedAssets, value => value.StartsWith(assetPath));
            if (result.Length > 1 && !CheckExtensionIsPng(result))
            {
                // 
                continue;
            }

            // 是目标
            TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            if (textureImporter != null)
            {
                Debug.Log($"自动修改图片配置:{assetPath}");
                textureImporter.textureType = TextureImporterType.Sprite;
                // textureImporter.isReadable = false;
                textureImporter.mipmapEnabled = false;
                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            }

        }
        // 不然会处理多次
        texturesWithoutMetaFile.Clear();
    }

    /// <summary>
    /// 这组里是否有非png结尾的  有的话说明不是纯png文件 不自动修改配置
    /// </summary>
    /// <param name="groupPath"></param>
    /// <returns></returns>
    private static bool CheckExtensionIsPng(string[] groupPath)
    {
        foreach (var path in groupPath)
        {
            var extension = Path.GetExtension(path).ToLower();
            if (extension != ".png")
            {
                return false;
            }
        }
        return true;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;

namespace ETModel
{
    public class EditorCreateAudio : MonoBehaviour
    {
        //音效资源路径
        private static string audiosDir = "Assets/Res/Sound";
        //导出预制体路径
        private static string prefabDir = "Assets/Bundles/Independent/Audios/";
        // private static string prefabName = "Audio";
        /// <summary>
        /// 生成声音名字的映射文件
        /// </summary>
        private static string soundNameDir = "Assets/Hotfix/Module/Sound/";
        private static string soundNameCsName = "SoundName.cs";

        private static string GetTempName(string path)
        {
            System.IO.FileInfo _fi = new System.IO.FileInfo(path);
            var _tempName = _fi.Name.Replace(_fi.Extension, "").ToLower();
            return _tempName;
        }

        [MenuItem("HanJunTools/创建音效预设", priority = 1004)]
        static void CreateAudioPrefab()
        {
#if UNITY_EDITOR_WIN
            string[] _patterns = new string[] { "*.mp3", "*.wav", "*.ogg"};//识别不同的后缀名
#else 
            string[] _patterns = new string[] { "*.mp3", "*.wav", "*.ogg", "*.MP3" };//识别不同的后缀名
#endif
            List<string> _allFilePaths = new List<string>();
            foreach (var item in _patterns)
            {
                string[] _temp = Directory.GetFiles(audiosDir, item, SearchOption.AllDirectories);
                _allFilePaths.AddRange(_temp);
            }
            if (!System.IO.Directory.Exists(prefabDir))
                System.IO.Directory.CreateDirectory(prefabDir);

            foreach (var item in _allFilePaths)
            {
                var _tempName = GetTempName(item);
                AudioClip _clip = AssetDatabase.LoadAssetAtPath<AudioClip>(item);
                if (null != _clip)
                {
                    string path = $"{prefabDir}/{_tempName}.prefab";
                    // 存在不做任何处理 防止预制体内容被覆盖
                    if (System.IO.File.Exists(path))
                    {
                        Resources.UnloadAsset(_clip);
                        continue;
                    }

                    // 不存在就创建
                    GameObject _go = new GameObject();
                    _go.name = _tempName;
                    // AudioSource _as = _go.AddComponent<AudioSource>();
                    // _as.playOnAwake = false;
                    SoundData _data = _go.AddComponent<SoundData>();
                    // _data.audio = _as;
                    _data.audioClip = _clip;

                    var temp = PrefabUtility.SaveAsPrefabAsset(_go, path);

                    //添加ab标记
                    // AssetImporter importer = AssetImporter.GetAtPath(path);
                    // if (importer == null || temp == null)
                    // {
                    if (temp == null)
                    {
                        Debug.LogError("error: " + path);
                        return;
                    }
                    // 换用addreassable后不用设置bundleName了
                    // importer.assetBundleName = "sound";
                    // importer.assetBundleVariant = "unity3d";

                    GameObject.DestroyImmediate(_go);
                    EditorUtility.SetDirty(temp);
                    Resources.UnloadAsset(_clip);
                }
            }
            CreateScriptName(_allFilePaths);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建枚举脚本文件使用音效的名字 方便调用
        /// </summary>
        private static void CreateScriptName(List<string> allFilePaths)
        {
            if (!System.IO.Directory.Exists(soundNameDir))
                System.IO.Directory.CreateDirectory(soundNameDir);

            var resultCs = System.IO.Directory.GetFiles(soundNameDir, soundNameCsName);
            foreach (var i in resultCs)
            {
                System.IO.File.Delete(i);
            }
            var resultPath = System.IO.Path.Combine(soundNameDir, soundNameCsName);
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(resultPath, false, new UTF8Encoding(false)))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"
//由自动代码生成
namespace ETHotfix
{
    /// <summary>
    /// 音效名称
    /// </summary>
    public class SoundName
    { 
");
                foreach (var item in allFilePaths)
                {
                    var name = GetTempName(item);
                    string result = string.Format("        public const string {0} = \"{1}\";", name, name);
                    sb.AppendLine(result);
                }

                sb.Append(@"
    }
}");

                sw.Write(Regex.Replace(sb.ToString(), "(?<!\r)\n", "\r\n"));
                sw.Flush();
            }
        }
    }


}
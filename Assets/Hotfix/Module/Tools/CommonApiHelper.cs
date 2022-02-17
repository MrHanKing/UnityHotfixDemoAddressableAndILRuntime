using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// using DG.Tweening;
using ETModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 公用接口方便调用
    /// </summary>
    public class CommonApiHelper
    {
        /// <summary>
        /// 获取某一种标记属性的所有热更层的类
        /// 注意Attribute定义 在热更层不要传热更层的type进去 读取不到的！！！
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<Type> GetOneAttributeList<T>() where T : Attribute
        {
            List<Type> types = ETHotfix.Game.EventSystem.GetTypes();
            List<Type> result = new List<Type>();

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(T), false);

                if (attrs.Length == 0)
                {
                    continue;
                }

                T configAttribute = attrs[0] as T;
                // 只加载指定的配置
                if (configAttribute == null)
                {
                    continue;
                }

                result.Add(type);
            }
            return result;
        }

        /// <summary>
        /// 转化sgf到能提交给服务器的格式
        /// </summary>
        /// <param name="inputSgf"></param>
        /// <returns></returns>
        public static string ConverSgfToGiveServer(string inputSgf)
        {
            var result = inputSgf.Replace("\r", "").Replace("\n", "");
            return result;
        }

        /// <summary>
        /// 获得一个tween对应的异步资源
        /// tween的AsyncWaitForCompletion无论是否kill都会返回 没法判断完成和被提前kill的状态 所以得封一层TaskCompletionSource解决
        /// </summary>
        /// <param name="tween"></param>
        /// <returns></returns>
        // public static TaskCompletionSource<bool> GetDoTweenTaskComplition(Tween tween)
        // {
        //     var taskSource = new TaskCompletionSource<bool>();
        //     tween.AsyncWaitForCompletion().ContinueWith((task) =>
        //     {
        //         if (taskSource.Task.IsCompleted || taskSource.Task.IsCanceled)
        //         {
        //             return;
        //         }
        //         taskSource.TrySetResult(true);
        //     });
        //     return taskSource;
        // }

        /// <summary>
        /// MD5字符串加密
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }


        #region 适配ET框架 创建UI相关

        public static async Task<UI> Create(string type)
        {
            UI result = null;
            try
            {
                result = await Game.Scene.GetComponent<UIComponent>().Create(type);
            }
            catch (System.Exception e)
            {

                Debug.LogError("自动创建失败:" + e);
            }

            return result;
        }

        public static void Remove(string type)
        {
            try
            {
                Game.Scene.GetComponent<UIComponent>().Remove(type);
            }
            catch (System.Exception e)
            {
                Debug.LogError("自动删除失败:" + e);
            }
        }

        public static UIComponent GetUIComponent()
        {
            return Game.Scene.GetComponent<UIComponent>();
        }
        #endregion


        /// <summary>
        /// 修改TextMeshPro内容 并改变容器contentPanel的大小 适应TextMeshPro
        /// 注意 此方法不校准锚点对齐 只适配大小 请保证锚点统一
        /// </summary>
        /// <param name="inputStr">显示内容</param>
        /// <param name="textMeshPro">渲染的TextMesh组件</param>
        /// <param name="contentPanel">文字的外部包围背景组件</param>
        /// <param name="offSet">边框外扩大小 x 水平轴 y 垂直轴</param>
        /// <returns></returns>
        public static void RefreshTextMeshPro(string inputStr, TMP_Text textMeshPro, RectTransform contentPanel, Vector2 offSet)
        {
            if (textMeshPro != null)
            {
                textMeshPro.text = inputStr;
                var textBox = textMeshPro.GetPreferredValues();
                Log.Info("ha:" + textBox.ToString());
                contentPanel?.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textBox.x + offSet.x);
                contentPanel?.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textBox.y + offSet.y);
            }
        }
        /// <summary>
        /// 刷新VerticalLayout节点的高度
        /// 便于手动控制 contentfilter嵌套有问题
        /// </summary>
        /// <param name="verticalLayoutRoot"></param>
        public static void RefreshVerticalLayoutGroupHeight(GameObject verticalLayoutRoot)
        {
            var layout = verticalLayoutRoot.GetComponent<VerticalLayoutGroup>();
            layout?.CalculateLayoutInputHorizontal();
            layout?.CalculateLayoutInputVertical();
            (layout?.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, layout.preferredHeight);
        }
    }
}
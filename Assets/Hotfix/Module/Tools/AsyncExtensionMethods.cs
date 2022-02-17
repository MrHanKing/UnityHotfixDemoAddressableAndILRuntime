using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 异步方法扩展
    /// </summary>
    public static class ExtensionMethods
    {
        public static TaskAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += obj => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
        /// <summary>
        /// spine的异步
        /// </summary>
        /// <param name="asyncOp"></param>
        /// <returns></returns>
        // public static TaskAwaiter GetAwaiter(this Spine.TrackEntry asyncOp)
        // {
        //     var tcs = new TaskCompletionSource<object>();
        //     asyncOp.Complete += obj => { tcs.SetResult(null); };
        //     return ((Task)tcs.Task).GetAwaiter();
        // }

        /// <summary>
        /// 等待按钮被点击一次
        /// </summary>
        /// <param name="asyncOp"></param>
        /// <returns></returns>
        public static TaskCompletionSource<bool> ClickOnce(this Button asyncOp)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOp.onClick.AddListener(() =>
            {
                tcs.TrySetResult(true);
            });
            return tcs;
        }
    }
}
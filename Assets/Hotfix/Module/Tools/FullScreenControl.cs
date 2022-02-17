using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class FullScreenControlUpdateSystem : LateUpdateSystem<FullScreenControl>
    {
        public override void LateUpdate(FullScreenControl self)
        {
            self.LateUpdate();
        }
    }
    public class FullScreenControl : Component
    {
        /// <summary>
        /// 临时测试代码放的地方
        /// </summary>
        public void LateUpdate()
        {
            if (!Debug.isDebugBuild)
            {
                return;
            }
            // 下面的代码只在Development Build 启动

            if (Input.GetKey(KeyCode.F1))
            {
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            }
        }
    }
}
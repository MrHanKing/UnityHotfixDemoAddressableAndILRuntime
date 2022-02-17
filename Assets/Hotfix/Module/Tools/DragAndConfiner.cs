using UnityEngine;
using Lean.Common;
using Lean.Touch;
using ETModel;
using Cinemachine;

namespace ETHotfix
{
    [ObjectSystem]
    public class DragAndConfinerAwakeSystem : AwakeSystem<DragAndConfiner>
    {
        public override void Awake(DragAndConfiner self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class DragAndConfinerLateUpdateSystem : LateUpdateSystem<DragAndConfiner>
    {
        public override void LateUpdate(DragAndConfiner self)
        {
            self.LateUpdate();
        }
    }

    /// <summary>
    /// 带区域限制的拖拽
    /// </summary>
    public class DragAndConfiner : Component
    {
        /// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
        public LeanFingerFilter Use = new LeanFingerFilter(true);

        /// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
        public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

        /// <summary>The movement speed will be multiplied by this.
        /// -1 = Inverted Controls.</summary>
        public float Sensitivity { set { sensitivity = value; } get { return sensitivity; } }
        [SerializeField] private float sensitivity = 1.0f;

        /// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
        /// -1 = Instantly change.
        /// 1 = Slowly change.
        /// 10 = Quickly change.</summary>
        public float Damping { set { damping = value; } get { return damping; } }
        [SerializeField] private float damping = -1.0f;

        /// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
        /// NOTE: This requires <b>Damping</b> to be above 0.</summary>
        public float Inertia { set { inertia = value; } get { return inertia; } }
        [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

        [SerializeField]
        private Vector3 remainingDelta;

        private Transform transform;
        private GameObject gameObject;
        /// <summary>
        /// 摄像机的设置
        /// </summary>
        public LensSettings cameraSetting = LensSettings.Default;
        /// <summary>
        /// 约束包围盒
        /// </summary>
        public Collider2D m_BoundingShape2D;
        public bool fixY = true;
        public bool fixZ = true;

        /// <summary>This method moves the current GameObject to the center point of all selected objects.</summary>
        [ContextMenu("Move To Selection")]
        public virtual void MoveToSelection()
        {
            var center = default(Vector3);
            var count = 0;

            foreach (var selectable in LeanSelectable.Instances)
            {
                if (selectable.IsSelected == true)
                {
                    center += selectable.transform.position;
                    count += 1;
                }
            }

            if (count > 0)
            {
                var oldPosition = transform.localPosition;

                transform.position = center / count;

                remainingDelta += transform.localPosition - oldPosition;

                transform.localPosition = oldPosition;
            }
        }

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
        public void AddFinger(LeanFinger finger)
        {
            Use.AddFinger(finger);
        }

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
        public void RemoveFinger(LeanFinger finger)
        {
            Use.RemoveFinger(finger);
        }

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
        public void RemoveAllFingers()
        {
            Use.RemoveAllFingers();
        }

        public void Awake()
        {
            var ui = this.GetParent<UI>();
            if (ui == null)
            {
                Debug.LogError("DragAndConfiner 组件必须挂载在UI容器里面");
            }
            this.transform = ui.GameObject.transform;
            this.gameObject = ui.GameObject;

            Use.UpdateRequiredSelectable(gameObject);
        }

        public void LateUpdate()
        {
            // Get the fingers we want to use
            var fingers = Use.UpdateAndGetFingers();

            // Get the last and current screen point of all fingers
            var lastScreenPoint = LeanGesture.GetLastScreenCenter(fingers);
            var screenPoint = LeanGesture.GetScreenCenter(fingers);

            // Get the world delta of them after conversion
            var worldDelta = ScreenDepth.ConvertDelta(lastScreenPoint, screenPoint, gameObject);
            if (this.fixY)
            {
                worldDelta.y = 0;
            }
            if (this.fixZ)
            {
                worldDelta.z = 0;
            }

            // Store the current position
            var oldPosition = transform.localPosition;

            var targetPos = transform.position - worldDelta * sensitivity;
            // 检查是否超约束框
            if (this.CheckConfiner(targetPos))
            {
                return;
            }

            // Pan the camera based on the world delta
            transform.position = targetPos;

            // Add to remainingDelta
            remainingDelta += transform.localPosition - oldPosition;

            // Get t value
            var factor = LeanHelper.GetDampenFactor(damping, Time.deltaTime);

            // Dampen remainingDelta
            var newRemainingDelta = Vector3.Lerp(remainingDelta, Vector3.zero, factor);

            // Shift this position by the change in delta
            transform.localPosition = oldPosition + remainingDelta - newRemainingDelta;

            if (fingers.Count == 0 && inertia > 0.0f && damping > 0.0f)
            {
                newRemainingDelta = Vector3.Lerp(newRemainingDelta, remainingDelta, inertia);
            }

            // Update remainingDelta with the dampened value
            remainingDelta = newRemainingDelta;
        }

        /// <summary>
        /// 检查约束 
        /// </summary>
        /// <returns>true为超出约束框了</returns>
        private bool CheckConfiner(Vector3 centerPos)
        {
            if (this.cameraSetting.OrthographicSize != default(float) && this.m_BoundingShape2D != null)
            {
                float dy = this.cameraSetting.OrthographicSize;
                float dx = dy * this.cameraSetting.Aspect;
                Vector3 vx = Vector3.right * dx;
                Vector3 vy = Vector3.up * dy;
                if (!ContainedPoint((centerPos - vy) - vx))
                    return true;
                if (!ContainedPoint((centerPos + vy) + vx))
                    return true;
                if (!ContainedPoint((centerPos - vy) + vx))
                    return true;
                if (!ContainedPoint((centerPos + vy) - vx))
                    return true;
            }
            return false;
        }

        private bool ContainedPoint(Vector3 pos)
        {
            return this.m_BoundingShape2D.OverlapPoint(pos);
        }
    }

}

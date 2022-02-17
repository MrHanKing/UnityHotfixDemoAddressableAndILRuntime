using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{
    public abstract class Component : Object, IDisposable, IComponentSerialize
    {
        // 只有Game.EventSystem.Add方法中会设置该值，如果new出来的对象不想加入Game.EventSystem中，则需要自己在构造函数中设置
        public long InstanceId { get; private set; }

        private bool isFromPool;

        public bool IsFromPool
        {
            get
            {
                return this.isFromPool;
            }
            set
            {
                this.isFromPool = value;

                if (!this.isFromPool)
                {
                    return;
                }

                this.InstanceId = IdGenerater.GenerateId();
                Game.EventSystem.Add(this);
            }
        }

        public bool IsDisposed
        {
            get
            {
                return this.InstanceId == 0;
            }
        }
        public Component Parent { get; set; }

        public T GetParent<T>() where T : Component
        {
            return this.Parent as T;
        }

        public Entity Entity
        {
            get
            {
                return this.Parent as Entity;
            }
        }

        protected Component()
        {
            this.InstanceId = IdGenerater.GenerateId();
        }

        public virtual void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            // 触发Destroy事件
            Game.EventSystem.Destroy(this);

            Game.EventSystem.Remove(this.InstanceId);

            this.InstanceId = 0;

            if (this.IsFromPool)
            {
                Game.ObjectPool.Recycle(this);
            }
        }

        public virtual void BeginSerialize()
        {
        }

        public virtual void EndDeSerialize()
        {
        }
    }


    /// <summary>
    /// 异步初始化的组件
    /// </summary>
    public abstract class AsyncInitComponent : Component
    {
        //异步初始化 awake之后 
        public abstract Task Init();
    }
}
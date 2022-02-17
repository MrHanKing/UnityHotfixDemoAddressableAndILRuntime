using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// UI返回结果 
    /// </summary>
    public interface IDelegateResultBase { };
    /// <summary>
    /// UI输入参数
    /// </summary>
    public interface IDelegateInputDataBase { };
    /// <summary>
    /// 空返回 表示没有这个界面
    /// </summary>
    public class DelegateResultNull : IDelegateResultBase { }


    /// <summary>
    /// 2021.09.15 该数据结构调整为 通用的输出返回 
    /// </summary>
    public class DelegateResultUIPropKuang : IDelegateResultBase
    {
        /// <summary>
        /// 正向选择为true 如:确定，同意等
        /// </summary>
        public bool choise = false;
        public DelegateResultUIPropKuang(bool choise)
        {
            this.choise = choise;
        }
    }

    /// <summary>
    /// UI层被异步调用的界面 小的委托界面
    /// 注意:继承的脚本名字 必须和SubUIType里面注册的名字匹配
    /// </summary>
    public abstract class UIManagerUIBaseEntity : Entity
    {
        protected string myRegisterName = "";

        /// <summary>
        /// 子类初始化需要调用这个接口
        /// </summary>
        public void Awake()
        {
            myRegisterName = this.GetType().Name;
            Log.Info("UIManagerUIBaseEntity Register Name:" + myRegisterName);
            // 注意 要被外部起掉的ui请在SubUIType中开放类型 需要Value = myRegisterName;
            UIManagerHelper.RegisterUI(myRegisterName, this);
        }

        public Task<IDelegateResultBase> PreShow(IDelegateInputDataBase input)
        {
            // 处理已存在的等待
            this.TryCancelCurrentTask();
            // 新异步
            this.action = new TaskCompletionSource<IDelegateResultBase>();

            return Show(input);
        }

        public abstract Task<IDelegateResultBase> Show(IDelegateInputDataBase input);

        public abstract Transform GetTransform();

        public override void Dispose()
        {
            UIManagerHelper.UnRegisterUI(myRegisterName);
            base.Dispose();
        }

        /// <summary>
        /// 寄存的异步资源
        /// </summary>
        protected TaskCompletionSource<IDelegateResultBase> action;
        public void TryCancelCurrentTask()
        {
            if (this.action != null)
            {
                var oldAction = this.action;
                this.action = null;
                oldAction.TrySetCanceled();
            }
        }

        public void TryResolveCurrentTask(bool result)
        {
            if (this.action != null)
            {
                var oldAction = this.action;
                this.action = null;
                oldAction.TrySetResult(new DelegateResultUIPropKuang(result));
            }
        }

        public void TryResolveCurrentTask(IDelegateResultBase result)
        {
            if (this.action != null)
            {
                var oldAction = this.action;
                this.action = null;
                oldAction.TrySetResult(result);
            }
        }
    }
}
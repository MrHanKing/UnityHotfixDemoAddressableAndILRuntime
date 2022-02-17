using System.Collections.Generic;
using System.Threading.Tasks;

namespace ETHotfix
{
    // <summary>
    /// 异步输入
    /// </summary>
    public class TaskInput
    {

    }
    /// <summary>
    /// 异步返回
    /// </summary>
    public class TaskOutput
    {

    }
    /// <summary>
    /// 异步空返回
    /// </summary>
    public class TaskNullOutPut : TaskOutput { }

    /// <summary>
    /// 旧的给websocket用的 后续请用新的MineTaskRun 这个不够抽象
    /// </summary>
    /// <typeparam name="K"></typeparam>
    public abstract class CustomTaskRun<K>
    {
        private TaskCompletionSource<TaskOutput> task;

        public Task<TaskOutput> Run(TaskInput input)
        {
            this.task = new TaskCompletionSource<TaskOutput>();
            // Todo Presend
            return this.task.Task;
        }

        public Task<TaskOutput> GetTask()
        {
            if (this.task != null)
            {
                return this.task.Task;
            }

            return Task.FromResult(new TaskOutput());
        }

        /// <summary>
        /// 结束异步
        /// </summary>
        /// <param name="result"></param>
        public void End(K result)
        {
            this.Handle(result);
        }

        public void Cancel()
        {
            if (this.task != null)
            {
                this.task.TrySetCanceled();
                this.task = null;
            }
        }

        /// <summary>
        /// 结束异步调用
        /// </summary>
        /// <param name="output"></param>
        protected void SetResult(TaskOutput output)
        {
            if (this.task != null)
            {
                this.task.TrySetResult(output);
                this.task = null;
            }
        }

        public abstract void PreSend(TaskInput body = null);
        public abstract void Handle(K output);
    }

    public class MineTaskRun<T> where T : TaskOutput
    {
        private TaskCompletionSource<T> action;
        public bool isValid
        {
            get
            {
                return this.action != null && (this.action.Task.Status >= TaskStatus.Created && this.action.Task.Status <= TaskStatus.WaitingForChildrenToComplete);
            }
        }
        public Task<T> GetTask()
        {
            if (this.action != null)
            {
                return this.action.Task;
            }

            return Task.FromResult<T>(null);
        }
        public Task<T> Run()
        {
            // 处理已存在的等待
            this.TryCancelCurrentTask();
            // 新异步
            this.action = new TaskCompletionSource<T>();

            return this.action.Task;
        }
        public void TryCancelCurrentTask()
        {
            if (this.action != null)
            {
                var oldAction = this.action;
                this.action = null;
                oldAction.TrySetCanceled();
            }
        }
        public void TryResolveCurrentTask(T result)
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
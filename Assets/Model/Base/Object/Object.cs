using System.ComponentModel;
using System.Threading.Tasks;

namespace ETModel
{
    public abstract class Object : ISupportInitialize
    {
        public virtual void BeginInit()
        {
        }
        public virtual Task BeginInitAsync()
        {
            return Task.CompletedTask;
        }

        public virtual void EndInit()
        {
        }

        public override string ToString()
        {
            return JsonHelper.ToJson(this);
        }
    }
}
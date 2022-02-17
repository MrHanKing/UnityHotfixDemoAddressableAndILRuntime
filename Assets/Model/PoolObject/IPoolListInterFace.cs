namespace ETModel
{
    public abstract class IPoolListInterFace
    {
        public abstract void AddList(object obj);
        public abstract void RemoveList(object obj);
        public abstract void UpdateList(PoolObject obj);
        public abstract void Clear();
    }
}


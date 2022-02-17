using ETModel;
using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public class DictionaryBase<T, K> 
    {
        private readonly Dictionary<T, K> dictionary = new Dictionary<T, K>();

       
        public Dictionary<T, K> GetDictionary()
        {
            return this.dictionary;
        }

        public K[] GetAll()
        {
           return dictionary.Values.ToArray();
        }

        public T[] GetKeys() 
        {
            return dictionary.Keys.ToArray();
        }

        public void Add(T t, K k)
        {
            K list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                dictionary.Add(t,k);
            }
            else
            {
                this.dictionary[t] = k;
            }
        }

       

        public K Get(T t)
        {
            K list;
            this.dictionary.TryGetValue(t, out list);
            return list;
        }

        public K GetThis(T t)
        {
           if (this.dictionary.ContainsKey(t))
                return this.dictionary[t];
            K list;
            this.dictionary.TryGetValue(t, out list);
            return list;
        }

        public T FirstKey()
        {
            return this.dictionary.Keys.First();
        }

        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }
        
        public bool Remove(T t, K k)
        {
            K list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                Log.Info("删除失败:"+t.GetType());
                return false;
            }
            this.dictionary.Remove(t);
            return true;
        }

        public bool Remove(T t)
        {
            K list;
            this.dictionary.TryGetValue(t, out list);
            if (list == null)
            {
                Log.Info("删除失败:" + t.GetType());
                return false;
            }
            return this.dictionary.Remove(t);
        }
        
        public bool ContainsKey(T t)
        {
            return this.dictionary.ContainsKey(t);
        }

        public void Clear()
        {
            dictionary.Clear();
        }
        
        public void Dispose()
        {
           foreach(K k in dictionary.Values)
            {
                Entity entity = k as Entity;
                if(entity != null)
                {
                    entity.Dispose();
                }
                Component component = k as Component;
                if(component != null)
                {
                    component.Dispose();
                }
                List<Entity> entities = k as List<Entity>;
                if(entities != null)
                {
                    foreach(Entity ety in entities)
                    {
                        ety.Dispose();
                    }
                }
                List<Component> components = k as List<Component>;
                if(components != null)
                {
                    foreach(Component cpt in components)
                    {
                        cpt.Dispose();
                    }
                }
            }
            dictionary.Clear();
         
        }
    }
}
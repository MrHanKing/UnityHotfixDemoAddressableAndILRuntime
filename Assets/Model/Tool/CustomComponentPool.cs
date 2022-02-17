using UnityEngine.Events;
using UnityEngine;
using System.Collections.Generic;

public class CustomComponentPool<T> where T : Component
{
    private readonly Stack<T> m_Stack = new Stack<T>();
    private readonly UnityAction<T> m_ActionOnGet;
    private readonly UnityAction<T> m_ActionOnRelease;
    private readonly GameObject m_gameObject;

    public int countAll { get; private set; }
    public int countActive { get { return countAll - countInactive; } }
    public int countInactive { get { return m_Stack.Count; } }

    public CustomComponentPool(GameObject gameObject, UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
    {
        m_ActionOnGet = actionOnGet;
        m_ActionOnRelease = actionOnRelease;
        m_gameObject = gameObject;
    }

    public T Get()
    {
        T element;
        if (m_Stack.Count == 0)
        {
            element = (T)m_gameObject.AddComponent(typeof(T));
            countAll++;
        }
        else
        {
            element = m_Stack.Pop();
        }
        if (m_ActionOnGet != null)
            m_ActionOnGet(element);
        return element;
    }

    public void Release(T element)
    {
        if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
            Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
        if (m_ActionOnRelease != null)
            m_ActionOnRelease(element);
        m_Stack.Push(element);
    }

    public void Dispose()
    {
        if (this.m_gameObject)
        {
            GameObject.Destroy(this.m_gameObject);
        }
    }
}

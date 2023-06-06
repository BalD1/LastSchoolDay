using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonoPool<T> where T : MonoBehaviour
{
    private Queue<T> pool = new Queue<T>();
    private Func<T> createAction;
    private int maxCapacity = -1;

    private Transform parentContainer;

    public MonoPool(Func<T> _createAction,  Transform _parentContainer, int initialCount = 10, int _maxCapacity = -1)
    {
        createAction = _createAction;
        parentContainer = _parentContainer;
        maxCapacity = _maxCapacity;

        T[] res = GetNextMultiple(initialCount);
        foreach (var item in res)
        {
            item.gameObject.SetActive(false);
        }

        pool = new Queue<T>();
        SceneManager.sceneUnloaded += (Scene s) => pool.Clear();
    }

    #region GetNextSingle
    public T GetNext()
    {
        if (pool == null) pool = new Queue<T>();
        T obj = null;
        bool isValid = false;

        if (pool.Count > 0)
        {
            do
            {
                obj = pool.Dequeue();
            }
            while (obj == null && pool.Count > 0);

            isValid = obj != null;
        }

        if (!isValid && (pool.Count <= maxCapacity || maxCapacity < 0))
        {
            obj = createAction?.Invoke();
            obj?.transform.SetParent(parentContainer, true);
        }

        obj?.gameObject.SetActive(true);

        return obj;
    }
    public T GetNext(Vector2 position)
    {
        T obj = GetNext();
        obj.transform.position = position;
        return obj;
    }
    public T GetNext(Quaternion rotation)
    {
        T obj = GetNext();
        obj.transform.rotation = rotation;
        return obj;
    }
    public T GetNext(Vector2 position, Quaternion rotation)
    {
        T obj = GetNext();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }
    #endregion

    #region GetNextMultipleNonAlloc
    public void GetNextMultiple(int count, out T[] objs)
    {
        objs = new T[count];
        for (int i = 0; i < count; i++)
        {
            objs[i] = GetNext();
        }
    }
    public void GetNextMultiple(int count, Vector2 position, out T[] objs)
    {
        objs = new T[count];
        for (int i = 0; i < count; i++)
        {
            objs[i] = GetNext();
            objs[i].transform.position = position;
        }
    }
    public void GetNextMultiple(int count, Quaternion rotation, out T[] objs)
    {
        objs = new T[count];
        for (int i = 0; i < count; i++)
        {
            objs[i] = GetNext();
            objs[i].transform.rotation = rotation;
        }
    }
    public void GetNextMultiple(int count, Vector2 position, Quaternion rotation, out T[] objs)
    {
        objs = new T[count];
        for (int i = 0; i < count; i++)
        {
            objs[i] = GetNext();
            objs[i].transform.SetPositionAndRotation(position, rotation);
        }
    }
    #endregion

    #region GetNextMultipleAlloc
    public T[] GetNextMultiple(int count)
    {
        T[] objs = new T[count];
        GetNextMultiple(count, out objs);
        return objs;
    }
    public T[] GetNextMultiple(int count, Vector2 position)
    {
        T[] objs = new T[count];
        GetNextMultiple(count, out objs);
        foreach (var item in objs)
        {
            item.transform.position = position;
        }
        return objs;
    }
    public T[] GetNextMultiple(int count, Quaternion rotation)
    {
        T[] objs = new T[count];
        GetNextMultiple(count, rotation, out objs);
        return objs;
    }
    public T[] GetNextMultiple(int count, Vector2 position, Quaternion rotation)
    {
        T[] objs = new T[count];
        GetNextMultiple(count, position, rotation, out objs);
        return objs;
    } 
    #endregion

    public void Enqueue(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
    public void ResetQueue() => pool.Clear();
}
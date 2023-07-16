using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviourEventsHandler, IDependency
    where T : Component
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance) return instance;

            T[] objs = FindObjectOfType(typeof(T)) as T[];
            if (objs == null)
            {
                LogsManager.Log(typeof(T), "There is none " + typeof(T) + " singleton found.", LogsManager.E_LogType.Error);
                return null;
            }
            if (objs.Length > 0) instance = objs[0];
            if (objs.Length > 1) LogsManager.Log(typeof(T), "There is more than one " + typeof(T) + " object.", LogsManager.E_LogType.Error);

            return instance;
        }
    }

    [SerializeField] private GameObject[] dependecies;

    public bool IsCreated() => instance != null;

    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        base.Awake();
        instance = this as T;
    }

    protected virtual void Start()
    {
        foreach (var item in dependecies)
        {
            if (!(item.GetComponent<IDependency>()).InstanceExists())
            {
                this.Log("No instance dependecy " + item.name + " of " + typeof(T) + " was found. Trying to create one...");
                GameObject res = item.Create();
                if (res) this.Log("Creation of " + item.name + " Succeeded.");
                else this.Log("Creation of " + item.name + " Failed.");
            }
        }
    }

    protected override void EventsSubscriber() { }

    protected override void EventsUnSubscriber() { }

    public bool InstanceExists()
    {
        return Instance != null;
    }

    public GameObject GetInstance()
    {
        return Instance.gameObject;
    }

    [Obsolete("Should use GetInstance instead, as there should not exist more than one Singleton Instance at once.")]
    public List<GameObject> GetInstances()
    {
        this.Log("Should not be using GetInstances on Singleton. Use GetInstance instead.", LogsManager.E_LogType.Warning);
        return new List<GameObject>(GetInstances());
    }
}
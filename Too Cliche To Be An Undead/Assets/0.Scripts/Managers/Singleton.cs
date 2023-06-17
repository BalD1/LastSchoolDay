using UnityEngine;

public abstract class Singleton<T> : MonoBehaviourEventsHandler
    where T : Component
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance) return instance;

            T[] objs = FindObjectOfType(typeof(T)) as T[];
            if (objs == null) return null;
            if (objs.Length > 0) instance = objs[0];

            if (objs.Length > 1) Debug.LogErrorFormat($"There is more than one {typeof(T)} object");

            return instance;
        }
    }

    public bool IsCreated() => instance != null;

    protected override void Awake()
    {
        base.Awake();
        instance = this as T;
    }

    protected override void EventsSubscriber() { }

    protected override void EventsUnSubscriber() { }
}
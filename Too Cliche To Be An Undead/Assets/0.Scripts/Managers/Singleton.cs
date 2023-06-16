using UnityEngine;

public class Singleton<T> : MonoBehaviourEventsHandler
    where T : Component
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance) return instance;

            T[] objs = FindObjectOfType(typeof(T)) as T[];

            if (objs.Length > 0) instance = objs[0];

            if (objs.Length > 1) Debug.LogErrorFormat($"There is more than one {typeof(T)} object");

            if (instance == null)
            {
                GameObject obj = new GameObject();
                instance = obj.AddComponent<T>();
            }

            return instance;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        instance = this as T;
    }

    protected override void EventsSubscriber() { }

    protected override void EventsUnSubscriber() { }
}


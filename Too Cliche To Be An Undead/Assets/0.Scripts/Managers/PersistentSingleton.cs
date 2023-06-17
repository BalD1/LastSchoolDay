using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class PersistentSingleton<T> : Singleton<T>
    where T : Component
{
    protected override void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        base.Awake();
        this.gameObject.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);

    }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);

    protected abstract void OnSceneUnloaded(Scene scene);
}
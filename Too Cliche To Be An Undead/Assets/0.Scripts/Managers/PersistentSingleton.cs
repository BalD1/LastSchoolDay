using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentSingleton<T> : Singleton<T>
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
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameVerManager : PersistentSingleton<GameVerManager>
{
    private static bool showVersion;

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        SetShowVersion(true);
#endif
    }

    public static void SetShowVersion(bool version)
    {
        showVersion = version;
    }

    private void OnGUI()
    {
        if (!showVersion) return;

        int lastSize = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = 30;
        Rect r = new Rect(10, Screen.height - 40, Screen.width, 250);
        GUI.Label(r, Application.version);
        GUI.skin.label.fontSize = lastSize;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVerManager : MonoBehaviour
{
    private static bool showVersion;

    private static GameVerManager instance;
    public static GameVerManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        this.transform.parent = null;
        DontDestroyOnLoad(this.gameObject);

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
}

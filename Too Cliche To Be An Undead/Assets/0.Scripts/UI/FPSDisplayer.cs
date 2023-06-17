using UnityEngine;
using System.Text;
using UnityEngine.SceneManagement;

public class FPSDisplayer : PersistentSingleton<FPSDisplayer>
{
    private float deltaTime;
    private int fps;
    private int lowestFPS;
    private int highestFPS;

    private const int fpsReset_COOLDOWN = 2;
    private float fpsReset_TIMER;

    [SerializeField] private bool run;

    private void Start()
    {
#if UNITY_EDITOR
        run = true;
#endif
        fpsReset_TIMER = fpsReset_COOLDOWN;
        ResetLowestAndHighest();
    }

    private void Update()
    {
        if (!run) return;

        CalculateFPS();

        fpsReset_TIMER -= Time.deltaTime;
        if (fpsReset_TIMER <= 0) ResetLowestAndHighest();
    }

    private void CalculateFPS()
    {
        deltaTime += Time.deltaTime;
        deltaTime /= 2;
        fps = (int)Mathf.Round(1 / deltaTime);

        if (fps < lowestFPS) lowestFPS = fps;
        if (fps > highestFPS) highestFPS = fps;
    }

    private void ResetLowestAndHighest()
    {
        lowestFPS = int.MaxValue;
        highestFPS = -1;
        fpsReset_TIMER = fpsReset_COOLDOWN;
    }

    public void SetState(bool state) => run = state;
    public bool IsRunning() => run;

    private void OnGUI()
    {
        if (!run) return;

        StringBuilder sbContent = new StringBuilder("FPS : ");
        sbContent.AppendLine(fps.ToString());

        sbContent.Append("Lowest : ");
        sbContent.AppendLine(lowestFPS.ToString());

        sbContent.Append("Highest : ");
        sbContent.AppendLine(highestFPS.ToString());

        GUI.Label(new Rect(10, 150, 80, 100), sbContent.ToString());
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
    }
}
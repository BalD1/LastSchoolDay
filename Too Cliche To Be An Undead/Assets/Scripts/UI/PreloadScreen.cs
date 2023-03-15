using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreloadScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private GameObject panelsManager;

    [SerializeField] private ButtonArgs_Scene sceneArgs;

    [SerializeField] private float waitBeforeInputs_DURATION =.5f;
    private float waitBeforeInputs_TIMER;

    PlayerCharacter p1;

    public void BeginScreen()
    {
        GameManager.Instance.allowQuitLobby = false;
        panelsManager.SetActive(false);
        waitBeforeInputs_TIMER = waitBeforeInputs_DURATION;
        p1 = GameManager.Player1Ref;

        p1.D_validateInput += PlayTutorial;
        p1.D_fourthActionButton += SkipTutorial;
        p1.D_cancelInput += Back;
    }

    private void Update()
    {
        if (waitBeforeInputs_TIMER > 0) waitBeforeInputs_TIMER -= Time.deltaTime;
    }

    public void PlayTutorial()
    {
        if (waitBeforeInputs_TIMER > 0) return;

        Launch(false);
    }

    public void SkipTutorial()
    {
        if (waitBeforeInputs_TIMER > 0) return;

        Launch(true);
    }

    private void Launch(bool skipTuto)
    {
        DataKeeper.Instance.alreadyPlayedTuto = skipTuto;
        DataKeeper.Instance.skipTuto = skipTuto;

        p1.D_validateInput -= PlayTutorial;
        p1.D_fourthActionButton -= SkipTutorial;
        p1.D_cancelInput -= Back;

        if (sceneArgs == null)
        {
            sceneArgs = new ButtonArgs_Scene();
            sceneArgs.args = GameManager.E_ScenesNames.MainScene;
        }

        loadingScreen.SetActive(true);
        loadingScreen.GetComponent<LoadingScreen>().StartLoad(sceneArgs);

        this.gameObject.SetActive(false);
    }

    public void Back()
    {
        if (waitBeforeInputs_TIMER > 0) return;

        panelsManager.SetActive(true);

        p1.D_validateInput -= PlayTutorial;
        p1.D_fourthActionButton -= SkipTutorial;
        p1.D_cancelInput -= Back;

        panelsManager.GetComponent<PlayerPanelsManager>().Refocus();

        this.gameObject.SetActive(false);
        GameManager.Instance.allowQuitLobby = true;
    }
}

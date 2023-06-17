using System;
using UnityEngine;

public class ManagerInstancier : MonoBehaviour
{
    [Header("Any Scene")]
    [SerializeField] private GameVerManager gameVerManagerPF;
    [SerializeField] private FPSDisplayer fpsDisplayerManagerPF;
    [SerializeField] private PlayerInputsManager inputsManagerPF;
    [SerializeField] private GameAssets gameAssetsPF;

    private GameObject pf;

    [Header("Main Menu")]

    [Header("IG")]
    [SerializeField] private IGPlayersManager igPlayersManagerPF;


    private void Awake()
    {
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.LoadingScreen)) return;
        CreateAnySceneManagers();
        if (GameManager.CompareCurrentScene(GameManager.E_ScenesNames.MainMenu))
        {
            CreateMainMenuManagers();
            return;
        }
        CreateIGManagers();
    }

    private void CreateAnySceneManagers()
    {
        if (PlayerInputsManager.Instance == null) inputsManagerPF?.Create(this.transform.parent);
        if (GameAssets.Instance == null) gameAssetsPF?.Create(this.transform.parent);
        if (FPSDisplayer.Instance == null) fpsDisplayerManagerPF?.Create(this.transform.parent);
        if (GameVerManager.Instance == null) gameVerManagerPF?.Create(this.transform.parent);
    }

    private void CreateMainMenuManagers()
    {
    }

    private void CreateIGManagers()
    {
        if (IGPlayersManager.Instance == null) igPlayersManagerPF?.Create(this.transform.parent);
    }
}

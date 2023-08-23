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
        if (!PlayerInputsManager.ST_InstanceExists()) inputsManagerPF?.Create(this.transform.parent);
        if (!GameAssets.ST_InstanceExists()) gameAssetsPF?.Create(this.transform.parent);
        if (!FPSDisplayer.ST_InstanceExists()) fpsDisplayerManagerPF?.Create(this.transform.parent);
        if (!GameVerManager.ST_InstanceExists()) gameVerManagerPF?.Create(this.transform.parent);
    }

    private void CreateMainMenuManagers()
    {
    }

    private void CreateIGManagers()
    {
        if (!IGPlayersManager.ST_InstanceExists()) igPlayersManagerPF?.Create(this.transform.parent);
    }
}

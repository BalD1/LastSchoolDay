using UnityEngine;

public class ManagerInstancier : MonoBehaviour
{
    [Header("Any Scene")]
    [SerializeField] private PlayerInputsManager inputsManagerPF;
    [SerializeField] private GameAssets gameAssetsPF;

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
        if (PlayerInputsManager.Instance == null)
            inputsManagerPF?.Create();

        if (GameAssets.Instance == null)
            gameAssetsPF?.Create();
    }

    private void CreateMainMenuManagers()
    {

    }

    private void CreateIGManagers()
    {
        if (IGPlayersManager.Instance == null)
            igPlayersManagerPF?.Create();
    }
}

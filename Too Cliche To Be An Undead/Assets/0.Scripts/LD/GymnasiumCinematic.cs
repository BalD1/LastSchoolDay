using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GymnasiumCinematic;

public class GymnasiumCinematic : MonoBehaviour
{
    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string entryDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string bossHeardDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string bossSpawnDialogue;

    [SerializeField] private Transform doorTarget;
    [SerializeField] private Transform bossTarget;

    [SerializeField] private Transform playersTeleportPosition;

    [SerializeField] private BossZombie boss;

    public delegate void D_CinematicEnded();
    public D_CinematicEnded D_cinematicEnded;

    public void Begin() => StartCoroutine(Cinematic());

    private IEnumerator Cinematic()
    {
        SpawnersManager.Instance.AllowSpawns(false);
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

        foreach (var item in GameManager.Instance.playersByName)
        {
            if (item.playerScript.StateManager.ToString() == "Dying") item.playerScript.AskRevive();
        }

        UIManager.Instance.SetBlackBars(true);
        UIManager.Instance.FadeAllHUD(false);
        UIManager.Instance.FadeScreen(true);

        yield return new WaitForSeconds(.7f);

        GameManager.Instance.InstantiatedEntitiesParent.gameObject.SetActive(false);

        UIManager.Instance.FadeScreen(false);

        yield return new WaitForSeconds(.7f);

        UIManager.Instance.SetMinimapActiveState(false);
        UIManager.Instance.KeycardContainer.gameObject.SetActive(false);
        UIManager.Instance.CoinsContainer.gameObject.SetActive(false);
        UIManager.Instance.StampTimer.SetActive(false);

        DialogueManager.Instance.TryStartDialogue(entryDialogue, () =>
        {
            CameraManager.Instance.MoveCamera(doorTarget.position, () =>
            {
                DialogueManager.Instance.TryStartDialogue(bossHeardDialogue, () =>
                {
                    CameraManager.Instance.MoveCamera(bossTarget.position, () =>
                    {
                        boss.PlayAppearAnimation(() =>
                        {
                            boss.GetComponent<BossZombie>().TargetClosestPlayer();

                            DialogueManager.Instance.TryStartDialogue(bossSpawnDialogue, () =>
                            {
                                UIManager.Instance.FadeScreen(true, () =>
                                {
                                    GameManager.Instance.TeleportAllPlayers(playersTeleportPosition.position);
                                    AreaTransitorManager.Instance.ForceHideCorridor();
                                    CameraManager.Instance.EndCinematic();

                                    UIManager.Instance.FadeScreen(false, () =>
                                    {
                                        UIManager.Instance.SetBlackBars(false);
                                        UIManager.Instance.FadeAllHUD(true);
                                        D_cinematicEnded?.Invoke();
                                        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
                                        boss.SetIsAppeared();
                                    });
                                });
                            });
                        });
                    });
                });
            });

            
        });
    }
}

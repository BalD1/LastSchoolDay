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
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

        UIManager.Instance.SetBlackBars(true);
        UIManager.Instance.FadeAllHUD(false);
        UIManager.Instance.FadeScreen(true);

        yield return new WaitForSeconds(.7f);

        GameManager.Instance.InstantiatedEntitiesParent.gameObject.SetActive(false);

        UIManager.Instance.FadeScreen(false);

        yield return new WaitForSeconds(.7f);

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
                                    CameraManager.Instance.EndCinematic();

                                    UIManager.Instance.FadeScreen(false, () =>
                                    {
                                        UIManager.Instance.SetBlackBars(false);
                                        UIManager.Instance.FadeAllHUD(true);
                                        DialogueManager.Instance.ForceStopDialogue();
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

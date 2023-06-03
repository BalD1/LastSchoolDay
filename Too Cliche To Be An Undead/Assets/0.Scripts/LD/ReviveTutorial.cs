using System.Collections.Generic;
using UnityEngine;

public class ReviveTutorial : MonoBehaviour
{
    [SerializeField] private SCRPT_SingleDialogue startDialogue;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Shirley;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Whitney;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Jason;
    [SerializeField] private SCRPT_SingleDialogue reviveDialogue_Nelson;

    [SerializeField] private YardDoor door;

    private PlayerCharacter victimPlayer;

    private Queue<LTDescr> tweensQueue = new Queue<LTDescr>();

    public void OnStart()
    {
        if (GameManager.Instance.PlayersCount <= 1)
        {
            door.OpenDoor();
            return;
        }
        GameManager.Instance.D_onPlayerIsSetup += SetupVictimPlayer;
        victimPlayer = GameManager.Instance.playersByName.RandomElement().playerScript;
    }

    private void SetupVictimPlayer(int id)
    {
        if (victimPlayer == null) return;
        if (id != victimPlayer.PlayerIndex) return;

        FSM_Player_Manager victimStateManager = victimPlayer.StateManager;
        victimPlayer.AnimationController.FlipSkeleton(false);
        victimStateManager.SwitchState<FSM_Player_Dying>(FSM_Player_Manager.E_PlayerState.Dying).SetAsFakeState();
        victimPlayer.transform.position = this.transform.position;
        victimPlayer.OnInteract += StartDialogue;
        GameManager.Instance.D_onPlayerIsSetup -= SetupVictimPlayer;
    }

    private void StartDialogue(GameObject interactor)
    {
        victimPlayer.OnInteract -= StartDialogue;
        interactor.GetComponent<PlayerInteractor>().PromptText.gameObject.SetActive(false);
        UIManager.Instance.FadeScreen(fadeOut: true, () =>
        {
            victimPlayer.Revive();
            victimPlayer.AnimationController.FlipSkeleton(interactor.transform.position.x > victimPlayer.transform.position.x);

            LeanTween.delayedCall(.25f, () =>
            {
                SCRPT_SingleDialogue dialogueToPlay = null;

                switch (victimPlayer.GetCharacterName())
                {
                    case GameManager.E_CharactersNames.Shirley:
                        dialogueToPlay = reviveDialogue_Shirley;
                        break;

                    case GameManager.E_CharactersNames.Whitney:
                        dialogueToPlay = reviveDialogue_Whitney;
                        break;

                    case GameManager.E_CharactersNames.Jason:
                        dialogueToPlay = reviveDialogue_Jason;
                        break;

                    case GameManager.E_CharactersNames.Nelson:
                        dialogueToPlay = reviveDialogue_Nelson;
                        break;

                    default:
                        dialogueToPlay = reviveDialogue_Shirley;
                        break;
                }

                UIManager.Instance.FadeScreen(fadeOut: false, () =>
                {
                    DialogueManager.Instance.TryStartDialogue(dialogueToPlay, () =>
                    {
                        DialogueManager.Instance.TryStartDialogue(startDialogue);
                    });
                    door.OpenDoor();
                });
            });
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameManager.Instance.PlayersCount > 1) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        DialogueManager.Instance.TryStartDialogue(startDialogue);
        Destroy(this.gameObject);
    }
    private void OnDestroy()
    {
        if (victimPlayer == null) return;
        victimPlayer.OnInteract -= StartDialogue;
    }
}

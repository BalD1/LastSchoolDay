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
        victimPlayer.AnimationController.FlipSkeleton(interactor.transform.position.x > victimPlayer.transform.position.x);
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

            case GameManager.E_CharactersNames.None:
                dialogueToPlay = reviveDialogue_Shirley;
                break; 
        }

        DialogueManager.Instance.TryStartDialogue(dialogueToPlay, () =>
        {
            DialogueManager.Instance.TryStartDialogue(startDialogue);
        });
        door.OpenDoor();
        victimPlayer.OnInteract -= StartDialogue;
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

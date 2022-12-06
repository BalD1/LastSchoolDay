using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [InspectorButton(nameof(DialogueManager.SearchAndUpdateDialogueList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string startDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string zombiesDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string cleanedRoomDialogue;

    [SerializeField] private OpenableDoor doorToCloseOnZombies;
    [SerializeField] private NormalZombie[] tutorialZombies;

    private int zombiesCount;

    private bool enteredInTriggerFlag = false;

    private void Awake()
    {
        foreach (var item in tutorialZombies)
        {
            item.SelfVision.SetVisionState(false);
            item.d_OnDeath += OnZombieDeath;
        }

        zombiesCount = tutorialZombies.Length;
    }

    private void Start()
    {
        if (DataKeeper.Instance.skipTuto)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void StartFirstDialogue() => DialogueManager.Instance.TryStartDialogue(startDialogue);
    public void StartZombiesDialogue()
    {
        doorToCloseOnZombies.Close(true);
        DialogueManager.Instance.TryStartDialogue(zombiesDialogue, ResetZombiesVision);
    }

    private void ResetZombiesVision()
    {
        foreach (var item in tutorialZombies) item.SelfVision.SetVisionState(true);
    }

    private void OnZombieDeath()
    {
        zombiesCount--;

        if (zombiesCount <= 0)
        {
            DialogueManager.Instance.TryStartDialogue(cleanedRoomDialogue, TeleportPlayersToInGameHUB);
        }
    }

    private void TeleportPlayersToInGameHUB()
    {
        PlayersManager.Instance.SetAllPlayersControlMap("Dialogue");

        // Wait for the screen to fade out
        UIManager.Instance.FadeScreen(true, () =>
        {
            // Teleport every players to the HUB
            List<GameManager.PlayersByName> players = GameManager.Instance.playersByName;
            Vector2 goalPos;

            for (int i = 0; i < players.Count; i++)
            {
                goalPos = GameManager.Instance.IngameSpawnPoints[i].position;
                players[i].playerScript.transform.position = goalPos;
            }

            // Fade the screen in, then re-enable players controls
            UIManager.Instance.FadeScreen(false, () =>
            {
                PlayersManager.Instance.SetAllPlayersControlMapToInGame();
                GameManager.Instance.IsInTutorial = false;
            });
        });

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() != null && enteredInTriggerFlag == false)
        {
            enteredInTriggerFlag = true;
            StartZombiesDialogue();
        }
    }
}

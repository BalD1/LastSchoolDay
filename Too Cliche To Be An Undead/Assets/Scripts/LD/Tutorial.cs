using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [InspectorButton(nameof(DelegateUpdateNmaesOnList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string startDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string zombiesDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string cleanedRoomDialogue;

    [SerializeField] private OpenableDoor doorToCloseOnZombies;
    [SerializeField] private NormalZombie[] tutorialZombies;

    [SerializeField] private Transform onZombiesPlayersTeleportGoal;

    private int zombiesCount;

    private bool enteredInTriggerFlag = false;

    private bool tutoFinished = false;

    private void Awake()
    {
        foreach (var item in tutorialZombies)
        {
            item.d_OnDeath += OnZombieDeath;
        }

        zombiesCount = tutorialZombies.Length;
        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void StartFirstDialogue() => DialogueManager.Instance.TryStartDialogue(startDialogue);
    public void StartZombiesDialogue()
    {
        doorToCloseOnZombies.Close();
        DialogueManager.Instance.TryStartDialogue(zombiesDialogue, EnableTutorialZombies);
    }

    private void EnableTutorialZombies()
    {
        GameManager.Instance.TeleportAllPlayers(onZombiesPlayersTeleportGoal.position);

        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(true);
            item.Vision.TargetClosestPlayer();
        }
    }

    private void OnZombieDeath()
    {
        if (tutoFinished) return;

        zombiesCount--;

        if (zombiesCount <= 0)
        {
            tutoFinished = true;
            foreach (var item in tutorialZombies)
            {
                item.d_OnDeath -= OnZombieDeath;
            }
            foreach (var item in GameManager.Instance.playersByName)
            {
                item.playerScript.StopGamepadShake();
            }
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
                DataKeeper.Instance.alreadyPlayedTuto = true;

                GameManager.Instance.ShopTuto.StartTutorial();
            });
        });

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() != null && enteredInTriggerFlag == false)
        {
            enteredInTriggerFlag = true;
            StartZombiesDialogue();
        }
    }

    private void DelegateUpdateNmaesOnList()
    {
        DialogueManager.SearchAndUpdateDialogueList();
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [InspectorButton(nameof(DelegateUpdateNmaesOnList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string zombiesDialogue;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string cleanedRoomDialogue;

    [SerializeField] private OpenableDoor doorToCloseOnZombies;
    [SerializeField] private NormalZombie[] tutorialZombies;

    [SerializeField] private List<Transform> onZombiesPlayersTeleportGoals;

    [SerializeField] private CanvasGroup[] tutorialsArray;
    private Queue<CanvasGroup> tutorialsQueue;

    [SerializeField] private ReviveTutorial reviveTutorial;

    [SerializeField] private float tutorialImagesAnimScaleMultiplier = 2;

    private Cinematic cinematic;

    private int zombiesCount;

    private bool enteredInTriggerFlag = false;

    private bool tutoFinished = false;


    private void Awake()
    {
        if (DataKeeper.Instance.skipTuto || DataKeeper.Instance.alreadyPlayedTuto)
        {
            Destroy(this.gameObject);
            Destroy(reviveTutorial.gameObject);
            return;
        }
        foreach (var item in tutorialZombies)
        {
            item.d_OnDeath += OnZombieDeath;
        }

        zombiesCount = tutorialZombies.Length;
        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(false);
        }

        tutorialsQueue = new Queue<CanvasGroup>();
        foreach (var item in tutorialsArray)
        {
            item.alpha = 0;
            tutorialsQueue.Enqueue(item);
        }
    }

    private void Start()
    {
        cinematic = new Cinematic();
    }

    public void AnimateNextTutorial() => AnimateNextTutorial(null);
    public void AnimateNextTutorial(Action actionToPlayAtEnd)
    {
        if (tutorialsQueue.Count <= 0) return;

        CanvasGroup cg = tutorialsQueue.Dequeue();

        Transform cgTransform = cg.transform;
        Vector2 basePos = cgTransform.localPosition;

        cgTransform.localPosition = Vector2.zero;
        cgTransform.localScale *= tutorialImagesAnimScaleMultiplier;

        cg.LeanAlpha(1, .5f).setOnComplete(() =>
        {
            LeanTween.delayedCall(1, () =>
            {
                cgTransform.LeanScale(Vector2.one, 2).setEaseInOutQuart().setIgnoreTimeScale(true);
                cg.transform.LeanMoveLocal(basePos, 2).setEaseInOutQuart().setIgnoreTimeScale(true).setOnComplete(actionToPlayAtEnd);
            });
        });
    }

    public void AnimateNextTutorialMultiple(int count, Action actionToPlayAtEnd = null)
    {
        float x = 0;
        for (int i = 0; i < count; i++)
        {
            CanvasGroup cg = tutorialsQueue.Dequeue();

            Transform cgTransform = cg.transform;
            Vector2 basePos = cgTransform.localPosition;

            // Get the size of the element
            float transformSizeX = cg.GetComponent<RectTransform>().rect.width * tutorialImagesAnimScaleMultiplier;

            // Calculate the offset
            // Add to the left if at first half of loop
            // Add to the right if at 2nd half of loop
            x = i < count / 2 ? x - transformSizeX : x + transformSizeX;

            // Apply offset on x
            cgTransform.localPosition = new Vector2(
                x: x * (i + 1),
                y: 0);
            cgTransform.localScale *= tutorialImagesAnimScaleMultiplier;

            cg.LeanAlpha(1, .5f).setOnComplete(() =>
            {
                LeanTween.delayedCall(1, () =>
                {
                    cgTransform.LeanScale(Vector2.one, 2).setEaseInOutQuart().setIgnoreTimeScale(true);
                    cg.transform.LeanMoveLocal(basePos, 2).setEaseInOutQuart().setIgnoreTimeScale(true).setOnComplete(actionToPlayAtEnd);
                });
            });
        }
    }

    public void StartZombiesDialogue()
    {
        doorToCloseOnZombies.Close();
        DialogueManager.Instance.TryStartDialogue(zombiesDialogue, true,EnableTutorialZombies);
    }

    private void EnableTutorialZombies()
    {
        GameManager.Instance.ForceSetAllPlayersStateTo(FSM_Player_Manager.E_PlayerState.Cinematic);
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

        foreach (var item in tutorialZombies)
        {
            item.gameObject.SetActive(true);
            item.Vision.TargetClosestPlayer();
        }

        AnimateNextTutorialMultiple(2, () =>
        {
            GameManager.Instance.GameState = GameManager.E_GameState.InGame;
            GameManager.Instance.ForceSetAllPlayersStateTo(FSM_Player_Manager.E_PlayerState.Idle);
        });
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
            DialogueManager.Instance.TryStartDialogue(cleanedRoomDialogue, true, TeleportPlayersToInGameHUB);
        }
    }

    private void TeleportPlayersToInGameHUB()
    {
        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.InLobby);

        // Wait for the screen to fade out
        UIManager.Instance.FadeScreen(true, () =>
        {
            UIManager.Instance.TutoHUD.gameObject.SetActive(false);
            // Teleport every players to the HUB
            List<GameManager.PlayersByName> players = GameManager.Instance.playersByName;
            Vector2 goalPos;

            for (int i = 0; i < players.Count; i++)
            {
                PlayerCharacter player = players[i].playerScript;

                player.SetAllVelocity(Vector2.zero);
                player.StateManager.ForceSetState(FSM_Player_Manager.E_PlayerState.Idle);

                goalPos = GameManager.Instance.IngameSpawnPoints[i].position;
                player.transform.position = goalPos;
            }

            // Fade the screen in, then re-enable players controls
            UIManager.Instance.FadeScreen(false, () =>
            {
                GameManager.Instance.IsInTutorial = false;
                DataKeeper.Instance.alreadyPlayedTuto = true;

                GameManager.Instance.ShopTuto.StartTutorial();
            });
        });
    }

    public void ForceEndTutorial() => TeleportPlayersToInGameHUB();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<PlayerCharacter>() != null && enteredInTriggerFlag == false)
        {
            enteredInTriggerFlag = true;
            //cinematic.MoveAllPlayersTo(onZombiesPlayersTeleportGoals);
            GameManager.Instance.MoveAllPlayers(onZombiesPlayersTeleportGoals, StartZombiesDialogue);
        }
    }

    private void DelegateUpdateNmaesOnList()
    {
        DialogueManager.SearchAndUpdateDialogueList();
    }
}

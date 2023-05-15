using Spine;
using Spine.Unity;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class HUBDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private GameObject outline;

    [SpineAnimation] [SerializeField] private string openAnimation, closeAnimation;

    private TrackEntry trackEntry;
    private float targetTime;

    [SerializeField] private Vector2 playerTPPos;
    [SerializeField] private Collider2D playerCounterTrigger;

    [SerializeField] private TextMeshPro playersCounter;
    private int currentCounter = 0;
    private int maxPlayers;

    private bool hasSpawnedKeys = false;

    private bool playAnimBackwards = false;

    private void Start()
    {
        targetTime = skeletonAnimation.skeleton.Data.FindAnimation(closeAnimation).Duration;

        maxPlayers = GameManager.Instance.PlayersCount;
        hasSpawnedKeys = false;
        UpdateText();
    }

    private void Update()
    {
        if (playAnimBackwards)
        {
            trackEntry.TimeScale = 0f;
            trackEntry.AnimationLast = 0f;
            trackEntry.TrackTime = targetTime;
            skeletonAnimation.state.Apply(skeletonAnimation.skeleton);

            targetTime -= Time.deltaTime;

            if (targetTime <= 0) playAnimBackwards = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawnedKeys) return;

        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            if (currentCounter < maxPlayers)
            {
                currentCounter++;

                UpdateText();
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hasSpawnedKeys) return;

        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            currentCounter--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (playersCounter == null) return;

        StringBuilder sb = new StringBuilder();

        sb.Append(currentCounter);
        sb.Append(" / ");
        sb.Append(maxPlayers);

        playersCounter.text = sb.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerTPPos + (Vector2)this.transform.position, 0.5f);
    }

    public void EnteredInRange(GameObject interactor)
    {
        if (currentCounter > 0) return;

        outline.SetActive(true);
    }

    public void ExitedRange(GameObject interactor)
    {
        if (currentCounter > 0) return;

        outline.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (currentCounter < maxPlayers)
            return;

        StartCoroutine(FadeAndTeleport());
        outline.SetActive(false);
    }

    private IEnumerator FadeAndTeleport()
    {
        // Prevent players from moving
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

        // fade out screen
        float fadeTime = .5f;

        skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false);
        UIManager.Instance.FadeScreen(true, fadeTime);

        yield return new WaitForSeconds(fadeTime);

        // Deactivate door's componenets
        playerCounterTrigger.enabled = false;
        playersCounter.gameObject.SetActive(false);

        // Teleport every players
        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.gameObject.transform.position = playerTPPos + (Vector2)this.transform.position;

            FSM_Player_Manager stateManager = item.playerScript.StateManager;
            stateManager.SwitchState(stateManager.idleState);
        }

        // Setup Scene
        SpawnersManager.Instance?.ManageKeycardSpawn();
        hasSpawnedKeys = true;

        UIManager.Instance.KeycardContainer.SetActive(true);

        GameManager.Instance._onRunStarted?.Invoke();

        yield return new WaitForSeconds(fadeTime);

        // Fade in screen
        UIManager.Instance.FadeScreen(false, fadeTime);
        skeletonAnimation.AnimationState.SetAnimation(0, closeAnimation, false);

        trackEntry = skeletonAnimation.AnimationState.Tracks.Items[0];
        playAnimBackwards = true;

        yield return new WaitForSeconds(fadeTime);

        // Re-enable players controllers
        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
        SpawnersManager.Instance?.AllowSpawns(true);

        SoundManager.Instance.PlayMusic(SoundManager.E_MusicClipsTags.MainScene);
    }

    public bool CanBeInteractedWith()
    {
        return !hasSpawnedKeys;
    }

    public float GetDistanceFrom(Transform target)
    {
        return Vector2.Distance(target.position, this.transform.position);
    }
}

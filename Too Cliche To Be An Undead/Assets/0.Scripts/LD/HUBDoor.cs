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

    private Cinematic teleportToIGCinematic;

    private int currentCounter = 0;
    private int maxPlayers;

    private bool hasSpawnedKeys = false;

    private bool playAnimBackwards = false;

    private void Start()
    {
        targetTime = skeletonAnimation.skeleton.Data.FindAnimation(closeAnimation).Duration;
        maxPlayers = PlayerInputsManager.PlayersCount;
        hasSpawnedKeys = false;
        UpdateText();
        BuildCinematic();
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

    private void BuildCinematic()
    {
        teleportToIGCinematic = new Cinematic(
            new CA_CinematicActionMultiple(
                new CA_CinematicScreenFade(_fadeIn: false, .5f),
                new CA_CinematicCustomAction(() => skeletonAnimation.AnimationState.SetAnimation(0, openAnimation, false))
            ),
            new CA_CinematicWait(.25f),
            new CA_CinematicCameraMove(Camera.main, (playerTPPos + (Vector2)this.transform.position)), 
            new CA_CinematicPlayersMove((playerTPPos + (Vector2)this.transform.position), true, true),
            new CA_CinematicScreenFade(_fadeIn: true, .5f),
            new CA_CinematicWait(.5f),
            new CA_CinematicCustomAction(() => this.InteractedWithDoor())
            ).SetPlayers(IGPlayersManager.Instance.PlayersList);
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

        teleportToIGCinematic.StartCinematic();
        outline.SetActive(false);
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

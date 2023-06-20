using UnityEngine;

public class ShopDialogueAfterFirstDeath : DialogueTrigger
{
    [SerializeField] private Shop shop;
    private void Start()
    {
        if (DataKeeper.Instance.runsCount == 2) return;
        Destroy(this.gameObject);
    }
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (DataKeeper.Instance.runsCount != 2) return;

        if (collision.CompareTag("Player") && !triggerFlag)
        {
            triggerFlag = true;
            PlayersManager.Instance.SetAllPlayersControlMapToDialogue();
            StartCinematic();
        }
    }

    private void StartCinematic()
    {
        LeanTween.delayedCall(1, () =>
        {
            CameraManager.Instance.MoveCamera(shop.transform.position, OnCameraEndedTravel);
        });
    }

    private void OnCameraEndedTravel()
    {
        DialogueManager.Instance.TryStartDialogue(dialogueToStart, true, OnDialogueEnded);
        Destroy(this.gameObject);
    }

    private void OnDialogueEnded()
    {
            // TODO : Replace by simply deactivating players inputs
        PlayersManager.Instance.SetAllPlayersControlMapToDialogue();
        CameraManager.Instance.MoveCamera(GameManager.Player1Ref.transform.position, EndCinematic);
    }

    private void EndCinematic()
    {
        CameraManager.Instance.EndCinematic();
        PlayersManager.Instance.SetAllPlayersControlMapToInGame();
    }
}

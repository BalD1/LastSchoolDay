using UnityEngine;

public class ShopDialogueAfterFirstDeath : DialogueTrigger
{
    [SerializeField] private Shop shop;
    private Cinematic deathCinematic;

    private void Start()
    {
        if (DataKeeper.Instance.runsCount != 2)
        {
            Destroy(this.gameObject);
            return;
        }

        Vector2 p1Position = Vector2.zero;
        if (IGPlayersManager.ST_TryGetPlayer(0, out PlayerCharacter player))
            p1Position = player.transform.position;

        deathCinematic = new Cinematic(
            new CA_CinematicWait(1),
            new CA_CinematicCameraMove(shop.transform.position),
            new CA_CinematicDialoguePlayer(dialogueToStart),
            new CA_CinematicCameraMove(p1Position)
            );
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (DataKeeper.Instance.runsCount != 2) return;

        if (collision.CompareTag("Player") && !triggerFlag)
        {
            triggerFlag = true;
            StartCinematic();
        }
    }

    private void StartCinematic()
    {
        deathCinematic.StartCinematic();
    }
}

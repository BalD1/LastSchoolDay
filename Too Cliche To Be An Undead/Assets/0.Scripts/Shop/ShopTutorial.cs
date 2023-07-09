using UnityEngine;

public class ShopTutorial : MonoBehaviourEventsHandler
{
    [SerializeField] private SCRPT_SingleDialogue shopTutoDialogue;
    [SerializeField] private Transform shopTransform;

    private Cinematic shopTutoCinematic;
    private Cinematic shopClosedCinematic;

    private Shop shop;

    private void Start()
    {
        shop = GameManager.Instance.GetShop;
        BuildStartCinematic();
        BuildShopClosedCinematic();
    }

    private void BuildStartCinematic()
    {
        shopTutoCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        shopTutoCinematic.AddActions(
            new CA_CinematicCameraMove(Camera.main, shop.transform.position, 2),
            new CA_CinematicDialoguePlayer(shopTutoDialogue),
            new CA_CinematicCustomAction(shop.OpenShop)
            );
    }

    private void BuildShopClosedCinematic()
    {
        shopClosedCinematic = new Cinematic().SetPlayers(IGPlayersManager.Instance.PlayersList);
        shopTutoCinematic.AddActions(
            new CA_CinematicCameraMove(Camera.main, IGPlayersManager.Instance.PlayersList[0].transform.position, 2)
            );
    }

    protected override void EventsSubscriber()
    {
        TutorialEvents.OnTutorialEnded += StartTutorial;
    }

    protected override void EventsUnSubscriber()
    {
        TutorialEvents.OnTutorialEnded -= StartTutorial;
        ShopEvents.OnCloseShop -= OnShopClosed;
    }

    public void StartTutorial()
    {
        ShopEvents.OnCloseShop += OnShopClosed;
        shopTutoCinematic.StartCinematic();
    }

    private void OnShopClosed()
    {
        ShopEvents.OnCloseShop -= OnShopClosed;
        shopClosedCinematic.StartCinematic();
    }
}

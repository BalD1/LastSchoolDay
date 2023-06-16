using UnityEngine;
using UnityEngine.UI;

public class ImageChangeOnController : MonoBehaviourEventsHandler
{
    [SerializeField] private PlayerCharacter targetPlayer;

    [SerializeField] private Image image;

    [SerializeField] private ButtonsImageByDevice.E_ButtonType buttonType;

    protected override void EventsSubscriber()
    {
        PlayerInputsEvents.OnDeviceChange += CheckDevice;
    }

    protected override void EventsUnSubscriber()
    {
        PlayerInputsEvents.OnDeviceChange -= CheckDevice;
    }

    private void Start()
    {
        if (targetPlayer == null) targetPlayer = GameManager.Player1Ref;
        CheckDevice(PlayerInputsManager.CurrentPlayer1Device);
    }

    private void CheckDevice(PlayerInputsManager.E_Devices device)
    {
        image.sprite = ButtonsImageByDevice.Instance.GetButtonImage(buttonType, device);
    }

    private void OnValidate()
    {
        if (image == null) image = this.GetComponent<Image>();
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ImageChangeOnController : MonoBehaviourEventsHandler
{
    [SerializeField] protected PlayerCharacter targetPlayer;

    [SerializeField] private Image image;

    [SerializeField] protected SO_InputImagesByDevices imagesHolder;
    [SerializeField] protected SO_InputImagesByDevices.E_ButtonType btnType;
    [SerializeField] private ButtonsImageByDevice.E_ButtonType buttonType;

    protected override void EventsSubscriber()
    {
        PlayerInputsEvents.OnDeviceChange += CheckDevice;
    }

    protected override void EventsUnSubscriber()
    {
        PlayerInputsEvents.OnDeviceChange -= CheckDevice;
    }

    protected virtual void Start()
    {
        CheckDevice(PlayerInputsManager.CurrentPlayer1Device);
    }

    protected virtual void CheckDevice(PlayerInputsManager.E_Devices device)
    {
        if (imagesHolder == null) Debug.Log(this.gameObject.name, this.gameObject);
        image.sprite = imagesHolder.GetButtonImage(btnType, device);
    }

    private void OnValidate()
    {
        if (image == null) image = this.GetComponent<Image>();
    }
}

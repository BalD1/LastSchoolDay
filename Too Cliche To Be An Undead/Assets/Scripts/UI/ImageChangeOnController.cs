using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using static ButtonsImageByDevice;

public class ImageChangeOnController : MonoBehaviour
{
    [SerializeField] private PlayerCharacter targetPlayer;

    [SerializeField] private Image image;

    [SerializeField] private ButtonsImageByDevice.E_ButtonType buttonType;

    private void Start()
    {
        if (targetPlayer == null) targetPlayer = GameManager.Player1Ref;

        targetPlayer.D_onDeviceChange += CheckDevice;

        CheckDevice(targetPlayer.currentDeviceType);
    }

    private void CheckDevice(PlayerCharacter.E_Devices device)
    {
        image.sprite = ButtonsImageByDevice.Instance.GetButtonImage(buttonType, device);
    }

    private void OnDestroy()
    {
        targetPlayer.D_onDeviceChange -= CheckDevice;
    }

    private void OnValidate()
    {
        if (image == null) image = this.GetComponent<Image>();
    }
}

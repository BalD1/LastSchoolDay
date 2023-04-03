using UnityEngine;

public class ButtonsImageByDevice : MonoBehaviour
{
    private static ButtonsImageByDevice instance;
    public static ButtonsImageByDevice Instance
    {
        get => instance;
    }

    private void Awake()
    {
        instance = this;
    }

    [field: SerializeField] public S_ImageByDevice[] BackButtonImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] ValidateButtonImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] ThirdButtonImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] FourthButtonImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] SecondaryContextualImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] DialogueLineSkipImagesByDevice { get; private set; }
    [field: SerializeField] public S_ImageByDevice[] DialogueWholeSkipImagesByDevice { get; private set; }

    [System.Serializable]
    public struct S_ImageByDevice
    {
        public PlayerCharacter.E_Devices device;
        public Sprite buttonImage;
    }

    public enum E_ButtonType
    {
        Back,
        Validate,
        Third,
        Fourth,
        SecondaryContextual,
        DialogueLineSkip,
        DialogeWholeSkip,
    }

    public Sprite GetButtonImage(E_ButtonType buttonType, PlayerCharacter.E_Devices device)
    {
        switch (buttonType)
        {
            case E_ButtonType.Back:
                return GetImageFromArray(device, BackButtonImagesByDevice);

            case E_ButtonType.Validate:
                return GetImageFromArray(device, ValidateButtonImagesByDevice);

            case E_ButtonType.Third:
                return GetImageFromArray(device, ThirdButtonImagesByDevice);

            case E_ButtonType.Fourth:
                return GetImageFromArray(device, FourthButtonImagesByDevice);

            case E_ButtonType.SecondaryContextual:
                return GetImageFromArray(device, SecondaryContextualImagesByDevice);

            case E_ButtonType.DialogueLineSkip:
                return GetImageFromArray(device, DialogueLineSkipImagesByDevice);

            case E_ButtonType.DialogeWholeSkip:
                return GetImageFromArray(device, DialogueWholeSkipImagesByDevice);
        }

        return null;
    }

    private Sprite GetImageFromArray(PlayerCharacter.E_Devices device, S_ImageByDevice[] arr)
    {
        foreach (var item in arr)
        {
            if (device == item.device) return item.buttonImage;
        }

        return arr[0].buttonImage;
    }
}

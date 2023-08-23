using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Devices Buttons Images", menuName = "Scriptable/Assets/DevicesBtnImages")]
public class SO_InputImagesByDevices : ScriptableObject
{
    
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> BackButtonImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> ValidateButtonImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> ThirdButtonImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> FourthButtonImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> SecondaryContextualImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> DialogueLineSkipImagesByDevice { get; private set; }
    [field: SerializeField] public SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> DialogueWholeSkipImagesByDevice { get; private set; }

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

    public Sprite GetButtonImage(E_ButtonType buttonType, PlayerInputsManager.E_Devices device)
    {
        switch (buttonType)
        {
            case E_ButtonType.Back:
                return GetSpriteFromDicitonary(BackButtonImagesByDevice, device);

            case E_ButtonType.Validate:
                return GetSpriteFromDicitonary(ValidateButtonImagesByDevice, device);

            case E_ButtonType.Third:
                return GetSpriteFromDicitonary(ThirdButtonImagesByDevice, device);

            case E_ButtonType.Fourth:
                return GetSpriteFromDicitonary(FourthButtonImagesByDevice, device);

            case E_ButtonType.SecondaryContextual:
                return GetSpriteFromDicitonary(SecondaryContextualImagesByDevice, device);

            case E_ButtonType.DialogueLineSkip:
                return GetSpriteFromDicitonary(DialogueLineSkipImagesByDevice, device);

            case E_ButtonType.DialogeWholeSkip:
                return GetSpriteFromDicitonary(DialogueWholeSkipImagesByDevice, device);
        }

        return null;
    }

    private Sprite GetSpriteFromDicitonary(SerializedDictionary<PlayerInputsManager.E_Devices, Sprite> dict, PlayerInputsManager.E_Devices key)
    {
        if (!dict.TryGetValue(key, out Sprite val))
        {
            this.Log("Could not find " + key + " in dictionary.");
            return null;
        }
        return val;
    }
}
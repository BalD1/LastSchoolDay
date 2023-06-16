using UnityEngine;

public class MainMenuOptionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject mainButtons;

    public void Begin()
    {
        PlayerInputsEvents.OnCancelButton += Close;
        mainButtons.SetActive(false);
    }

    public void Close(int idx)
    {
        if (idx != 0) return;
        PlayerInputsEvents.OnCancelButton -= Close;
        mainButtons.SetActive(true);
    }
}

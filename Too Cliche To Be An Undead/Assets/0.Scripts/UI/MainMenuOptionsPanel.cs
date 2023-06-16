using UnityEngine;

public class MainMenuOptionsPanel : MonoBehaviour
{
    [SerializeField] private GameObject mainButtons;

    public void Begin()
    {
        GameManager.Player1Ref.OnCancelInput += Close;
        mainButtons.SetActive(false);
    }

    public void Close()
    {
        GameManager.Player1Ref.OnCancelInput -= Close;
        mainButtons.SetActive(true);
    }
}

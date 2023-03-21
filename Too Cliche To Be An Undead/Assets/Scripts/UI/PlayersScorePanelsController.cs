using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersScorePanelsController : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel_PF;

    [SerializeField] private RectTransform playerPanelsContainer;

    [SerializeField] private TextMeshProUGUI completitionTime;

    [SerializeField] private WinScreen winScreen;

    [field: SerializeField] public Toggle PressToSkipToggle { get; private set; }

    private bool canSkipToNextScreen;

    [System.Serializable]
    public struct S_PlayerImages
    {
        public GameManager.E_CharactersNames character;
        public Sprite happyImage;
        public Sprite neutralImage;
        public Sprite sadImage;
    }

    [field: SerializeField] public S_PlayerImages[] PlayersImages { get; private set; }

    public void Begin()
    {
        int playersCount = GameManager.Instance.PlayersCount;

        for (int i = 0; i < playersCount; i++)
        {
            GameObject panel = playerPanel_PF.Create(playerPanelsContainer);
            PlayerScorePanel scorePanel = panel.GetComponent<PlayerScorePanel>();

            if (i == playersCount - 1) scorePanel.D_animationEnded += AllowNextScreen;

            PlayerCharacter player = GameManager.Instance.playersByName[i].playerScript;

            foreach (var item in PlayersImages)
            {
                if (item.character == player.GetCharacterName())
                {
                    scorePanel.Setup(item, Random.Range(0, 1000), Random.Range(0, 1000));
                    break;
                }
            }

        }
    }

    private void AllowNextScreen()
    {
        GameManager.Player1Ref.D_validateInput += ShowButtonsPanel;
        PressToSkipToggle.gameObject.SetActive(true);
    }

    public void ShowButtonsPanel()
    {
        GameManager.Player1Ref.D_validateInput -= ShowButtonsPanel;
        winScreen.ShowButtonsPanel();
    }
}

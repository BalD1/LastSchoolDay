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
            // Create panel
            GameObject panel = playerPanel_PF.Create(playerPanelsContainer);
            PlayerScorePanel scorePanel = panel.GetComponent<PlayerScorePanel>();

            // Listen to the end of the last panel animation
            if (i == playersCount - 1) scorePanel.D_animationEnded += AllowNextScreen;

            // get the i player
            PlayerCharacter player = GameManager.Instance.playersByName[i].playerScript;

            // get the images of the player's character
            foreach (var item in PlayersImages)
            {
                if (item.character == player.GetCharacterName())
                {
                    scorePanel.Setup(item, player);
                    break;
                }
            }
        }

        int completitionTimeSeconds = (int)(Time.time - GameManager.Instance.TimeAtRunStart);

        completitionTimeSeconds += Random.Range(10000, 10000000);

        System.TimeSpan time = System.TimeSpan.FromSeconds(completitionTimeSeconds);

        completitionTime.text = time.ToString(@"hh\:mm\:ss");
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

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

    [SerializeField] private float singleKillValue = 100;
    [SerializeField] private float singleDamageDealtValue = 10;
    [SerializeField] private float singleDamageTakenValue = -5;

    public float SingleKillValue { get => singleKillValue; } 
    public float SingleDamageDealtValue { get => singleDamageDealtValue; }
    public float SingleDamageTakenValue { get => singleDamageTakenValue; }

    [field: SerializeField] public Toggle PressToSkipToggle { get; private set; }

    private bool canSkipToNextScreen;

    private Queue<PlayerScorePanel> panelsQueue;

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
        panelsQueue = new Queue<PlayerScorePanel>();
        int playersCount = GameManager.Instance.PlayersCount;

        for (int i = 0; i < playersCount; i++)
        {
            // Create panel
            GameObject panel = playerPanel_PF.Create(playerPanelsContainer);
            PlayerScorePanel scorePanel = panel.GetComponent<PlayerScorePanel>();

            panelsQueue.Enqueue(scorePanel);

            scorePanel.D_animationEnded += PlayNextPanel;

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

        PlayNextPanel();

        int completitionTimeSeconds = (int)(Time.time - GameManager.Instance.TimeAtRunStart);

        System.TimeSpan time = System.TimeSpan.FromSeconds(completitionTimeSeconds);

        completitionTime.text = time.ToString(@"hh\:mm\:ss");
    }

    private void PlayNextPanel()
    {
        if (panelsQueue.Count > 0) panelsQueue.Dequeue().BeginAnim();
        else AllowNextScreen();
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

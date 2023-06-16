using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersScorePanelsController : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel_PF;

    [SerializeField] private RectTransform playerPanelsContainer;

    [SerializeField] private TextMeshProUGUI completitionTime;

    [SerializeField] private WinScreen winScreen;

    [SerializeField] private CanvasGroup blackFadeImg; 

    [SerializeField] private CanvasGroup selfGroup;

    [SerializeField] private int singleKillValue = 100;
    [SerializeField] private int singleDamageDealtValue = 10;
    [SerializeField] private int singleDamageTakenValue = -5;

    public int SingleKillValue { get => singleKillValue; } 
    public int SingleDamageDealtValue { get => singleDamageDealtValue; }
    public int SingleDamageTakenValue { get => singleDamageTakenValue; }

    [field: SerializeField] public Toggle PressToSkipToggle { get; private set; }

    private bool canSkipToNextScreen;

    private Queue<PlayerScorePanel> panelsQueue;

    private PlayerScorePanel[] panels;

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

        blackFadeImg.LeanAlpha(1, .2f).setIgnoreTimeScale(true);
        panels = new PlayerScorePanel[playersCount];

        for (int i = 0; i < playersCount; i++)
        {
            // Create panel
            GameObject panel = playerPanel_PF.Create(playerPanelsContainer);
            PlayerScorePanel scorePanel = panel.GetComponent<PlayerScorePanel>();

            panelsQueue.Enqueue(scorePanel);
            panels[i] = scorePanel;

            scorePanel.D_animationEnded += PlayNextPanel;

            // get the i player
            PlayerCharacter player = GameManager.Instance.playersByName[i].playerScript;

            // get the images of the player's character
            foreach (var item in PlayersImages)
            {
                if (item.character == player.GetCharacterName())
                {
                    scorePanel.Setup(item, player, this);
                    break;
                }
            }
        }

        PlayNextPanel();

        float completitionTimeSeconds = PlayerEndStatsManager.Instance.GameTime;

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
        int maxScore = int.MinValue;
        int minScore = int.MaxValue;

        int lowestScoreIdx = -1;
        int highestScoreIdx = -1;

        for (int i = 0; i < panels.Length; i++)
        {
            int score = panels[i].FinalScore;

            if (panels[i].FinalScore > maxScore)
            {
                maxScore = score;
                highestScoreIdx = i;
            }

            if (panels[i].FinalScore < minScore)
            {
                minScore = score;
                lowestScoreIdx = i;
            }
        }

        panels[lowestScoreIdx].SetImageToSad();
        panels[highestScoreIdx].SetImageToHappy();

        PlayerInputsEvents.OnValidateButton += ShowButtonsPanel;
        PressToSkipToggle.gameObject.SetActive(true);
        winScreen.winGroup.blocksRaycasts = true;
        winScreen.winGroup.interactable = true;
    }

    public void ShowButtonsPanel(int idx = -1)
    {
        PlayerInputsEvents.OnValidateButton -= ShowButtonsPanel;

        selfGroup.interactable = false;
        selfGroup.blocksRaycasts = false;

        winScreen.ShowButtonsPanel();
    }
}

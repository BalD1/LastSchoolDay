using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private RectTransform panel;

    [SerializeField] private Image panelImage;
    [SerializeField] private Color wrongColor;

    public Image PanelImage { get => panelImage; }

    [SerializeField] private Image[] playersTokens;

    [SerializeField] private List<S_TakenTokensByPlayerIndex> tokensQueue = new List<S_TakenTokensByPlayerIndex>();

    [field: SerializeField] public GameManager.E_CharactersNames associatedCharacter;

    [field: SerializeField] public bool isValid { get; private set; }

    public bool isEnabled = false;

    [System.Serializable] 
    private class S_TakenTokensByPlayerIndex
    {
        public int playerIndex;
        public Image token;

        public S_TakenTokensByPlayerIndex(int _playerIndex, Image _token)
        {
            playerIndex = _playerIndex;
            token = _token;
        }
    }

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    [SerializeField] private Vector3 scaleGoal = new Vector3(1.2f,1.2f,1.2f);

    [SerializeField] private float scaleTime = .4f;

    public PlayerPanelsManager panelsManager;
    [field: SerializeField] public int panelID { get; private set; }

    public void Enable()
    {
        ScaleUpAndDown();
    }

    public void JoinPanel(int id)
    {
        foreach (var item in playersTokens)
        {
            if (item.color.a == 0)
            {
                S_TakenTokensByPlayerIndex newToken = new S_TakenTokensByPlayerIndex(id, item);
                this.tokensQueue.Add(newToken);
                item.sprite = panelsManager.tokensByIndex[id];
                panelsManager.PlayerAssociatedCard[id] = this.panelID;
                item.SetAlpha(1);
                break;
            }
        }

        if (tokensQueue.Count > 1)
        {
            this.panelImage.color = wrongColor;
            isValid = false;
        }

        Enable();
    }

    public void QuitPanel(int id)
    {
        bool swapNexts = false;
        for (int i = 0; i < tokensQueue.Count; i++)
        {
            if (tokensQueue[i].playerIndex == id)
                swapNexts = true;

            if (!swapNexts) continue;

            if (i + 1 < tokensQueue.Count)
            {
                tokensQueue[i].playerIndex = tokensQueue[i + 1].playerIndex;
                tokensQueue[i].token.sprite = tokensQueue[i + 1].token.sprite;
            }
            else
                tokensQueue[i].token.SetAlpha(0);
        }

        tokensQueue.RemoveAt(tokensQueue.Count - 1);

        if (tokensQueue.Count > 1) return;

        if (this.isValid == false)
        {
            this.panelImage.color = Color.white;
            isValid = true;
        }
    }

    public void ResetPanel(bool tweenScale = true)
    {
        this.panelImage.color = Color.white;

        isEnabled = false;

        foreach (var item in playersTokens)
        {
            item.SetAlpha(0);
        }

        tokensQueue = new List<S_TakenTokensByPlayerIndex>();

        if (tweenScale)
            ScaleUpAndDown();
    }

    private void ScaleUpAndDown()
    {
        LeanTween.scale(panel, scaleGoal, scaleTime).setEase(inType).setIgnoreTimeScale(true).setOnComplete(
        () =>
        {
            LeanTween.scale(panel, Vector3.one, scaleTime).setEase(outType).setIgnoreTimeScale(true);
        });
    }

    private void SetImageOpacity(ref Image i, int opacity)
    {
        Color c = i.color;
        c.a = opacity;
        i.color = c;
    }
    private void SetImageOpacity(ref Button b, int opacity)
    {
        Color c = b.image.color;
        c.a = opacity;
        b.image.color = c;
    }

    public void OnPointerDown()
    {
        if (!isEnabled) return;
        panelsManager.JoinPanelIndex(0, this.panelID);
    }
}

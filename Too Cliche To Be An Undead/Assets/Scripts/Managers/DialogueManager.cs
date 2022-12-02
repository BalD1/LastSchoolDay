using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get => instance;
    }

    [SerializeField] private CanvasGroup dialogueContainer;

    [SerializeField] private float leanFadeTime = .2f;

    [SerializeField] private Image dialoguePortrait;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI pressKeyToContinue;

    [field: SerializeField] public SCRPT_SingleDialogue[] Dialogues { get; private set; }

    [SerializeField] [ReadOnly] private SCRPT_SingleDialogue currentDialogue;
    [SerializeField] [ReadOnly] private SCRPT_SingleDialogue.DialogueLine currentLine;
    [SerializeField] [ReadOnly] private int currentLineIndex = -1;

    private int unfinishedEffectsCount;
    private int UnfinishedEffectsCount
    {
        get => unfinishedEffectsCount;
        set
        {
            unfinishedEffectsCount = value;
            if (unfinishedEffectsCount <= 0) OnIsReadyToShowNextLine();
        }
    }

    private Queue<int> pauseOnIndexQueue = new Queue<int>();

    private void Awake()
    {
        instance = this;
    }

    public bool TryStartDialogue(string searchedID)
    {
        foreach (var item in Dialogues)
        {
            if (item.ID == searchedID)
            {
                StartDialogue(item);
                return true;
            }
        }

#if UNITY_EDITOR
        Debug.LogErrorFormat($"{searchedID} was not found in {Dialogues} array."); 
#endif
        return false;
    }

    public void StartDialogue(SCRPT_SingleDialogue dialogue)
    {
        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.SwitchControlMapToDialogue();
        }

        //PostproManager.Instance.SetBlurState(true);

        dialogueText.text = "";

        currentDialogue = dialogue;
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;
        UIManager.Instance.SetBlackBars(true, .3f);
        UIManager.Instance.FadeAllHUD(false);
        dialogueContainer.LeanAlpha(1f, leanFadeTime)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => ShowNextLine());
    }

    public void TryNextLine()
    {
        if (UnfinishedEffectsCount == 0) ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLineIndex == -1) currentLineIndex = 0;
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }
        pauseOnIndexQueue.Clear();
        UnfinishedEffectsCount = 0;

        currentLine = currentDialogue.dialogueLines[currentLineIndex];

        dialoguePortrait.sprite = currentLine.portrait;
        dialogueText.text = currentLine.textLine;

        foreach (var item in currentLine.effects)
        {
            ManageEffects(item);
        }

        if (UnfinishedEffectsCount <= 0)
            OnIsReadyToShowNextLine();
        else
            OnCantShowNextLine();

        currentLineIndex++;
    }

    private void OnCantShowNextLine()
    {
        LeanTween.value(pressKeyToContinue.alpha, 0, .2f).setOnUpdate(
            (float val) =>
            {
                pressKeyToContinue.alpha = val;
            }).setIgnoreTimeScale(true);
    }
    private void OnIsReadyToShowNextLine()
    {
        LeanTween.value(pressKeyToContinue.alpha, 1, .2f).setOnUpdate(
            (float val) =>
            {
                pressKeyToContinue.alpha = val;
            }).setIgnoreTimeScale(true).setOnComplete(() => LeanTween.delayedCall(1, () => PressKeyTextVisibleEffects()));
    }

    private void PressKeyTextVisibleEffects()
    {
        LeanTween.delayedCall(3, () => {

            if (pressKeyToContinue.alpha <= 0) return;

            LeanTween.scale(pressKeyToContinue.rectTransform, Vector3.one * 1.2f, 0.5f)
            .setEase(LeanTweenType.easeInOutBack).setOnComplete(() =>
            {
                LeanTween.scale(pressKeyToContinue.rectTransform, Vector3.one, 0.5f);
            });

        }).setRepeat(-1);
    }

    private void EndDialogue()
    {
        UIManager.Instance.SetBlackBars(false, .3f);
        UIManager.Instance.FadeAllHUD(true);
        dialogueContainer.LeanAlpha(0, leanFadeTime)
            .setIgnoreTimeScale(true)
            .setOnComplete(
            () =>
            {
                GameManager.Instance.GameState = GameManager.E_GameState.InGame;
            });

        ResetDialogue();
        PostproManager.Instance.SetBlurState(true);
    }

    private void ResetDialogue()
    {
        UnfinishedEffectsCount = 0;
        currentLineIndex = -1;
        pressKeyToContinue.alpha = 0;
        pauseOnIndexQueue.Clear();
    }

    private void ManageEffects(SCRPT_SingleDialogue.DialogueEffect lineEffect)
    {
        switch (lineEffect.effect)
        {
            case SCRPT_SingleDialogue.E_Effects.ProgressiveReveal:
                UnfinishedEffectsCount++;
                StartCoroutine(Reveal(lineEffect.value == 0 ? 0.025f : lineEffect.value));
                break;

            case SCRPT_SingleDialogue.E_Effects.PauseOnIndex:
                pauseOnIndexQueue.Enqueue((int)lineEffect.value);
                break;

            default:
                break;
        }
    }

    private IEnumerator Reveal(float time)
    {
        dialogueText.ForceMeshUpdate();
        int totalVisibleCharacters = dialogueText.textInfo.characterCount;
        int counter = 0;

        bool loop = true;

        while(loop)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);

            if (pauseOnIndexQueue.Count > 0) 
            {
                if (visibleCount == pauseOnIndexQueue.Peek())
                {
                    pauseOnIndexQueue.Dequeue();
                    yield return new WaitForSeconds(1);
                }
            }

            dialogueText.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
            {
                loop = false;
                UnfinishedEffectsCount--;
            }

            counter++;

            yield return new WaitForSeconds(time);
        }
    }
}

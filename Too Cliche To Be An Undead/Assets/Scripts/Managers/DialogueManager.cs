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

    [SerializeField] private bool isReadyToShowNextLine = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) TryStartDialogue("test");
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

        Debug.LogErrorFormat($"{searchedID} was not found in {Dialogues} array.");
        return false;
    }

    public void StartDialogue(SCRPT_SingleDialogue dialogue)
    {
        foreach (var item in GameManager.Instance.playersByName)
        {
            item.playerScript.SwitchControlMapToDialogue();
        }

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
        if (isReadyToShowNextLine) ShowNextLine();
    }

    private void ShowNextLine()
    {
        if (currentLineIndex == -1) currentLineIndex = 0;
        if (currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }
        LeanTween.textAlpha(pressKeyToContinue.rectTransform, 0, .2f);
        isReadyToShowNextLine = false;

        currentLine = currentDialogue.dialogueLines[currentLineIndex];

        dialoguePortrait.sprite = currentLine.portrait;
        dialogueText.text = currentLine.textLine;

        foreach (var item in currentLine.effects)
        {
            ManageEffects(item);
        }

        currentLineIndex++;
        isReadyToShowNextLine = true;
        LeanTween.textAlpha(pressKeyToContinue.rectTransform, 1, .2f);
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
    }

    private void ResetDialogue()
    {
        isReadyToShowNextLine = false;
        currentLineIndex = -1;
    }

    private void ManageEffects(SCRPT_SingleDialogue.E_Effects effect)
    {
        switch (effect)
        {
            case SCRPT_SingleDialogue.E_Effects.ProgressiveReveal:
                StartCoroutine(Reveal());
                break;

            default:
                break;
        }
    }

    private IEnumerator Reveal()
    {
        int totalVisibleCharacters = dialogueText.textInfo.characterCount;
        int counter = 0;

        bool loop = true;

        while(loop)
        {
            int visibleCount = counter % (totalVisibleCharacters + 1);

            dialogueText.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalVisibleCharacters)
                loop = false;

            counter++;

            yield return new WaitForSeconds(.5f);
        }
    }
}

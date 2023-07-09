using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviourEventsHandler
{
    private static DialogueManager instance;
    public static DialogueManager Instance
    {
        get => instance;
    }

    public static List<string> DialogueNamesList;

    [SerializeField] private CanvasGroup dialogueContainer;

    [SerializeField] private float leanFadeTime = .2f;

    [SerializeField] private float allowSkip_COOLDOWN = .15f;
    private float allowSkip_TIMER;

    [SerializeField] private float onStartSkipWait_DURATION = .3f;
    private float onStartSkipWait_TIMER;

    [SerializeField] private Image speakerName;
    [SerializeField] private Image dialoguePortrait;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private CanvasGroup pressKeyToContinue;
    [SerializeField] private RectTransform pressKeyToContinueRT;

    [SerializeField] private Button skipButton;

    public delegate void D_SkipDialogue();
    public D_SkipDialogue D_skipDialogue;

    public static bool IsDialogueActive { get; private set; }

    [field: SerializeField] public SCRPT_SingleDialogue[] Dialogues { get; private set; }

#if UNITY_EDITOR
    [InspectorButton(nameof(UpdateDialogueList), ButtonWidth = 300)][SerializeField] private bool updateNamesList;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueNamesList))]
    [SerializeField] private string dialogueNamePopupTest; 
#endif

    [SerializeField] [ReadOnly] private SCRPT_SingleDialogue currentDialogue;
    [SerializeField] [ReadOnly] private SCRPT_SingleDialogue.DialogueLine currentLine;
    [SerializeField] [ReadOnly] private int currentLineIndex = -1;

    private Action endDialogueAction;

    private Coroutine revealCoroutine;

    private bool fromCinematic = false;

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

    protected override void EventsSubscriber()
    {
        PlayerInputsEvents.OnTryNextLineCall += TryNextLine;
        PlayerInputsEvents.OnSkipDialogueCall += TrySkip;
    }

    protected override void EventsUnSubscriber()
    {
        PlayerInputsEvents.OnTryNextLineCall -= TryNextLine;
        PlayerInputsEvents.OnSkipDialogueCall -= TrySkip;
    }

    protected override void Awake()
    {
        base.Awake();
        instance = this;

        ResetDialogue();
    }

    private void Start()
    {
        skipButton?.onClick.AddListener(TrySkip);
    }

    private void Update()
    {
        if (allowSkip_TIMER > 0) allowSkip_TIMER -= Time.deltaTime;
        if (onStartSkipWait_TIMER > 0) onStartSkipWait_TIMER -= Time.deltaTime;
    }

    /// <summary>
    /// <para> Searchs in the list for the "<paramref name="searchedID"/>" dialogue, then plays it if found. </para>
    /// <para> Can trigger "<paramref name="actionAtDialogueEnd"/>" at the end of the dialogue. </para>
    /// </summary>
    /// <param name="searchedID"></param>
    /// <param name="actionAtDialogueEnd"></param>
    /// <returns>Returns <see langword="true"/> if the dialogue was found,  <see langword="false"/> otherwise.</returns>
    public bool TryStartDialogue(string searchedID, bool fromCinematic, Action actionAtDialogueEnd = null)
    {
        foreach (var item in Dialogues)
        {
            if (item.ID == searchedID)
            {
                return TryStartDialogue(item, fromCinematic, actionAtDialogueEnd);
            }
        }

#if UNITY_EDITOR
        Debug.LogErrorFormat($"{searchedID} was not found in {Dialogues} array."); 
#endif
        return false;
    }
    public bool TryStartDialogue(SCRPT_SingleDialogue dialogue, bool fromCinematic, Action actionAtDialogueEnd = null)
    {
        if (dialogue == null) return false;

        StartDialogue(dialogue, fromCinematic);

        endDialogueAction = actionAtDialogueEnd;
        return true;
    }

    public void TrySkip()
    {
        if (onStartSkipWait_TIMER > 0) return;
        D_skipDialogue?.Invoke();
        D_skipDialogue = null;
        EndDialogue();
    }

    /// <summary>
    /// Starts the dialogue "<paramref name="dialogue"/>", activating "cinematic" mode.
    /// </summary>
    /// <param name="dialogue"></param>
    public void StartDialogue(SCRPT_SingleDialogue dialogue, bool _fromCinematic)
    {
        this.StartedDialogue(_fromCinematic);

        fromCinematic = _fromCinematic;
        dialoguePortrait.enabled = false;
        speakerName.enabled = false;

        IsDialogueActive = true;

        onStartSkipWait_TIMER = onStartSkipWait_DURATION;

        dialogueText.text = "";

        currentDialogue = dialogue;

        // Fades in the dialogue container, then shows the first line
        dialogueContainer.LeanAlpha(1f, leanFadeTime)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => ShowNextLine());
    }

    /// <summary>
    /// <para> /!\ Should only be called through <seealso cref="PlayerCharacter.ContinueDialogue"/> /!\</para>
    /// <para> Shows the next line. If the current one's effects aren't finished, end them.</para>
    /// </summary>
    public void TryNextLine()
    {
        if (onStartSkipWait_TIMER > 0) return;
        if (currentDialogue == null || currentLine.textLine == null) return;
        if (UnfinishedEffectsCount > 0 && allowSkip_TIMER <= 0)
        {
            ForceStopEffects();

            UnfinishedEffectsCount = 0;

            return;
        }
        ShowNextLine();
    }

    /// <summary>
    /// Shows the next dialogue line, and plays it's effects if it haves any.
    /// </summary>
    private void ShowNextLine()
    {
        if (currentLineIndex == -1) currentLineIndex = 0;
        if (currentDialogue == null || currentDialogue.dialogueLines == null ||
            currentLineIndex >= currentDialogue.dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        currentLine.eventToPlayBeforeText?.Invoke();

        if (revealCoroutine != null)  StopCoroutine(revealCoroutine);

        pauseOnIndexQueue.Clear();
        UnfinishedEffectsCount = 0;

        currentLine = currentDialogue.dialogueLines[currentLineIndex];

        if (currentLine.speakerData == null)
        {
            dialoguePortrait.SetAlpha(0);
            speakerName.SetAlpha(0);
        }
        else
        {
            if (currentLine.speakerData.speakerPortraitImage != null)
            {
                SCRPT_PortraitsWithRect portraitRect = currentLine.speakerData.speakerPortraitImage;
            
                dialoguePortrait.sprite = currentLine.speakerData.speakerPortraitImage.portrait;
                dialoguePortrait.rectTransform.SetAnchorsAndOffset(portraitRect.offsetMin, portraitRect.offsetMax,
                                                                   portraitRect.anchorMin, portraitRect.anchorMax);
                dialoguePortrait.SetAlpha(1);
            }
            else dialoguePortrait.SetAlpha(0);
            
            dialoguePortrait.enabled = true;
            
            if (currentLine.speakerData.speakerNameImage != null)
            {
                speakerName.sprite = currentLine.speakerData.speakerNameImage;
                speakerName.SetAlpha(1);
            }
            else speakerName.SetAlpha(0);
            speakerName.enabled = true;
        }

        if (currentLine.audioToPlay.audioClip != null)
        {
            AudioclipPlayer.Create(currentLine.audioToPlay.position, currentLine.audioToPlay.audioClip);
        }

        dialogueText.text = currentLine.textLine;

        foreach (var item in currentLine.effects)
        {
            ManageEffects(item);
        }

        // If there is no effects, instantly ready the next line.
        if (UnfinishedEffectsCount <= 0)
            OnIsReadyToShowNextLine();
        else
            OnCantShowNextLine();

        currentLineIndex++;
    }

    private void OnCantShowNextLine()
    {
        allowSkip_TIMER = allowSkip_COOLDOWN;
        LeanTween.value(pressKeyToContinue.alpha, 0, .2f).setOnUpdate(
            (float val) =>
            {
                pressKeyToContinue.alpha = val;
            }).setIgnoreTimeScale(true);
    }
    private void OnIsReadyToShowNextLine()
    {
        currentLine.eventToPlayAfterText?.Invoke();

        LeanTween.cancel(pressKeyToContinue.gameObject);
        LeanTween.cancel(pressKeyToContinueRT.gameObject);
        LeanTween.value(pressKeyToContinue.alpha, 1, .2f).setOnUpdate(
            (float val) =>
            {
                pressKeyToContinue.alpha = val;
            }).setIgnoreTimeScale(true).setOnComplete(() => LeanTween.delayedCall(1, () => PressKeyTextVisibleEffects()));
    }

    /// <summary>
    /// Plays the animation of the "press key" text.
    /// </summary>
    private void PressKeyTextVisibleEffects()
    {
        if (pressKeyToContinue == null) return;

        LeanTween.scale(pressKeyToContinueRT, Vector3.one * 1.2f, 0.5f).setLoopPingPong();
    }

    /// <summary>
    /// Ends the dialogue, removing every cinematic effects and giving back player's controls.
    /// </summary>
    private void EndDialogue()
    {
        IsDialogueActive = false;

        dialogueContainer.LeanAlpha(0, leanFadeTime)
            .setIgnoreTimeScale(true)
            .setOnComplete(
            () =>
            {
                ResetDialogue();
                this.EndedDialogue(fromCinematic);
                Action copyAction = endDialogueAction;
                endDialogueAction = null;
                copyAction?.Invoke();
            });
    }

    public void ForceStopDialogue()
    {
        StopAllCoroutines();
        LeanTween.cancel(this.gameObject);

        dialogueContainer.alpha = 0;
        ResetDialogue();
        this.EndedDialogue(fromCinematic);
    }

    private void ResetDialogue()
    {
        currentDialogue = null;
        UnfinishedEffectsCount = 0;
        currentLineIndex = -1;
        pressKeyToContinue.alpha = 0;
        pauseOnIndexQueue.Clear();

        LeanTween.cancel(pressKeyToContinue.gameObject);
        LeanTween.cancel(pressKeyToContinueRT.gameObject);
    }

    private void ManageEffects(SCRPT_SingleDialogue.DialogueEffect lineEffect)
    {
        switch (lineEffect.effect)
        {
            case SCRPT_SingleDialogue.E_Effects.ProgressiveReveal:
                UnfinishedEffectsCount++;
                revealCoroutine = StartCoroutine(Reveal(lineEffect.value == 0 ? 0.025f : lineEffect.value));
                break;

            case SCRPT_SingleDialogue.E_Effects.PauseOnIndex:
                pauseOnIndexQueue.Enqueue((int)lineEffect.value);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// <para> Progressive Reveal text effect : makes every dialogue characters appear progressivly, instead of immediatly. </para>
    /// <para> Can play Pause On Index effects, which pauses the text on the character's index for 1 second.</para>
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
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
                revealCoroutine = null;
                UnfinishedEffectsCount--;
            }

            counter++;

            yield return new WaitForSeconds(time);
        }
    }

    private void ForceStopEffects()
    {
        StopCoroutine(revealCoroutine);
        dialogueText.ForceMeshUpdate();

        int totalVisibleCharacters = dialogueText.textInfo.characterCount;
        dialogueText.maxVisibleCharacters = totalVisibleCharacters;
    }

    public void UpdateDialogueList()
    {
        DialogueNamesList = new List<string>();
        foreach (var item in Dialogues)
        {
            DialogueNamesList.Add(item.ID);
        }
    }

    public static void SearchAndUpdateDialogueList()
    {
        GameObject.FindObjectOfType<DialogueManager>().UpdateDialogueList();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        IsDialogueActive = false;
    }
}

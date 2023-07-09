using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class UIScreenBase : MonoBehaviourEventsHandler
{
    [Header("Base Screen")]
#if UNITY_EDITOR
    [InspectorButton(nameof(ED_CreateTween), ButtonWidth = 200)]
    [SerializeField] protected bool ED_createTween;
    protected void ED_CreateTween()
    {
        Undo.AddComponent<FadeScreenTween>(this.gameObject);
        EditorUtility.SetDirty(this);
    }
#endif

    [SerializeField] protected CanvasGroup group;
    public CanvasGroup Group { get => group; }
    [SerializeField] protected List<BaseScreenTween> screenTweens = new List<BaseScreenTween>();

    [field: SerializeField] public GameObject ObjectToSelectOnOpen { get; private set; }
    [field: SerializeField] public GameObject ObjectToSelectOnClose { get; private set; }

    public event Action OnTweensEnded;
    private int remainingTweens;

    protected virtual void Reset()
    {
        group = this.GetComponent<CanvasGroup>();
    }

    protected override void Awake()
    {
        base.Awake();
        foreach (var item in screenTweens)
        {
            item.OnTweenEnded += () =>
            {
                remainingTweens--;
                if (remainingTweens <= 0) OnTweensEnded?.Invoke();
            };
        }
    }

    public virtual void Open()
    {
        this.OpenScreen();
        OnScreenUp();
    }

    public virtual void Close()
    {
        this.CloseScreen();
        OnScreenDown();
    }
    protected virtual void Close(int playerIdx)
    {
        if (playerIdx == 0) Close();
    }

    public virtual void Show()
    {
        OnScreenUp();
        this.ShowScreen();
    }
    public virtual void Hide()
    {
        OnScreenDown();
        this.HideScreen();
    }

    public void ForceStartTweensIn()
    {
        Debug.Log("force");
        Debug.Log(screenTweens.Count);
        Debug.Log(screenTweens[0]);
        foreach (var item in screenTweens) item.StartTweenIn();
    }
    public void ForceStartTweensOut()
    { 
        foreach (var item in screenTweens) item.StartTweenOut();
    }

    protected virtual void OnScreenUp()
    {
        foreach (var item in screenTweens) item.StartTweenIn();
        PlayerInputsEvents.OnCancelButton += Close;
        this.group.interactable = this.group.blocksRaycasts = true;
        remainingTweens = screenTweens.Count;

        if (ObjectToSelectOnOpen != null)
            EventSystem.current.SetSelectedGameObject(ObjectToSelectOnOpen);

    }

    protected virtual void OnScreenDown()
    {
        foreach (var item in screenTweens) item.StartTweenOut();
        PlayerInputsEvents.OnCancelButton -= Close;
        this.group.interactable = this.group.blocksRaycasts = false;
        remainingTweens = screenTweens.Count;

        if (ObjectToSelectOnClose != null)
            EventSystem.current.SetSelectedGameObject(ObjectToSelectOnClose);
    }

    public void AddScreenTween(BaseScreenTween st)
    {
        if (screenTweens.Contains(st)) return;
        this.screenTweens.Add(st);
    }
    public void RemoveScreenTween(BaseScreenTween st) => this.screenTweens.Remove(st);

    protected virtual void ScreenTweenEnded()
        => this.ScreenTweenEnd();

    protected override void EventsSubscriber() { }
    protected override void EventsUnSubscriber() { }
}

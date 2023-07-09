using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public abstract class BaseScreenTween : MonoBehaviour
{
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected LeanTweenType tweenType = LeanTweenType.linear;

    public event Action OnTweenEnded;
    protected void TweenEnded() => OnTweenEnded?.Invoke();

    protected LTDescr currentTween;

    public virtual void Reset()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        UIScreenBase sb = this.GetComponent<UIScreenBase>();
        sb.AddScreenTween(this);
#if UNITY_EDITOR
        EditorUtility.SetDirty(sb);
#endif
    }

    public virtual void Awake() { }

    public virtual void Setup() { }

    public virtual void StartTweenIn(bool ignoreTween = false)
    {
        if (currentTween != null) LeanTween.cancel(currentTween.uniqueId);
    }

    public virtual void StartTweenOut(bool ignoreTween = false)
    {
        if (currentTween != null) LeanTween.cancel(currentTween.uniqueId);
    }
}

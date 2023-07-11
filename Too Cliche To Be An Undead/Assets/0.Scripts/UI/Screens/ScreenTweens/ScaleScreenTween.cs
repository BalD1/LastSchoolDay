using System.Collections;
using UnityEngine;

public class ScaleScreenTween : BaseScreenTween
{
    [SerializeField] private Vector3 maxScale = Vector3.one;
    [SerializeField] private Vector3 minScale = Vector3.zero;
    [SerializeField] private float scaleTime = .25f;
    [SerializeField] protected RectTransform rectTransform;

    public override void Reset()
    {
        base.Reset();
        rectTransform = this.GetComponent<RectTransform>();
    }

    public override void StartTweenIn(bool ignoreTween = false)
    {
        base.StartTweenIn();
        TweenTo(true, ignoreTween);
    }

    public override void StartTweenOut(bool ignoreTween = false)
    {
        base.StartTweenOut();
        TweenTo(false, ignoreTween);
    }

    private void TweenTo(bool targetIsMaxScale, bool ignoreTween)
    {
        Vector3 startScale = targetIsMaxScale ? minScale : maxScale;
        Vector3 targetScale = targetIsMaxScale ? maxScale : minScale;
        if (this.rectTransform.localScale != startScale)
            this.rectTransform.localScale = startScale;

        if (ignoreTween)
        {
            this.rectTransform.localScale = maxScale;
            this.TweenEnded();
            return;
        }
        currentTween = this.rectTransform.LeanScale(targetScale, scaleTime).setEase(tweenType).setOnComplete(() =>
        {
            currentTween = null;
            this.TweenEnded();
        }).setIgnoreTimeScale(true);
    }
}

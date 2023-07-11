using UnityEngine;

public class FadeScreenTween : BaseScreenTween
{
    [SerializeField] private bool useCustomAlphaLeanTime = false;
    [SerializeField] private float customAlphaLeanTime = .25f;
    private float currentAlphaLeanTime;

    public const float BASE_ALPHA_LEAN_TIME = .25f;

    public override void Awake()
    {
        base.Awake();
        if (UIManager.Instance == null)
        {
            currentAlphaLeanTime = customAlphaLeanTime;
            return;
        }
        currentAlphaLeanTime = useCustomAlphaLeanTime ?
                               customAlphaLeanTime :
                               BASE_ALPHA_LEAN_TIME;
    }

    public override void StartTweenIn(bool ignoreTween = false)
    {
        base.StartTweenIn();
        if (ignoreTween)
        {
            canvasGroup.alpha = 1;
            this.TweenEnded();
            return;
        }
        if (canvasGroup.alpha != 0) canvasGroup.alpha = 0;
        currentTween = canvasGroup.LeanAlpha(1, currentAlphaLeanTime).setEase(tweenType)
                            .setOnComplete(() =>
                            {
                                canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
                                currentTween = null;
                                this.TweenEnded();
                            }).setIgnoreTimeScale(true);
    }

    public override void StartTweenOut(bool ignoreTween = false)
    {
        base.StartTweenOut();
        if (ignoreTween)
        {
            canvasGroup.alpha = 0;
            this.TweenEnded();
            return;
        }
        if (canvasGroup.alpha != 1) canvasGroup.alpha = 1;
        currentTween = canvasGroup.LeanAlpha(0, currentAlphaLeanTime).setEase(tweenType)
             .setOnComplete(() =>
             {
                 canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
                 currentTween = null;
                 this.TweenEnded();
             }).setIgnoreTimeScale(true);
    }
}

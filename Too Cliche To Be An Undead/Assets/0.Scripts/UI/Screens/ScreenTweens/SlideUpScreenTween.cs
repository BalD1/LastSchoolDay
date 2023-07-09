using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideUpScreenTween : BaseScreenTween
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] protected RectTransform rectTransform;
    [SerializeField] private float slideTime = .5f;
    [SerializeField] private bool pingPong = true;
    [SerializeField] private E_Direction inDirection;
    [SerializeField] private E_Direction outDirection;
    protected float screenSize;

    private enum E_Direction
    {
        Down,
        Up
    }

    public override void Reset()
    {
        base.Reset();
        parentCanvas = this.GetComponentInParent<Canvas>();
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void Start()
    {
        screenSize = Screen.currentResolution.height;
    }

    public override void StartTweenIn(bool ignoreTween = false)
    {
        TweenTo(true, ignoreTween);
        base.StartTweenIn();
    }

    public override void StartTweenOut(bool ignoreTween = false)
    {
        TweenTo(false, ignoreTween);
        base.StartTweenOut();
    }

    private void TweenTo(bool targetIsBasePos, bool ignoreTween)
    {
        if (screenSize == 0) screenSize = Screen.currentResolution.height;
        float startPos = -1;
        float targetPos = -1;
        switch (inDirection)
        {
            case E_Direction.Down:
                startPos = targetIsBasePos ? -screenSize : 0;
                break;
            case E_Direction.Up:
                startPos = targetIsBasePos ? screenSize : 0;
                break;
        }
        switch (outDirection)
        {
            case E_Direction.Down:
                    targetPos = targetIsBasePos ? 0 : -screenSize;
                break;
            case E_Direction.Up:
                    targetPos = targetIsBasePos ? 0 : screenSize;
                break;
        }

        if (ignoreTween)
        {
            this.rectTransform.SetBottom(targetPos);
            this.rectTransform.SetTop(targetPos * -1);
            return;
        }
        if (this.rectTransform.localPosition.y != startPos)
        {
            this.rectTransform.SetBottom(startPos);
            this.rectTransform.SetTop(startPos * -1);
        }
        currentTween = this.rectTransform.LeanMoveY(targetPos, slideTime).setEase(tweenType).setOnComplete(() =>
        {
            currentTween = null;
            this.TweenEnded();
        });
    }
}

using UnityEngine;

public class SlideScreenTween : BaseScreenTween
{
    [SerializeField] private float slideTime = .5f;
    [SerializeField] protected RectTransform rectTransform;
    protected Vector2 screenSize;

    [SerializeField] private Vector2 screenSizeMultiplier = Vector2.one;
    private Vector2 basePos = Vector2.zero;

    [SerializeField] private E_HDirection inHorizontalDir;
    [SerializeField] private E_VDirection inVerticalDir;

    [SerializeField] private E_HDirection outHorizontalDir;
    [SerializeField] private E_VDirection outVerticalDir;

    private enum E_HDirection
    {
        None,
        Left,
        Right,
    }

    private enum E_VDirection
    {
        None,
        Down,
        Up
    }

    public override void Awake()
    {
        base.Awake();
        Init();
    }

    public override void Setup()
    {
        base.Setup();
        rectTransform = this.GetComponent<RectTransform>();
    }

    private void Init()
    {
        basePos = Vector2.zero;
        screenSize.x = UIManager.CanvasSize.x * screenSizeMultiplier.x;
        screenSize.y = UIManager.CanvasSize.y * screenSizeMultiplier.y;
        Vector2 startPos = basePos;
        switch (inHorizontalDir)
        {
            case E_HDirection.Left:
                startPos.x = -screenSize.x;
                break;
            case E_HDirection.Right:
                startPos.x = screenSize.x;
                break;
        }
        switch (inVerticalDir)
        {
            case E_VDirection.Down:
                startPos.y = -screenSize.y;
                break;
            case E_VDirection.Up:
                startPos.y = screenSize.y;
                break;
        }
        ForceSetPos(startPos);
    }

    public override void StartTweenIn(bool ignoreTween = false)
    {
        base.StartTweenIn(ignoreTween);
        TweenTo(true, ignoreTween);
    }

    public override void StartTweenOut(bool ignoreTween = false)
    {
        base.StartTweenOut(ignoreTween);
        TweenTo(false, ignoreTween);
    }

    private void TweenTo(bool targetIsBasePos, bool ignoreTween)
    {
        Vector2 startPos = BuildStartPos(targetIsBasePos);
        Vector2 targetPos = BuildTargetPos(targetIsBasePos);

        if (ignoreTween)
        {
            ForceSetPos(targetPos);
            this.TweenEnded();
            return;
        }
        else if (this.rectTransform.position.x != startPos.x ||
                 this.rectTransform.position.y != startPos.y)
        {
            ForceSetPos(startPos);
        }

        this.rectTransform.LeanMove((Vector3)targetPos, slideTime).setEase(tweenType).setOnComplete(() =>
        {
            currentTween = null;
            this.TweenEnded();
        }).setIgnoreTimeScale(true);
    }

    private Vector2 BuildStartPos(bool targetIsBasePos)
    {
        Vector2 res = Vector2.zero;
        switch (inHorizontalDir)
        {
            case E_HDirection.Left:
                res.x = targetIsBasePos ? -screenSize.x : basePos.x;
                break;
            case E_HDirection.Right:
                res.x = targetIsBasePos ? screenSize.x : basePos.x;
                break;
        }
        switch (inVerticalDir)
        {
            case E_VDirection.Down:
                res.y = targetIsBasePos ? -screenSize.y : basePos.y;
                break;
            case E_VDirection.Up:
                res.y = targetIsBasePos ? screenSize.y : basePos.y;
                break;
        }

        return res;
    }

    private Vector2 BuildTargetPos(bool targetIsBasePos)
    {
        Vector2 res = Vector2.zero;

        switch (outHorizontalDir)
        {
            case E_HDirection.Left:
                res.x = targetIsBasePos ? basePos.x : -screenSize.x;
                break;
            case E_HDirection.Right:
                res.x = targetIsBasePos ? basePos.x : screenSize.x;
                break;
        }
        switch (outVerticalDir)
        {
            case E_VDirection.Down:
                res.y = targetIsBasePos ? basePos.y : -screenSize.y;
                break;
            case E_VDirection.Up:
                res.y = targetIsBasePos ? basePos.y : screenSize.y;
                break;
        }



        return res;
    }

    private void ForceSetPos(Vector2 pos)
    {
        this.rectTransform.SetLeft(pos.x);
        this.rectTransform.SetRight(pos.x * -1);
        this.rectTransform.SetBottom(pos.y);
        this.rectTransform.SetTop(pos.y * -1);
    }
}

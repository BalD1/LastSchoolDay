using UnityEngine;

public class Minimap : MonoBehaviourEventsHandler
{
    [SerializeField] private RectTransform selfRectTransform;

    [SerializeField] private RectTransform coinsRectTransform;
    [SerializeField] private RectTransform buttonTutoRectTransform;

    private S_TargetRect selfRectScaleBase;
    private S_TargetRect coinsRectScaleBase;
    private S_TargetRect buttonTutoRectScaleBase;

    [SerializeField] private S_TargetRect selfRectScaleTarget;
    [SerializeField] private S_TargetRect coinsRectScaleTarget;
    [SerializeField] private S_TargetRect buttonTutoRectScaleTarget;

    [SerializeField] private float scaleTime = .5f;

    [System.Serializable]
    private struct S_TargetRect
    {
        public Vector2 min;
        public Vector2 max;

        public S_TargetRect(Vector2 _min, Vector2 _max)
        {
            min = _min;
            max = _max;
        }
        public S_TargetRect(RectTransform rt)
        {
            min = rt.anchorMin;
            max = rt.anchorMax;
        }

        public static S_TargetRect operator -(S_TargetRect a, S_TargetRect b)
        {
            a.min -= b.min;
            a.max -= b.max;

            return a;
        }
        public static S_TargetRect operator +(S_TargetRect a, S_TargetRect b)
        {
            a.min += b.min;
            a.max += b.max;

            return a;
        }
        public static S_TargetRect operator -(S_TargetRect a, RectTransform b)
        {
            a.min -= b.anchorMin;
            a.max -= b.anchorMax;

            return a;
        }
        public static S_TargetRect operator +(S_TargetRect a, RectTransform b)
        {
            a.min += b.anchorMin;
            a.max += b.anchorMax;

            return a;
        }
    }

    [SerializeField] [ReadOnly]private bool isTweening = false;

    [SerializeField] [ReadOnly]private bool isAtBaseScale = true;

    protected override void EventsSubscriber()
    {
        PlayerInputsEvents.OnSecondContext += OnAskForScale;
    }

    protected override void EventsUnSubscriber()
    {
        PlayerInputsEvents.OnSecondContext -= OnAskForScale;
    }

    private void Start()
    {
        // Get the base Rect of the elements;
        selfRectScaleBase = new S_TargetRect(selfRectTransform.anchorMin, selfRectTransform.anchorMax);
        coinsRectScaleBase = new S_TargetRect(coinsRectTransform.anchorMin, coinsRectTransform.anchorMax);
        buttonTutoRectScaleBase = new S_TargetRect(buttonTutoRectTransform.anchorMin, buttonTutoRectTransform.anchorMax);
    }

    private void Reset()
    {
        selfRectTransform = this.GetComponent<RectTransform>();
    }

    private void OnAskForScale(int idx)
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        if (isTweening) return;
        isTweening = true;

        if (isAtBaseScale) CameraManager.Instance.SetMinimapToOverview();
        else CameraManager.Instance.AttachMinimapCamera();

        PerformScale();
    }

    private void PerformScale()
    {
        S_TargetRect selfAlphaTarget = new S_TargetRect();
        S_TargetRect coinsAlphaTarget = new S_TargetRect();
        S_TargetRect buttonTutoAlphaTarget = new S_TargetRect();

        if (isAtBaseScale)
        {
            selfAlphaTarget = selfRectScaleTarget - selfRectScaleBase;
            coinsAlphaTarget = coinsRectScaleTarget - coinsRectScaleBase;
            buttonTutoAlphaTarget = buttonTutoRectScaleTarget - buttonTutoRectScaleBase;
        }
        else
        {
            selfAlphaTarget = selfRectScaleBase - selfRectScaleTarget;
            coinsAlphaTarget = coinsRectScaleBase - coinsRectScaleTarget;
            buttonTutoAlphaTarget = buttonTutoRectScaleBase - buttonTutoRectScaleTarget;
        }

        LeanTween.value(0.1f, 1, scaleTime).setOnUpdate((float alpha) =>
        {
            Vector2 selfScaleTo = isAtBaseScale ? selfRectScaleBase.min : selfRectScaleTarget.min;
            CalculateMinRectDisplacement(ref selfRectTransform, selfScaleTo, selfAlphaTarget, alpha);

            Vector2 coinsMinScaleTo = isAtBaseScale ? coinsRectScaleBase.min : coinsRectScaleTarget.min;
            Vector2 coinsMaxScaleTo = isAtBaseScale ? coinsRectScaleBase.max : coinsRectScaleTarget.max;
            CalculateRectDisplacement(ref coinsRectTransform, coinsMinScaleTo, coinsMaxScaleTo, coinsAlphaTarget, alpha);

            Vector2 buttonMinScaleTo = isAtBaseScale ? buttonTutoRectScaleBase.min : buttonTutoRectScaleTarget.min;
            Vector2 buttonMaxScaleTo = isAtBaseScale ? buttonTutoRectScaleBase.max : buttonTutoRectScaleTarget.max;
            CalculateRectDisplacement(ref buttonTutoRectTransform, buttonMinScaleTo, buttonMaxScaleTo, buttonTutoAlphaTarget, alpha);

        }).setEaseSpring().setOnComplete(() =>
        {
            isTweening = false;
            isAtBaseScale = !isAtBaseScale;
        });
    }

    private void CalculateMinRectDisplacement(ref RectTransform rt, Vector2 scaleTo, S_TargetRect alphaTarget, float alpha)
    {
        Vector2 anchMin = DisplaceAnchors(alphaTarget.min,
                                          alpha,
                                          scaleTo);

        rt.anchorMin = anchMin;
    }
    private void CalculateMaxRectDisplacement(ref RectTransform rt, Vector2 scaleTo, S_TargetRect alphaTarget, float alpha)
    {
        Vector2 anchMax = DisplaceAnchors(alphaTarget.min,
                                          alpha,
                                          scaleTo);

        rt.anchorMax = anchMax;
    }
    private void CalculateRectDisplacement(ref RectTransform rt, Vector2 minScaleTo, Vector2 maxScaleTo, S_TargetRect alphaTarget, float alpha)
    {
        CalculateMinRectDisplacement(ref rt, minScaleTo, alphaTarget, alpha);
        CalculateMaxRectDisplacement(ref rt, maxScaleTo, alphaTarget, alpha);
    }

    private Vector2 DisplaceAnchors(Vector2 alphaTarget, float alpha, Vector2 target)
    {
        return (alphaTarget * alpha) + target;
    }
}

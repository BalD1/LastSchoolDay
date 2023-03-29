using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private RectTransform selfRect;

    [SerializeField] private RectTransform coinsRect;
    [SerializeField] private RectTransform buttonTutoRect;

    private S_TargetRect baseSelfRect;
    [SerializeField] private S_TargetRect selfRectScaledTarget;

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

    [SerializeField] [ReadOnly]private bool baseScale = true;

    private void Start()
    {
        baseSelfRect = new S_TargetRect(selfRect.anchorMin, selfRect.anchorMax);

        GameManager.Player1Ref.D_secondContextAction += OnAskForScale;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            selfRect.anchorMin = baseSelfRect.min;
            selfRect.anchorMax = baseSelfRect.max;
            baseScale = true;
            return;

        }
    }

    private void Reset()
    {
        selfRect = this.GetComponent<RectTransform>();
    }

    private void OnAskForScale()
    {
        if (isTweening) return;

        isTweening = true;

        PerformScale();
    }

    private void PerformScale()
    {
        S_TargetRect targetRect = new S_TargetRect();

        if (baseScale)
        {
            targetRect = selfRectScaledTarget;
            targetRect -= baseSelfRect;
        }
        else
        {
            targetRect = baseSelfRect;
            targetRect -= selfRectScaledTarget;
        }

        LeanTween.value(0.1f, 1, scaleTime).setOnUpdate((float alpha) =>
        {
            selfRect.anchorMin = (targetRect.min * alpha) + (baseScale ? baseSelfRect.min : selfRectScaledTarget.min);
        }).setOnComplete(() =>
        {
            isTweening = false;
            baseScale = !baseScale;
        });
    }
}

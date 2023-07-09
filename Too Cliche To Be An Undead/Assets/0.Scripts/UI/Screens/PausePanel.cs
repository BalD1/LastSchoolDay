using Spine.Unity;
using UnityEngine;

public class PausePanel : UIScreenBase
{
    [SerializeField] private SkeletonGraphic skeletonPanel;
    [SerializeField] private float tweenTime = .25f;
    [SerializeField] private LeanTweenType tweenType = LeanTweenType.easeInOutQuart;

    private LTDescr externalPanelTween = null;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Open()
    {
        base.Open();
        LeanSkeletonPanel(0, 1);
    }

    public override void Close()
    {
        base.Close();
        LeanSkeletonPanel(1, 0);
    }

    private void LeanSkeletonPanel(float from, float goal)
    {
        if (skeletonPanel == null) return;

        if (externalPanelTween != null) LeanTween.cancel(externalPanelTween.uniqueId);

        externalPanelTween = LeanTween.value(from, goal, tweenTime).setOnUpdate((float val) =>
        {
            Color c = skeletonPanel.color;
            c.a = val;
            skeletonPanel.color = c;
        }).setEase(tweenType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            externalPanelTween = null;
        });
    }
}

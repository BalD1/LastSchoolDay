using System.Collections;
using UnityEngine;

public class CA_CinematicFadeCanvasGroup : CA_CinematicAction
{
    [SerializeField] private CanvasGroup targetCG;
    [SerializeField] private float alphaGoal = 1;
    [SerializeField] private float fadeTime = 1;

    public CA_CinematicFadeCanvasGroup(CanvasGroup targetCG)
        => Setup(targetCG);
    public CA_CinematicFadeCanvasGroup(CanvasGroup targetCG, float alphaGoal)
        => Setup(targetCG, alphaGoal);
    public CA_CinematicFadeCanvasGroup(CanvasGroup targetCG, float alphaGoal, float fadeTime)
        => Setup(targetCG, alphaGoal, fadeTime);

    public void Setup(CanvasGroup _targetCG)
       => _targetCG = targetCG;
    public void Setup(CanvasGroup _targetCG, float _alphaGoal)
    {
        targetCG = _targetCG;
        alphaGoal = _alphaGoal;
    }
    public void Setup(CanvasGroup _targetCG, float _alphaGoal, float _fadeTime)
    {
        targetCG = _targetCG;
        alphaGoal = _alphaGoal;
        fadeTime = _fadeTime;
    }

    public override void Execute()
    {
        if (targetCG == null)
        {
            this.ActionEnded(this);
            return;
        }

        targetCG.LeanAlpha(alphaGoal, fadeTime).setOnComplete(() => this.ActionEnded(this));
    }
}

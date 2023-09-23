using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CA_CinematicFadeImage : CA_CinematicAction
{
    [SerializeField] private Image targetImage;
    [SerializeField] private float alphaGoal = 1;
    [SerializeField] private float fadeTime = 1;

    public CA_CinematicFadeImage(Image targetImage)
        => Setup(targetImage);
    public CA_CinematicFadeImage(Image targetImage, float alphaGoal)
        => Setup(targetImage, alphaGoal);
    public CA_CinematicFadeImage(Image targetImage, float alphaGoal, float fadeTime)
        => Setup(targetImage, alphaGoal, fadeTime);

    public void Setup(Image _targetImage)
       => _targetImage = targetImage; 
    public void Setup(Image _targetImage, float _alphaGoal)
    {
        targetImage = _targetImage;
        alphaGoal = _alphaGoal;
    }
    public void Setup(Image _targetImage, float _alphaGoal, float _fadeTime)
    {
        targetImage = _targetImage;
        alphaGoal = _alphaGoal;
        fadeTime = _fadeTime;
    }

    public override void Execute()
    {
        if (targetImage == null)
        {
            this.ActionEnded(this);
            return;
        }

        targetImage.LeanAlpha(alphaGoal, fadeTime).setOnComplete(() => this.ActionEnded(this));
    }
}

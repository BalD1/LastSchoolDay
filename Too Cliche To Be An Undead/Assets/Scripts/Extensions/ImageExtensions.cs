using UnityEngine;
using UnityEngine.UI;

public static class ImageExtensions
{
    public static void SetAlpha(this Image image, float alpha)
    {
        if (image == null) return;

        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }

    public static LTDescr LeanAlpha(this Image image, float alphaGoal, float time)
    {
        return LeanTween.alpha(image.rectTransform, alphaGoal, time);
    }
}

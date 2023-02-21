using UnityEngine;

public static class SpriteRendererExtensions
{
    public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
    {
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;
    }

    public static LTDescr LeanAlpha(this SpriteRenderer spriteRenderer, float alphaGoal, float time)
    {
        return LeanTween.alpha(spriteRenderer.gameObject, alphaGoal, time);
    }
}

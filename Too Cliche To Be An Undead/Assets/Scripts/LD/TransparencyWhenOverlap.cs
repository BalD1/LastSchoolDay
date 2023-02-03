using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyWhenOverlap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Color transparentColor;

    private int playersCountInTrigger;

    private bool isTweening;

    private void Awake()
    {
        transparentColor = Color.white;
        transparentColor.a /= 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<PlayerCharacter>() == null) return;

        playersCountInTrigger++;

        if (playersCountInTrigger > 1) return;
        if (isTweening) LeanTween.cancel(this.gameObject);

        isTweening = true;

        LeanTween.value(this.gameObject, spriteRenderer.color, transparentColor, .3f).setOnUpdate(
        (Color val) =>
        {
            spriteRenderer.color = val;
        }).setOnComplete(() => isTweening = false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.parent.GetComponent<PlayerCharacter>() == null) return;

        playersCountInTrigger--;

        if (playersCountInTrigger > 0) return;
        if (isTweening) LeanTween.cancel(this.gameObject);

        isTweening = true;

        LeanTween.value(this.gameObject, spriteRenderer.color, Color.white, .3f).setOnUpdate(
        (Color val) =>
        {
            spriteRenderer.color = val;
        }).setOnComplete(() => isTweening = false);
    }
}

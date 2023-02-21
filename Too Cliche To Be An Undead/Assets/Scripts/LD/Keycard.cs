using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    private bool isPicked = false;

    [SerializeField] private SCRPT_KeycardAssets assets;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer minimapRenderer;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        Sprite randSprite = assets.KeycardSprites.RandomElement();

        this.spriteRenderer.sprite = randSprite;
        this.minimapRenderer.sprite = randSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("pickup");
            isPicked = true;
            GameManager.AcquiredCards += 1;
            UIManager.Instance.UpdateKeycardsCounter();
        }
    }

    public void OnPickAnimationEnd()
    {
        Destroy(this.gameObject);
    }
}

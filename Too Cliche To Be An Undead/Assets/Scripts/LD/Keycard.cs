using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keycard : MonoBehaviour
{
    private bool isPicked = false;

    [SerializeField] private SCRPT_KeycardAssets assets;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject minimapSprites;

    public delegate void D_OnPickup();
    public D_OnPickup D_onPickup;

    private int idx;

    private void Awake()
    {
        Sprite randSprite = assets.KeycardSprites.RandomElement();

        this.spriteRenderer.sprite = randSprite;
    }

    private void Start()
    {
        for (int i = 0; i < UIManager.Instance.HudKeycards.Length; i++)
        {
            if (UIManager.Instance.HudKeycards[i].color == Color.white)
            {
                this.idx = i;
                UIManager.Instance.HudKeycards[i].sprite = this.spriteRenderer.sprite;
                UIManager.Instance.HudKeycards[i].color = Color.gray;
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag("Player"))
        {
            minimapSprites.SetActive(false);
            animator.SetTrigger("pickup");
            isPicked = true;
            GameManager.Instance.AcquiredCards += 1;
            UIManager.Instance.UpdateKeycardsCounter(idx);
            D_onPickup?.Invoke();
        }
    }

    public void SetMinimapState(bool active)
    {
        minimapSprites.SetActive(active);
        animator.Play("ANIM_Keycard_Idle");
    }

    public void OnPickAnimationEnd()
    {
        Destroy(this.gameObject);
    }
}

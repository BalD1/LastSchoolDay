using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Keycard : MonoBehaviour
{
    private bool isPicked = false;

    [SerializeField] private AnimationCurve animationCurve;

    [SerializeField] private SCRPT_KeycardAssets assets;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private Animator animator;

    [SerializeField] private GameObject minimapSprites;

    private RectTransform uiGoal;
    private Image uiImage;

    private bool travel;

    private float travelStep;

    public event Action onPickup;
    public event Action onAnimationEnded;

    private void Awake()
    {
        Sprite randSprite = assets.KeycardSprites.RandomElement();
        this.spriteRenderer.sprite = randSprite;
    }
    private void Update()
    {
        TravelToUI();
    }

    public void Setup(Image uiImage, RectTransform rectTransform)
    {
        this.uiImage = uiImage;
        uiImage.sprite = this.spriteRenderer.sprite;
        uiImage.color = Color.gray;
        uiGoal = uiImage.rectTransform;
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPicked) return;

        if (collision.CompareTag("Player"))
        {
            minimapSprites.SetActive(false);
            animator.SetTrigger("pickup");
            travel = true;
            isPicked = true;
            onPickup?.Invoke();
        }
    }

    private void TravelToUI()
    {
        if (!travel) return;

        Vector2 goal = Camera.main.ScreenToWorldPoint(uiGoal.position);

        travelStep += Time.deltaTime;
        float delta = animationCurve.Evaluate(travelStep);

        this.transform.position = Vector2.MoveTowards(transform.position, goal, delta);

        if (Vector2.Distance(this.transform.position, goal) > .1f) return;

        OnPickAnimationEnd();
    }

    public void SetMinimapState(bool active)
    {
        minimapSprites.SetActive(active);
        animator.Play("ANIM_Keycard_Idle");
    }

    public void OnPickAnimationEnd()
    {
        uiImage.color = Color.white;
        LeanTween.scale(uiGoal, new Vector2(1.6f, 1.6f), .5f).setEase(LeanTweenType.easeInSine);
        LeanTween.rotate(uiGoal, new Vector3(0, 0, 3f), .5f).setEase(LeanTweenType.easeInSine).setOnComplete(() =>
        {
            LeanTween.scale(uiGoal, Vector2.one, .5f).setEase(LeanTweenType.easeOutSine);
            LeanTween.rotate(uiGoal, new Vector3(0, 0, -3f), .5f).setEase(LeanTweenType.easeOutSine);
        });
        onAnimationEnded?.Invoke();
        Destroy(this.gameObject);
    }
}

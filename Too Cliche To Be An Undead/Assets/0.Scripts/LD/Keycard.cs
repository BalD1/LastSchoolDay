using UnityEngine;
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

    private bool travel;

    private float travelStep;

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
            Image uiImage = UIManager.Instance.HudKeycards[i];
            if (uiImage.color == Color.white)
            {
                this.idx = i;
                uiImage.sprite = this.spriteRenderer.sprite;
                uiImage.color = Color.gray;

                uiGoal = uiImage.rectTransform;
                return;
            }
        }
    }

    private void Update()
    {
        TravelToUI();
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
            GameManager.Instance.AcquiredCards += 1;
            D_onPickup?.Invoke();
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
        UIManager.Instance.UpdateKeycardsCounter(idx);
        Destroy(this.gameObject);
    }
}

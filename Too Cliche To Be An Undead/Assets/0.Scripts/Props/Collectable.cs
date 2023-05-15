using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    private float dist;
    private float stepp;
    public float drawdistance = 3;
    public float baseDrawLerpAlpha = 2;
    public AnimationCurve animationCurve;
    public GameObject particle;
    public int coinValue = 1;

    [SerializeField] private Collider2D selfTrigger;

    [SerializeField] protected AudioClip pickupSound;
    [SerializeField] private AudioSource source;

    [SerializeField] private float pickupDistance = 1f;

    [SerializeField] protected bool pickupOnCollision = true;

    [SerializeField] protected Rigidbody2D rb;

    public bool canBePickedUp = true;

    private bool isPicked = false;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] protected PlayerCharacter detectedPlayer;
    private Transform goalTarget;
    private bool allowTravel;

    public delegate void D_OnPickUp();
    public D_OnPickUp D_onPickUp;

    private void Awake()
    {
        if (!pickupOnCollision) Destroy(selfTrigger);
    }

    protected virtual void Update()
    {
        if (allowTravel) TravelToGoal();
    }

    private void TravelToGoal()
    {
        dist = Vector2.Distance(this.transform.position, detectedPlayer.transform.position);

        bool allowDraw = drawdistance < 0 || dist <= drawdistance;

        //Draw Coin toward Player
        if (allowDraw)
        {
            //l'attirance est définie par une courbe

            float drawLerp = drawdistance > 0 ? drawdistance : baseDrawLerpAlpha;
            stepp = (animationCurve.Evaluate(1 - dist / drawLerp) / 90);

            float playerAddedSpeed = detectedPlayer.MaxSpeed_M - detectedPlayer.GetStats.Speed;
            if (playerAddedSpeed != 0) stepp += (playerAddedSpeed / 90);

            this.transform.position = Vector2.MoveTowards(transform.position, goalTarget.position, stepp);
        }

        dist = Vector2.Distance(this.transform.position, goalTarget.position);

        if (dist <= pickupDistance)
        {
            CreateParticles();

            TouchedPlayer(detectedPlayer);

            D_onPickUp?.Invoke();

            Destroy(this.gameObject);
            return;
        }
    }

    private void StartTravelToPlayer(PlayerCharacter goal)
    {
        detectedPlayer = goal;
        goalTarget = goal.transform;
        allowTravel = true;

        Destroy(selfTrigger);
    }

    protected virtual void PlayPickupSound()
    {
        source?.PlayOneShot(pickupSound);
    }

    protected virtual GameObject CreateParticles()
    {
        return particle?.Create(this.transform.position, Quaternion.identity);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player") == false) return;
        if (detectedPlayer != null) return;

        detectedPlayer = collision.GetComponentInParent<PlayerCharacter>();
        StartTravelToPlayer(detectedPlayer);
    }

    protected virtual void TouchedPlayer(PlayerCharacter player)
    {
        PlayPickupSound();
    }


    public void EnteredInRange(GameObject interactor)
    {
        if (pickupOnCollision) return;
        spriteRenderer.material = GameAssets.Instance.OutlineMaterial;
    }

    public void ExitedRange(GameObject interactor)
    {
        if (pickupOnCollision) return;
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;

    }

    public virtual void Interact(GameObject interactor)
    {
        if (pickupOnCollision || !canBePickedUp) return;
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;

        isPicked = true;

        PlayPickupSound();

        StartTravelToPlayer(interactor.GetComponentInParent<PlayerCharacter>());
    }

    public bool CanBeInteractedWith()
    {
        if (pickupOnCollision) return false;
        return !isPicked;
    }

    public float GetDistanceFrom(Transform target)
    {
        if (pickupOnCollision) return float.MaxValue;

        return Vector2.Distance(this.transform.position, target.position);
    }

}

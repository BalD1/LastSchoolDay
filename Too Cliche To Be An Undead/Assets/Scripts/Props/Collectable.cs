using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour, IInteractable
{
    private float dist;
    private float stepp;
    public float drawdistance = 3;
    public AnimationCurve animationCurve;
    public ParticleSystem particle;
    public int coinValue = 1;

    [SerializeField] private float pickupDistance = 1f;

    [SerializeField] protected bool pickupOnCollision = true;

    private bool isPicked = false;

    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] private List<PlayerCharacter> detectedPlayers = new List<PlayerCharacter>();

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //avant d'attirer la pièce, faire qu'elle soit "expulsée" d'un ennemi, et après activer l'attirance
        //actuellement la pièce ne se dirige que vers un seul joueur
        //potentiellement faire une draw distance sur le joueur et que ce soit lui qui attire les pièces

        if (!pickupOnCollision) return;

        if (detectedPlayers.Count == 0) return;

        dist = Vector2.Distance(this.transform.position, detectedPlayers[0].transform.position);

        //Draw Coin toward Player
        if (dist <= drawdistance)
        {
            //l'attirance est définie par une courbe

            stepp = animationCurve.Evaluate(1 - dist / drawdistance) / 90;

            this.transform.position = Vector2.MoveTowards(transform.position, detectedPlayers[0].transform.position, stepp);

        }

        foreach (var item in detectedPlayers)
        {
            dist = Vector2.Distance(this.transform.position, item.transform.position);

            if (dist <= pickupDistance)
            {
                //spawn particle
                if (particle != null)
                    Instantiate(particle, this.transform.position, Quaternion.identity);

                //play sound

                //add coin
                TouchedPlayer(item);

                Destroy(this.gameObject);
                return;
            }
        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            detectedPlayers.Add(collision.GetComponentInParent<PlayerCharacter>());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            detectedPlayers.Remove(collision.GetComponentInParent<PlayerCharacter>());
        }
    }

    protected virtual void TouchedPlayer(PlayerCharacter player)
    {

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
        if (pickupOnCollision) return;

        isPicked = true;

        Destroy(this.gameObject, 1f);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public GameObject player;
    private float dist;
    private float stepp;
    public float drawdistance = 3;
    private Rigidbody2D rigidBody2D;
    private BoxCollider2D boxCollider;
    public AnimationCurve animationCurve;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //avant d'attirer la pièce, faire qu'elle soit "expulsée" d'un ennemi, et après activer l'attirance
        //actuellement la pièce ne se dirige que vers un seul joueur
        //potentiellement faire une draw distance sur le joueur et que ce soit lui qui attire les pièces

        dist = Vector2.Distance(this.transform.position, player.transform.position);

        //Draw Coin toward Player
        if (dist <= drawdistance)
        {
            //l'attirance est définie par une courbe

            stepp = animationCurve.Evaluate(1 - dist/drawdistance)/90;

            this.transform.position = Vector2.MoveTowards(transform.position, player.transform.position, stepp);

        }
        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //spawn particle
        //play sound
        //add coin
        Destroy(this.gameObject);
    }

}


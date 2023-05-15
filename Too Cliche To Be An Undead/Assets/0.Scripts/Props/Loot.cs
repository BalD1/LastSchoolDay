using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private Collectable collectable;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float minRandomOffset = .80f;
    [SerializeField] private float maxRandomOffset = 1.20f;

    [SerializeField] private float velocityMultiplier = .5f;

    [System.Serializable]
    private struct VelocityCurves
    {
        public ParticleSystem.MinMaxCurve xVelocity;
        public ParticleSystem.MinMaxCurve yVelocity;

        public float GetVelocity_X(float time) => xVelocity.Evaluate(time);
        public float GetVelocity_Y(float time) => yVelocity.Evaluate(time);

        public Vector2 GetVelocity(float time) => new Vector2(GetVelocity_X(time), GetVelocity_Y(time));

        public float MaxTime()
        {
            float resX = xVelocity.curve[xVelocity.curve.length - 1].time;
            float resY = yVelocity.curve[yVelocity.curve.length - 1].time;

            return resX > resY ? resX : resY;
        }
    }
    [SerializeField] private VelocityCurves[] velocityCurves;
    private VelocityCurves curveToFollow;

    private float offset;
    private float elapsedTime;

    private bool inverseX;
    private bool inverseY;

    private Vector2 vel;

    private Rigidbody2D childBody;

    private void Awake()
    {
        if (velocityCurves == null || velocityCurves.Length <= 0)
        {
            Stop();
            return;
        }

        collectable.enabled = false;

        childBody = collectable.GetComponent<Rigidbody2D>();
        if (childBody != null)
            childBody.simulated = false;

        offset = Random.Range(minRandomOffset, maxRandomOffset);
        inverseX = Random.Range(0, 2) == 0;
        inverseY = Random.Range(0, 2) == 0;

        curveToFollow = velocityCurves[Random.Range(0, velocityCurves.Length)];
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= curveToFollow.MaxTime())
        {
            Stop();
            return;
        }

        vel.x = curveToFollow.GetVelocity_X(elapsedTime) * offset * velocityMultiplier;
        if (inverseX) vel.x = -vel.x;

        vel.y = curveToFollow.GetVelocity_Y(elapsedTime) * offset * velocityMultiplier;
        if (inverseY) vel.y = -vel.y;

        this.rb.velocity = vel;
    }

    private void Stop()
    {
        if (childBody != null)
            childBody.simulated = true;
        collectable.transform.parent = null;
        collectable.enabled = true;
        Destroy(this.gameObject);
    }
}

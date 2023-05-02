using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleSystemHandler : MonoBehaviour
{
    public static BloodParticleSystemHandler Instance { get; private set; }

    [SerializeField] private Vector2 quadSize;

    [SerializeField] private float minSpeedRange;
    [SerializeField] private float maxSpeedRange;

    private MeshParticleSystem meshParticleSystem;
    private List<Single> singleList;

    [SerializeField] private int minParticlesToSpawnAtOnce = 1;
    [SerializeField] private int maxParticlesToSpawnAtOnce = 4;

    [SerializeField] private int maxBloodParticles = 4000;

    public int c;

    private void Awake()
    {
        Instance = this;
        meshParticleSystem = GetComponent<MeshParticleSystem>();
        singleList = new List<Single>();
    }

    private void Update()
    {
        for (int i = 0; i < singleList.Count; i++)
        {
            Single single = singleList[i];
            single.Update();
            if (single.IsParticleComplete())
            {
                single.DestroySelf();
                singleList.RemoveAt(i);
                i--;
            }
        }

        c = singleList.Count;
    }

    public void SpawnBlood(Vector3 position, Vector3 direction, float lifetime)
    {
        int particlesToSpawn = Random.Range(minParticlesToSpawnAtOnce, maxParticlesToSpawnAtOnce);

        while (singleList.Count + particlesToSpawn > maxBloodParticles)
        {
            singleList[0].DestroySelf();
            singleList.RemoveAt(0);
        }

        for (int i = 0; i < particlesToSpawn; i++)
        {
            Vector3 finalDirection = ApplyRotationToVector(direction, Random.Range(-15f, 15f));
            float speed = Random.Range(minSpeedRange, maxSpeedRange);

            singleList.Add(new Single(position, finalDirection, meshParticleSystem, lifetime, speed));
        }
    }

    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }

    private class Single
    {
        private MeshParticleSystem meshParticleSystem;
        private Vector3 position;
        private Vector3 direction;
        private int quadIndex;
        private float moveSpeed;
        private float rotation;
        private int uvIndex;

        private float lifetime;

        public Single(Vector3 position, Vector3 direction, MeshParticleSystem meshParticleSystem, float lifetime, float speed)
        {
            this.position = position;
            this.direction = direction;
            this.meshParticleSystem = meshParticleSystem;

            rotation = Random.Range(0, 360f);
            moveSpeed = speed;
            uvIndex = Random.Range(0, 8);

            quadIndex = meshParticleSystem.AddQuad(position, rotation, Instance.quadSize, false, uvIndex);

            this.lifetime = lifetime;
        }

        public void Update()
        {
            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticleSystem.UpdateQuad(quadIndex, position, rotation, Instance.quadSize, false, uvIndex);

            float slowDownFactor = 7f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;

            lifetime -= Time.deltaTime;
        }

        public bool IsParticleComplete()
        {
            return moveSpeed < .1f && lifetime <= 0;
        }

        public void DestroySelf()
        {
            meshParticleSystem.DestroyQuad(quadIndex);
        }

    }

}

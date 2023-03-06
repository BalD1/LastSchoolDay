using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootprintParticleSystemHandler : MonoBehaviour
{
    public static FootprintParticleSystemHandler Instance { get; private set; }

    [SerializeField] private Vector2 quadSize;

    private List<Single> singleList;

    private MeshParticleSystem meshParticleSystem;

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
            if (single.IsComplete())
            {
                single.DestroySelf();
                singleList.RemoveAt(i);
                i--;
            }
        }
    }

    public void SpawnFootprint(Vector3 position, Vector3 direction, float lifetime)
    {
        float rotation = GetAngleFromVectorFloat(direction) + 90f;
        int idx = meshParticleSystem.AddQuad(position, rotation, quadSize, false, 0);

        singleList.Add(new Single(meshParticleSystem, lifetime, idx));
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    private class Single
    {
        private MeshParticleSystem meshParticleSystem;
        private int quadIndex;
        private float lifetime_TIMER;

        public Single(MeshParticleSystem meshParticleSystem, float lifetime, int idx)
        {
            this.meshParticleSystem = meshParticleSystem;

            lifetime_TIMER = lifetime;
            quadIndex = idx;
        }

        public void Update() => lifetime_TIMER -= Time.deltaTime;

        public bool IsComplete() => lifetime_TIMER <= 0;

        public void DestroySelf() => meshParticleSystem.DestroyQuad(quadIndex);

    }

}
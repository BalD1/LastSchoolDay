using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightArena : MonoBehaviour
{
    [SerializeField] private AnimationCurve spawnCurve;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float timeBetweenSpawns = 1f;

    private List<EnemyBase> enemies;

    public bool started;

    private bool isAtLastSpawn;

    public void SpawnNext(int idx)
    {
        started = true;
        if (idx >= spawnCurve.length)
        {
            isAtLastSpawn = true;
            return;
        }

        float enemiesNb = spawnCurve.Evaluate(idx);

        for (int i = 0; i < enemiesNb; i++)
        {
            Vector2 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            NormalZombie.Create(pos);
        }

        StartCoroutine(WaitForNext(idx));
    }

    private void Update()
    {
        if (isAtLastSpawn && enemies.Count <= 0) GameManager.Instance.GameState = GameManager.E_GameState.Win;
    }

    public IEnumerator WaitForNext(int idx)
    {
        yield return new WaitForSeconds(timeBetweenSpawns);

        SpawnNext(idx);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BossZombieEvents
{
    public static event Action<BossZombie> OnBossSpawn;
    public static void BossSpawn(this BossZombie boss)
        => OnBossSpawn?.Invoke(boss);

    public static event Action<BossZombie> OnBossDeath;
    public static void BossDeath(this BossZombie boss)
        => OnBossDeath?.Invoke(boss);
}

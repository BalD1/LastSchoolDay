using System.Collections.Generic;
using UnityEngine;

public class BossHUDManager : Singleton<BossHUDManager>
{
    [field: SerializeField] public CanvasGroup hudContainer { get; private set; }
    [field: SerializeField] public RectTransform hudParent { get; private set; }
    [field: SerializeField] public RectTransform hudFadeTarget { get; private set; }

    [SerializeField] private BossHUD bossHUD_PF;

    private Dictionary<BossZombie, BossHUD> bossesHUD = new Dictionary<BossZombie, BossHUD>();

    protected override void EventsSubscriber()
    {
        BossZombieEvents.OnBossSpawn += AddBoss;
        BossZombieEvents.OnBossDeath += DeleteElement;
    }

    protected override void EventsUnSubscriber()
    {
        BossZombieEvents.OnBossSpawn -= AddBoss;
        BossZombieEvents.OnBossDeath -= DeleteElement;
    }

    protected override void Awake()
    {
        base.Awake();
        bossesHUD = new Dictionary<BossZombie, BossHUD>();
    }

    public void LeanContainer(bool leanIn, float leanTime = 1)
    {
        hudContainer.LeanAlpha(leanIn ? 1 : 0, leanTime);
    }

    public void AddBoss(BossZombie boss)
    {
        if (bossesHUD.ContainsKey(boss))
        {
            this.Log("Dictionary already contains " + boss.name, LogsManager.E_LogType.Error);
            return;
        }
        BossHUD bossHUD = bossHUD_PF.Create(hudParent);
        bossesHUD.Add(boss, bossHUD);
        bossHUD.Setup(boss);
    }
    
    public int GetBossHUDsCount() => bossesHUD.Count;

    public void DeleteElement(BossZombie boss)
    {
        if (!bossesHUD.TryGetValue(boss, out BossHUD hud)) return;

        bossesHUD.Remove(boss);
        Destroy(hud.gameObject);
    }
}

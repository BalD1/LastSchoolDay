using System.Collections.Generic;
using UnityEngine;

public class BossHUDManager : Singleton<BossHUDManager>
{
    [field: SerializeField] public CanvasGroup hudContainer { get; private set; }
    [field: SerializeField] public RectTransform hudParent { get; private set; }
    [field: SerializeField] public RectTransform hudFadeTarget { get; private set; }

    [SerializeField] private GameObject bossHUD_PF;

    [SerializeField] private List<GameObject> bossesHUD;

    protected override void Awake()
    {
        base.Awake();
        bossesHUD = new List<GameObject>();
    }

    public void LeanContainer(bool leanIn, float leanTime = 1)
    {
        hudContainer.LeanAlpha(leanIn ? 1 : 0, leanTime);
    }

    public void AddBoss(BossZombie boss)
    {
        GameObject gO = bossHUD_PF.Create(hudParent);

        BossHUD bossHUD = gO.GetComponent<BossHUD>();

        bossesHUD.Add(gO);

        bossHUD.Setup(boss);
    }
    
    public int GetBossHUDsCount() => bossesHUD.Count;

    public void DeleteElement(GameObject gO)
    {
        int idxToDelete = -1;

        for (int i = 0; i < bossesHUD.Count; i++)
        {
            if (bossesHUD[i] == gO)
            {
                idxToDelete = i;
                break;
            }
        }

        if (idxToDelete != -1)
            bossesHUD.RemoveAt(idxToDelete);
    }
}

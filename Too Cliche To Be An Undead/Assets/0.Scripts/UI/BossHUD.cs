using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    [field: SerializeField] public Image fillImage;
    [SerializeField] private TextMeshProUGUI bossName_TXT;
    [SerializeField] private CanvasGroup hudGroup;

    private BossZombie relatedBoss;

    public void Setup(BossZombie boss)
    {
        if (boss == null)
        {
#if UNITY_EDITOR
            Debug.LogError("Boss was null", this.gameObject);
#endif
            return;
        }
        relatedBoss = boss;
        fillImage.fillAmount = 1;

        relatedBoss.OnTakeDamageFromEntity += UpdateFillAmount;
        bossName_TXT.text = relatedBoss.GetStats.EntityName;
        hudGroup.LeanAlpha(1, .25f).setIgnoreTimeScale(true);
    }

    public void UpdateFillAmount(bool critDamages, Entity damager, bool tickDamages)
    {
        fillImage.fillAmount = relatedBoss.CurrentHP / relatedBoss.MaxHP_M;
    }
}

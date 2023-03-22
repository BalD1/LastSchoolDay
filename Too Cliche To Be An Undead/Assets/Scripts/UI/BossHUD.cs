using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHUD : MonoBehaviour
{
    [field: SerializeField] public Image fillImage;

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

        relatedBoss.D_onTakeDamagesFromEntity += UpdateFillAmount;
        relatedBoss.d_OnDeath += OnDeath;
    }

    public void UpdateFillAmount(bool critDamages, Entity damager)
    {
        fillImage.fillAmount = relatedBoss.CurrentHP / relatedBoss.MaxHP_M;
    }

    public void OnDeath()
    {
        relatedBoss.D_onTakeDamagesFromEntity -= UpdateFillAmount;
        relatedBoss.d_OnDeath -= OnDeath;

        BossHUDManager.Instance.DeleteElement(this.gameObject);

        Destroy(this.gameObject);
    }
}

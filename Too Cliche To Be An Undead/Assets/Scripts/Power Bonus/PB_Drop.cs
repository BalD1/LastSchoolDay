using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Drop : Collectable
{
    [SerializeField] private SCRPT_PB bonusPower;

    private void Awake()
    {
        if (bonusPower != null)
            spriteRenderer.sprite = bonusPower.Thumbnail;
    }

    public void Setup(SCRPT_PB pb)
    {
        bonusPower = pb;
        spriteRenderer.sprite = bonusPower.Thumbnail;
    }

    public override void Interact(GameObject interactor)
    {
        if (pickupOnCollision) return;
        base.Interact(interactor);

        float val;

        PlayerCharacter player = interactor.GetComponentInParent<PlayerCharacter>();
        if (player.GetCharacterName().Equals(bonusPower.AssociatedCharacter)) val = bonusPower.AC_Amount;
        else val = bonusPower.Amount;

        player.AddModifier(bonusPower.ID, val, bonusPower.StatType);
        UIManager.Instance.AddPBToContainer(bonusPower, player.PlayerIndex);
    }
}

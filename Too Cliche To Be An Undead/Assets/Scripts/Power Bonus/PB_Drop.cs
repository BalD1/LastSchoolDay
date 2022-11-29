using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Drop : Collectable
{
    [SerializeField] private SCRPT_PB bonusPower;

    private void Awake()
    {
        spriteRenderer.sprite = bonusPower.Thumbnail;
    }

    protected override void TouchedPlayer(PlayerCharacter player)
    {
        base.TouchedPlayer(player);


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

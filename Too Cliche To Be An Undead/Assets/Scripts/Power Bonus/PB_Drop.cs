using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PB_Drop : Collectable
{
    [SerializeField] private SCRPT_PB bonusPower;
    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        sprite.sprite = bonusPower.Thumbnail;
    }

    protected override void TouchedPlayer(PlayerCharacter player)
    {
        base.TouchedPlayer(player);
        player.AddModifier(bonusPower.ID, bonusPower.Amount, bonusPower.StatType);
        UIManager.Instance.AddPBToContainer(bonusPower);
    }
}

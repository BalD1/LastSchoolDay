using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : Collectable
{
    protected override void TouchedPlayer(PlayerCharacter player)
    {
        if (!pickupOnCollision) return;
        PlayerCharacter.AddMoney(coinValue);

        AudioclipPlayer.Create(this.transform.position, pickupSound);
    }

}


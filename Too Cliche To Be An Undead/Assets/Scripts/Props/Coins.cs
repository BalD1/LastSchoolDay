using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : Collectable
{
    protected override void TouchedPlayer(PlayerCharacter player)
    {
        base.TouchedPlayer(player);
        player.AddMoney(coinValue);
    }

}


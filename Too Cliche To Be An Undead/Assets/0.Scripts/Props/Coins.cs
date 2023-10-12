using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : Collectable
{
    protected override void TouchedPlayer(PlayerCharacter player)
    {
        if (!pickupOnCollision) return;
        InventoryManager.Instance.AddMoney(coinValue);

        PlayPickupSound();
    }

    protected override void PlayPickupSound()
    {
        if (CoinsOptimizer.Instance.CanPlayAudio() == false) return;

        AudioclipPlayer.Create(this.transform.position, pickupSound);

        CoinsOptimizer.Instance.CreatedAudio(pickupSound.length);
    }

    protected override GameObject CreateParticles()
    {
        if (CoinsOptimizer.Instance.CanCreateParticles() == false) return null;

        CoinsOptimizer.Instance.CreatedParticles();
        return base.CreateParticles();
    }

}


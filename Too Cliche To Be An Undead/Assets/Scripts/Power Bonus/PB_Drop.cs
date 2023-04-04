using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PB_Drop : Collectable
{
    [SerializeField] private SCRPT_PB bonusPower;

    [SerializeField] private AnimationCurve fallSpeedY;
    [SerializeField] private AnimationCurve fallSpeedX;

    private float animationTimer = 0;

    private float minTime;

    private Vector2 vel;

    private void Awake()
    {
        canBePickedUp = false;
        if (bonusPower != null)
            spriteRenderer.sprite = bonusPower.Thumbnail;

        if (fallSpeedX.length <= 0)
        {
            if (fallSpeedY.length <= 0)
            {
                minTime = -1;
                canBePickedUp = true;
                return;
            }

            minTime = fallSpeedY.keys[fallSpeedY.length - 1].time;
            return;
        }

        if (fallSpeedY.length <= 0)
        {
            minTime = fallSpeedX.keys[fallSpeedX.length - 1].time;
            return;
        }

        minTime = fallSpeedX.keys[fallSpeedX.length - 1].time;
        float minTimeY = fallSpeedY.keys[fallSpeedY.length - 1].time;

        if (minTime > minTimeY) minTime = minTimeY;

        if (minTime <= 0)
        {
            animationTimer = 1;
            canBePickedUp = true;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (animationTimer > minTime) return;

        animationTimer += Time.deltaTime;

        if (animationTimer > minTime)
        {
            this.rb.velocity = Vector2.zero;
            canBePickedUp = true;
            return;
        }
        vel.x = fallSpeedX.Evaluate(animationTimer);
        vel.y = fallSpeedY.Evaluate(animationTimer);

        this.rb.velocity = vel;
    }

    public void Setup(SCRPT_PB pb)
    {
        bonusPower = pb;
        spriteRenderer.sprite = bonusPower.Thumbnail;
    }

    public override void Interact(GameObject interactor)
    {
        if (pickupOnCollision || !canBePickedUp) return;
        base.Interact(interactor);

        float val;

        PlayerCharacter player = interactor.GetComponentInParent<PlayerCharacter>();
        if (player.GetCharacterName().Equals(bonusPower.AssociatedCharacter)) val = bonusPower.AC_Amount;
        else val = bonusPower.Amount;

        player.AddModifier(bonusPower.ID, val, bonusPower.StatType);

        D_onPickUp += CreateText;
    }

    private void CreateText()
    {
        string txt = bonusPower.AssociatedCharacter == detectedPlayer.GetCharacterName() ? bonusPower.AC_Description : bonusPower.Description;

        TextPopup.Create(txt, (Vector2)detectedPlayer.transform.position + detectedPlayer.GetHealthPopupOffset, GameAssets.ItemComponents);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsSwitch : MonoBehaviour
{
    [SerializeField] private Sprite[] armsSprites;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool isValid;

    private int spriteIdx = -1;

    // spriteIdx 0 => 90 < angle < 45 : haut
    // spriteIdx 1 => else : milieu
    // spriteIdx 2 => 270 < angle < 315 : bas

    private void Awake()
    {
        isValid = armsSprites.Length == 3;

        spriteRenderer ??= this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isValid) return;

        float angle = this.transform.localRotation.eulerAngles.z;
        if ((90 > angle && angle > 45))
        {
            if (spriteIdx == 0) return;
            spriteIdx = 0;
            spriteRenderer.sprite = armsSprites[0];
            return;
        }

        if ((270 < angle && angle < 315))
        {
            if (spriteIdx == 2) return;

            spriteIdx = 2;
            spriteRenderer.sprite = armsSprites[2];
            return;
        }

        if (spriteIdx == 1) return;

        spriteIdx = 1;
        spriteRenderer.sprite = armsSprites[1];
    }

    public void SwitchImages(Sprite[] newSprites)
    {
        armsSprites = newSprites;
        isValid = armsSprites.Length == 3;
    }
}

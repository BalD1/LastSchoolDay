using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviour
{
    [SerializeField] private PlayerWeapon ownerWeapon;

    public void PerformAttack()
    {
        ownerWeapon.animIsReady = true;
    }
}

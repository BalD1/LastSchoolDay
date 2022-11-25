using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityWatcher : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    private bool isVisible = true;

    private int playerIdx = -1;

    private void Start()
    {
        playerIdx = owner.PlayerIndex;
    }

    private void OnBecameInvisible()
    {
        if (isVisible == false || owner.IsAlive() == false) return;

        CameraManager.Instance.PlayerBecameInvisible(this.transform.parent, playerIdx);
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        if (isVisible == true || owner.IsAlive() == false) return;

        CameraManager.Instance.PlayerBecameVisible(this.transform.parent);
        isVisible = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityWatcher : MonoBehaviour
{
    private bool isVisible = true;

    private int playerIdx = -1;

    private void Start()
    {
        playerIdx = this.GetComponentInParent<PlayerCharacter>().PlayerIndex;
    }

    private void OnBecameInvisible()
    {
        if (isVisible == false) return;

        CameraManager.Instance.PlayerBecameInvisible(this.transform.parent, playerIdx);
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        if (isVisible == true) return;

        CameraManager.Instance.PlayerBecameVisible(this.transform.parent);
        isVisible = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityWatcher : MonoBehaviour
{
    private bool isVisible = true;

    private void OnBecameInvisible()
    {
        if (isVisible == false) return;

        CameraManager.Instance.PlayerBecameInvisible(this.transform);
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        if (isVisible == true) return;

        CameraManager.Instance.PlayerBecameVisible(this.transform);
        isVisible = true;
    }
}

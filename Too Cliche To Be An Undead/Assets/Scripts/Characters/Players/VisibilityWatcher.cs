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
        if (owner != null)
            playerIdx = owner.PlayerIndex;
    }

    public void Setup(PlayerCharacter _owner)
    {
        this.owner = _owner;
        playerIdx = owner.PlayerIndex;
    }

    public void ForceVisibility()
    {
        if (this.transform.parent == null) return;
        if (owner == null) return;

        if (isVisible == true || owner.IsAlive() == false) return;

        CameraManager.Instance.PlayerBecameVisible(this.owner.transform);
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        if (GameManager.isAppQuitting) return;
        if (owner == null) return;

        if (this.transform.parent == null) return;

        if (isVisible == false || owner.IsAlive() == false) return;

        CameraManager.Instance.PlayerBecameInvisible(this.owner.transform, playerIdx);
        isVisible = false;
    }

    private void OnBecameVisible()
    {
        if (GameManager.Instance.GameState != GameManager.E_GameState.InGame) return;
        if (this.transform.parent == null) return;
        if (owner == null) return;

        if (isVisible == true || owner.IsAlive() == false) return;

        CameraManager.Instance.PlayerBecameVisible(this.owner.transform);
        isVisible = true;
    }
}

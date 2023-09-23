using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TEMP_HubDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 playerTPPos;
    [SerializeField] private Collider2D playerCounterTrigger;

    [SerializeField] private TextMeshPro playersCounter;
    private int currentCounter = 0;
    private int maxPlayers;

    private bool hasSpawnedKeys = false;

    private void Start()
    {
        maxPlayers = IGPlayersManager.PlayersCount;
        hasSpawnedKeys = false;
        UpdateText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasSpawnedKeys) return;

        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            if (currentCounter < maxPlayers)
            {
                currentCounter++;
                UpdateText();
            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hasSpawnedKeys) return;

        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            currentCounter--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
        if (playersCounter == null) return;

        StringBuilder sb = new StringBuilder();

        sb.Append(currentCounter);
        sb.Append(" / ");
        sb.Append(maxPlayers);

        playersCounter.text = sb.ToString();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerTPPos, 0.5f);
    }

    public void EnteredInRange(GameObject interactor)
    {
        if (currentCounter > 0) return;
        spriteRenderer.material = GameAssets.Instance.OutlineMaterial;
    }

    public void ExitedRange(GameObject interactor)
    {
        if (currentCounter > 0) return;
        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public void Interact(GameObject interactor)
    {
        if (currentCounter < maxPlayers)
            return;


        playerCounterTrigger.enabled = false;
        spriteRenderer.color = Color.red;

        foreach (var item in IGPlayersManager.Instance.PlayersList)
        {
            item.transform.position = playerTPPos;
        }

        SpawnersManager.Instance.ManageKeycardSpawn();
        hasSpawnedKeys = true;
        
        UIManager.Instance.KeycardContainer.SetActive(true);
        playersCounter.gameObject.SetActive(false);

        spriteRenderer.material = GameAssets.Instance.DefaultMaterial;
    }

    public bool CanBeInteractedWith()
    {
        return !hasSpawnedKeys;
    }

    public float GetDistanceFrom(Transform target)
    {
        return Vector2.Distance(target.position, this.transform.position);
    }
}

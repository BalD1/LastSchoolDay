using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TEMP_HubDoor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 playerTPPos;
    [SerializeField] private BoxCollider2D boxCollider2D;

    [SerializeField] private TextMeshPro playersCounter;
    private int currentCounter = 0;
    private int maxPlayers;

    private void Start()
    {
        maxPlayers = GameManager.Instance.PlayersCount;
        UpdateText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            if (currentCounter < maxPlayers)
            {
                currentCounter++;
                UpdateText();

                if (currentCounter < maxPlayers)
                    return;
            }

            boxCollider2D.isTrigger = false;
            spriteRenderer.color = Color.red;

            foreach (var item in GameManager.Instance.playersByName)
            {
                item.playerScript.gameObject.transform.position = playerTPPos;
            }

            UIManager.Instance.KeycardContainer.SetActive(true);
            playersCounter.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            currentCounter--;
            UpdateText();
        }
    }

    private void UpdateText()
    {
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
}

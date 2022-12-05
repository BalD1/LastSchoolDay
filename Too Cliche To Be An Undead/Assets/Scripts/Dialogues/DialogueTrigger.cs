using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string dialogueToStart;

    private bool triggerFlag = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggerFlag)
        {
            triggerFlag = true;
            DialogueManager.Instance.TryStartDialogue(dialogueToStart);
            Destroy(this.gameObject);
        }
    }
}

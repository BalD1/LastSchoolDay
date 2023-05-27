using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [InspectorButton(nameof(SearchAndUpdateDialogueListRepeter), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] protected string dialogueToStart;

    protected bool triggerFlag = false;

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggerFlag)
        {
            triggerFlag = true;
            DialogueManager.Instance.TryStartDialogue(dialogueToStart);
            Destroy(this.gameObject);
        }
    }

    private void SearchAndUpdateDialogueListRepeter() => DialogueManager.SearchAndUpdateDialogueList();



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDialogueTrigger : DialogueTrigger
{
    [SerializeField] private Tutorial tutorial;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !triggerFlag)
        {
            triggerFlag = true;
            DialogueManager.Instance.TryStartDialogue(dialogueToStart, true, tutorial.AnimateNextTutorial);
            Destroy(this.gameObject);
        }
    }
}

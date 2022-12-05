using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string startDialogue;

    private void Start()
    {
        if (DataKeeper.Instance.skipTuto)
        {
            Destroy(this.gameObject);
            return;
        }

        StartFirstDialogue();
    }

    public void StartFirstDialogue() => DialogueManager.Instance.TryStartDialogue(startDialogue);
}

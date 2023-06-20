using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicDialoguePlayer : CinematicAction
{
    private SCRPT_SingleDialogue dialogueToPlay;

    public CinematicDialoguePlayer(SCRPT_SingleDialogue _dialogueToPlay)
    {
        Setup(_dialogueToPlay);
    }

    public void Setup(SCRPT_SingleDialogue _dialogueToPlay)
    {
        dialogueToPlay = _dialogueToPlay;
    }
    public override void Execute()
    {
        if (DialogueManager.Instance == null)
        {
            this.ActionEnded(this);
            return;
        }

        if (!DialogueManager.Instance.TryStartDialogue(dialogueToPlay, false, this.ActionEnded))
            this.ActionEnded(this);
        else
        {
            foreach (var player in owner.Players)
            {
                player.AnimationController.SetAnimation(player.AnimationController.animationsData.IdleAnim, true);
            }
        }
    }

    private void ActionEnded() => ActionEnded(this);
}

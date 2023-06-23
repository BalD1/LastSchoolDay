using UnityEngine;

[System.Serializable]
public class CA_CinematicDialoguePlayer : CA_CinematicAction
{
    [SerializeField] private SCRPT_SingleDialogue dialogueToPlay;

    public CA_CinematicDialoguePlayer(SCRPT_SingleDialogue _dialogueToPlay)
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
            this.Log("DialogueManager Instance was not set. Skipping Cinematic Action.");
            this.ActionEnded(this);
            return;
        }

        if (!DialogueManager.Instance.TryStartDialogue(dialogueToPlay, true, this.ActionEnded))
        {
            this.Log(dialogueToPlay + " dialogue was not set in Manager. Skipping Cinematic Action.");
            this.ActionEnded(this);
        }
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

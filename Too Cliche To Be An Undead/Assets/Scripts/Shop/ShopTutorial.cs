using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTutorial : MonoBehaviour
{
    [InspectorButton(nameof(DelegateUpdateNmaesOnList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string shopTutoDialogue;

    public void StartTutorial()
    {
        CameraManager.Instance.MoveCamera(new Vector2(0.24f, 3.31f), 
            () =>
            {
                DialogueManager.Instance.TryStartDialogue(shopTutoDialogue, OnDialogueEnd);
            });
        
    }

    private void OnDialogueEnd()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;
        UIManager.Instance.FadeAllHUD(fadeIn: false);

        GameManager.Instance.GetShop.OpenShop();

        CameraManager.Instance.EndCinematic();
    }

    private void DelegateUpdateNmaesOnList()
    {
        DialogueManager.SearchAndUpdateDialogueList();
    }
}

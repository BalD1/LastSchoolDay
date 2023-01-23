using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTutorial : MonoBehaviour
{
    [InspectorButton(nameof(DelegateUpdateNmaesOnList), ButtonWidth = 300)]
    [SerializeField] private bool updateDialoguesNames;

    [ListToPopup(typeof(DialogueManager), nameof(DialogueManager.DialogueNamesList))]
    [SerializeField] private string shopTutoDialogue;

    [SerializeField] private Transform shopTransform;

    public void StartTutorial()
    {
        Vector2 shopPos = shopTransform.position;
        CameraManager.Instance.MoveCamera(shopPos, 
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

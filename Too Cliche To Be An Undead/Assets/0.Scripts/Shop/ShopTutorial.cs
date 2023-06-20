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

    private Shop shop;

    private void Start()
    {
        shop = GameManager.Instance.GetShop;
    }

    public void StartTutorial()
    {
        Vector2 shopPos = shopTransform.position;
        CameraManager.Instance.MoveCamera(shopPos, 
            () =>
            {
                DialogueManager.Instance.TryStartDialogue(shopTutoDialogue,true, OnDialogueEnd);
            });
        shop.D_closeShop += OnShopClosed;
    }

    private void OnDialogueEnd()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;

        shop.OpenShop();
        
    }

    private void OnShopClosed()
    {
        GameManager.Instance.GameState = GameManager.E_GameState.Restricted;
        UIManager.Instance.FadeAllHUD(false, 0);
        CameraManager.Instance.MoveCamera(GameManager.Player1Ref.transform.position, CameraTravelEnded);
    }

    private void CameraTravelEnded()
    {
        shop.D_closeShop -= OnShopClosed;
        CameraManager.Instance.EndCinematic();
        UIManager.Instance.FadeInGameHUD(true);
        GameManager.Instance.GameState = GameManager.E_GameState.InGame;
    }

    private void DelegateUpdateNmaesOnList()
    {
        DialogueManager.SearchAndUpdateDialogueList();
    }
}

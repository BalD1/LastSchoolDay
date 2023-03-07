using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class EventSystemSwitch : MonoBehaviour
{
    private static EventSystemSwitch instance;
    public static EventSystemSwitch Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] private InputSystemUIInputModule baseUIModule;

    public void SwitchModuleNavigation()
    {
        PlayerCharacter player1 = GameManager.Player1Ref;

        if (player1 == null) return;

        InputActionMap targetMap = player1.Inputs.currentActionMap;
        baseUIModule.move = InputActionReference.Create(targetMap.FindAction("Navigate"));
    }
}

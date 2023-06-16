using UnityEngine.InputSystem;
using UnityEngine;

public class MainMenuPlayersManager : Singleton<MainMenuPlayersManager>
{

    [SerializeField] private InputAction joinAction;
    [SerializeField] private InputAction leaveAction;

    [field: SerializeField] public SO_CharactersComponents[] CharacterComponents { get; private set; }

    protected override void EventsSubscriber()
    {
        base.EventsSubscriber();
    }

    protected override void EventsUnSubscriber()
    {
        base.EventsUnSubscriber();
    }

    protected override void Awake()
    {
        base.Awake();
    }
}
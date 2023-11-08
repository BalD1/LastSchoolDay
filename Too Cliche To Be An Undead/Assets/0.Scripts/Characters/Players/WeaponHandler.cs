using System;
using UnityEngine;

public class WeaponHandler : MonoBehaviourEventsHandler
{
    [SerializeField] private PlayerCharacter owner;
    [field: SerializeField] public PlayerWeaponBase CurrentWeapon { get; private set; }

    public event Action<PlayerWeaponBase> OnCreatedNewWeapon;

    protected override void Awake()
    {
        base.Awake();
        Setup();
    }

    protected override void EventsSubscriber()
    {
        owner.OnCharacterSwitch += OnOwnerCharacterChange;
    }

    protected override void EventsUnSubscriber()
    {
        owner.OnCharacterSwitch -= OnOwnerCharacterChange;
    }

    private void Setup()
    {
        if (owner.CurrentCharacterComponents == null)
            PlayerCharacterEvents.OnPlayerSetup += OnOwnerSetup;
        else
            CreateNewWeapon(owner.CurrentCharacterComponents.WeaponData);
    }

    private void OnOwnerSetup(PlayerCharacter playerCharacter)
    {
        if (playerCharacter != owner) return;
        PlayerCharacterEvents.OnPlayerSetup -= OnOwnerSetup;
        CreateNewWeapon(owner.CurrentCharacterComponents.WeaponData);
    }

    private void OnOwnerCharacterChange(SO_CharactersComponents newComponent)
    {
        CreateNewWeapon(newComponent.WeaponData);
    }

    private void CreateNewWeapon(SO_WeaponData weaponData)
    {
        PlayerWeaponBase newWeapon = new PlayerWeaponBase(weaponData, owner, this);
        CurrentWeapon = newWeapon;
        this.OnCreatedNewWeapon?.Invoke(newWeapon);
    }

    private void Update()
    {
        CurrentWeapon.Update();
    }
}

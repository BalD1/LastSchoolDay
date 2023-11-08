public interface IWeapon
{
    public void Setup(SO_WeaponData weaponData, IComponentHolder owner, WeaponHandler handler);
    public void Update();
    public bool AskAttack();
    public void StartAttack();
    public void QueueNextAttack();
}
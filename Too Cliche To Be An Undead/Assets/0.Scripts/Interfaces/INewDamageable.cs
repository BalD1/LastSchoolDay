
public interface INewDamageable
{
    public abstract void UpdateMaxHealth(float newMaxHealth, bool healDifference);
    public abstract void InflictDamages(float amount, bool isCrit);
    public abstract void Heal(float amount, bool isCrit);
    public abstract void Death();
    public bool IsAlive();
}

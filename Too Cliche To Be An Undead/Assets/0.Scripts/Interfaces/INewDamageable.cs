
public interface INewDamageable
{
    public abstract bool TakeDamages(float amount, Entity damager, bool isCrit);

    public abstract void Heal(float amount, Entity healer, bool isCrit);

    public abstract void Death();

    public bool IsAlive();
}

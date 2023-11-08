
using UnityEngine;

public interface ITargetable
{
    public abstract void AddAttacker(Entity entity);
    public abstract void RemoveAttacker(Entity entity);

    public abstract int GetAttackersCount();

    public abstract Vector2 GetPosition();
}
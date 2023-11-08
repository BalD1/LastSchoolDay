
using UnityEngine;

public interface IPathfindingTarget
{
    public abstract Vector2 GetPosition();
    public abstract Vector2 GetVelocity();
    public abstract float GetSpeed();
}
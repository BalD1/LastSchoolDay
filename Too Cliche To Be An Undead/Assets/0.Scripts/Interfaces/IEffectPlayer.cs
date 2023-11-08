using UnityEngine;

public interface IEffectPlayer
{
    public void PlayAt(Vector2 position, Quaternion rotation);
    public void AttachTo(Transform parent);
}
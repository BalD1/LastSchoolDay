using AYellowpaper.SerializedCollections;
using Spine.Unity;
using UnityEngine;

public abstract class SO_AnimationsData<Key> : ScriptableObject
{
    protected bool Intern_TryGetAnimationData(Key key, SerializedDictionary<Key, S_StateAnimationData> dictionary, out S_StateAnimationData animationData)
    {
        if (!dictionary.TryGetValue(key, out animationData)) return false;
        return true;
    }
    public abstract bool TryGetAnimationData(Key key, out S_StateAnimationData animationData);
}

[System.Serializable]
public struct S_StateAnimationData
{
    [field: SerializeField] public AnimationReferenceAsset Asset { get; private set; }
    [field: SerializeField] public bool Loop { get; private set; }

    public float AnimationDuration { get => Asset.Animation.Duration; }
}

[System.Serializable]
public struct S_DirectionalAnimationData
{
    [field: SerializeField] public AnimationReferenceAsset SideAnimation { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset BackAnimation { get; private set; }
    [field: SerializeField] public AnimationReferenceAsset FrontAnimation { get; private set; }

    [field: SerializeField] public bool Loop { get; private set; }

    public AnimationReferenceAsset GetAnimation(GameManager.E_Direction direction)
    {
        switch (direction)
        {
            case GameManager.E_Direction.Left or GameManager.E_Direction.Right: return SideAnimation;
            case GameManager.E_Direction.Down: return FrontAnimation;
            case GameManager.E_Direction.Up: return BackAnimation;
        }
        return null;
    }

    public float GetAnimationDuration(GameManager.E_Direction direction)
    {
        switch (direction)
        {
            case GameManager.E_Direction.Left or GameManager.E_Direction.Right: return SideAnimation.Animation.Duration;
            case GameManager.E_Direction.Down: return FrontAnimation.Animation.Duration;
            case GameManager.E_Direction.Up: return BackAnimation.Animation.Duration;
        }
        return -1;
    }
}
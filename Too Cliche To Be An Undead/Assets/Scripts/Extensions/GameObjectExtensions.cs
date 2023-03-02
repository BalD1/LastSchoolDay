using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject Create(this GameObject gameObject, Vector2 position)
    {
        GameObject gO = GameObject.Instantiate(gameObject, position, Quaternion.identity);
        return gO;
    }
    public static GameObject Create(this GameObject gameObject, Vector2 position, Quaternion quaternion)
    {
        GameObject gO = GameObject.Instantiate(gameObject, position, quaternion);
        return gO;
    }

    public static GameObject Create(this GameObject gameObject, Transform parent)
    {
        GameObject gO = GameObject.Instantiate(gameObject, parent);
        return gO;
    }
}

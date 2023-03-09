
using UnityEngine;

public static class TransformExtensions
{
    public static void SetLocalPositionX(this Transform transform, float newPosX)
    {
        Vector3 pos = transform.localPosition;
        pos.x = newPosX;
        transform.localPosition = pos;
    }
    public static void SetLocalPositionY(this Transform transform, float newPosY)
    {
        Vector3 pos = transform.localPosition;
        pos.y = newPosY;
        transform.localPosition = pos;
    }
    public static void SetLocalPositionZ(this Transform transform, float newPosZ)
    {
        Vector3 pos = transform.localPosition;
        pos.z = newPosZ;
        transform.localPosition = pos;
    }

    public static void AddToLocalPositionX(this Transform transform, float x)
    {
        Vector3 pos = transform.localPosition;
        pos.x += x;
        transform.localPosition = pos;
    }
    public static void AddToLocalPositionY(this Transform transform, float y)
    {
        Vector3 pos = transform.localPosition;
        pos.y += y;
        transform.localPosition = pos;
    }
    public static void AddToLocalPositionZ(this Transform transform, float z)
    {
        Vector3 pos = transform.localPosition;
        pos.z += z;
        transform.localPosition = pos;
    }
}

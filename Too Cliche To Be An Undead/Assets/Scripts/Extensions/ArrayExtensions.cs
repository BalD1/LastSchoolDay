using UnityEngine;

public static class ArrayExtensions
{
    public static int RandomIndex<T>(this T[] array) => Random.Range(0, array.Length);

    public static T RandomElement<T>(this T[] array) => array[Random.Range(0, array.Length)];
}
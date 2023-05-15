using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonArgs<T> : MonoBehaviour
{
    public T args;
    public T GetArgs { get => args; }
}

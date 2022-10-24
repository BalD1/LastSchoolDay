using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM_ManagerBase : MonoBehaviour
{
    protected abstract void Start();
    protected abstract void Update();
    protected abstract void FixedUpdate();

    public abstract override string ToString();
}

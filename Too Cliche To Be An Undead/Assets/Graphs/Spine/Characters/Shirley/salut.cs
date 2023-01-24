using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[ExecuteInEditMode]
public class salut : MonoBehaviour
{
    public float mdr;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, mdr);
    }
}

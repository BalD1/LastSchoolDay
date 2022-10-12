using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        Debug.Log(Physics2D.OverlapCircle(this.transform.position, 10));
    }
}

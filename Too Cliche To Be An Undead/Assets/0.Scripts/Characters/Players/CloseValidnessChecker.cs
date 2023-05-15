using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseValidnessChecker : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        {
            AreaSpawner areaSpawner = collision.GetComponent<AreaSpawner>();
            if (areaSpawner != null)
            {
                areaSpawner.SetValidity(false);
                return;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        {
            AreaSpawner areaSpawner = collision.GetComponent<AreaSpawner>();
            if (areaSpawner != null)
            {
                areaSpawner.SetValidity(true);
                return;
            }
        }
    }
}

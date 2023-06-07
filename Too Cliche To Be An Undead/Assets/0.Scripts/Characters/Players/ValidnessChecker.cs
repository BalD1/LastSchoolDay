using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidnessChecker : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDistanceChecker checker = collision.GetComponent<IDistanceChecker>();
        if (checker == null) return;
        checker.OnEnteredFarCheck();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IDistanceChecker checker = collision.GetComponent<IDistanceChecker>();
        if (checker == null) return;
        checker.OnExitedFarCheck();
    }
}

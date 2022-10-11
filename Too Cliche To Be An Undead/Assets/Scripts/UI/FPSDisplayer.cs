using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BalDUtilities.Misc;

public class FPSDisplayer : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        textMesh = this.GetComponent<TextMeshProUGUI>();

        if (textMesh == null)
        {
            Debug.LogError("Couldn't find TextMeshProUGUI component.", this.gameObject);
            Destroy(this);
        }
    }

    private void Update()
    {
        textMesh.text = FPS.GetFPSRounded().ToString() + " FPS";
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PH_DebugText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textComponent;

    public void ChangeText(string newText) => textComponent.text = newText;
}

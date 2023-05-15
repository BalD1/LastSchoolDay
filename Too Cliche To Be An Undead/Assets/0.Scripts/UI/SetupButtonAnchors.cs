using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupButtonAnchors : MonoBehaviour
{
    [InspectorButton(nameof(SetupAnchors), ButtonWidth = 150)]
    [SerializeField] private bool setupAnchors;

    public void SetupAnchors()
    {
        RectTransform selfRectTransform = this.transform as RectTransform;
        RectTransform parentTransform = this.transform.parent as RectTransform;

        Vector2 newAnchorsMin = new Vector2(selfRectTransform.anchorMin.x + selfRectTransform.offsetMin.x / parentTransform.rect.width,
                                            selfRectTransform.anchorMin.y + selfRectTransform.offsetMin.y / parentTransform.rect.height);
        Vector2 newAnchorsMax = new Vector2(selfRectTransform.anchorMax.x + selfRectTransform.offsetMax.x / parentTransform.rect.width,
                                            selfRectTransform.anchorMax.y + selfRectTransform.offsetMax.y / parentTransform.rect.height);

        selfRectTransform.anchorMin = newAnchorsMin;
        selfRectTransform.anchorMax = newAnchorsMax;
        selfRectTransform.offsetMin = selfRectTransform.offsetMax = new Vector2(0, 0);
    }
}

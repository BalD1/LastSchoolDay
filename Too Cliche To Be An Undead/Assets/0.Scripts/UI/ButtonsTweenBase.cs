using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsTweenBase : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector3 maxScale = new Vector3( 1.2f, 1.2f, 1.2f );

    [SerializeField] private float scaleTime = .4f;

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeInOutSine;


    private void Start()
    {
        if (rectTransform == null) rectTransform = this.GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.selectedObject == this.gameObject) return;
        ScaleSelf(inType, maxScale, scaleTime);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.selectedObject == this.gameObject) return;
        ScaleSelf(outType, Vector3.one, scaleTime);
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        ScaleSelf(inType, maxScale, scaleTime);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        ScaleSelf(outType, Vector3.one, scaleTime);
    }

    private LTDescr ScaleSelf(LeanTweenType leanType, Vector3 goal, float time)
    {
        if (rectTransform == null) rectTransform = this.GetComponent<RectTransform>();
        return LeanTween.scale(rectTransform, goal, time).setEase(leanType).setIgnoreTimeScale(true);
    }
}

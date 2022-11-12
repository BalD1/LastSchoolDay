using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PBThumbnail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);

    [SerializeField] private float scaleTime = .4f;

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeInOutSine;

    private bool isSelected = false;
    private bool isHovered = false;

    private int idx;

    public static PBThumbnail Create(SCRPT_PB pb, int _idx)
    {
        PBThumbnail thumbnail = Instantiate(GameAssets.Instance.PBThumbnailPF, Vector2.zero, Quaternion.identity).GetComponent<PBThumbnail>();

        thumbnail.Setup(pb, _idx);

        return thumbnail;
    }

    public void Setup(SCRPT_PB pb, int _idx)
    {
        thumbnail.sprite = pb.Thumbnail;
        descriptionText.text = pb.Description;
        idx = _idx;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        Selected();
        UIManager.Instance.D_exitPause += Deselected;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (isSelected) return;

        Deselected();
        UIManager.Instance.D_exitPause -= Deselected;
    }

    public virtual void OnSelect(BaseEventData eventData)
    {
        isSelected = true;
        Selected();
        UIManager.Instance.D_exitPause += Deselected;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        isSelected = false;

        if (isHovered) return;

        Deselected();
        UIManager.Instance.D_exitPause -= Deselected;
    }

    private void Selected()
    {
        Canvas.ForceUpdateCanvases();

        int maxByRow = UIManager.maxPBImagesByRows;

        int rowIdx = idx;
        while (rowIdx >= maxByRow) rowIdx -= maxByRow;

        UIManager.Instance.CurrentHorizontalScrollbar.value = (float)rowIdx / maxByRow; 

        tooltip.SetActive(true);
        ScaleSelf(inType, maxScale, scaleTime);
    }

    private void Deselected()
    {
        tooltip.SetActive(false);
        ScaleSelf(outType, Vector3.one, scaleTime);
    }

    private LTDescr ScaleSelf(LeanTweenType leanType, Vector3 goal, float time)
    {
        if (rectTransform == null) rectTransform = this.GetComponent<RectTransform>();
        return LeanTween.scale(rectTransform, goal, time).setEase(leanType).setIgnoreTimeScale(true);
    }
}

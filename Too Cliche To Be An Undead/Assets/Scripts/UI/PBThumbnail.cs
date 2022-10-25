using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using BalDUtilities.MouseUtils;

public class PBThumbnail : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Image thumbnail;
    [SerializeField] private TextMeshProUGUI descriptionText;

    public static PBThumbnail Create(SCRPT_PB pb)
    {
        PBThumbnail thumbnail = Instantiate(GameAssets.Instance.PBThumbnailPF, Vector2.zero, Quaternion.identity).GetComponent<PBThumbnail>();

        thumbnail.Setup(pb);

        return thumbnail;
    }

    public void Setup(SCRPT_PB pb)
    {
        thumbnail.sprite = pb.Thumbnail;
        descriptionText.text = pb.Description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.SetActive(false);
    }
}

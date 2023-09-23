using UnityEngine;
using UnityEngine.UI;

public class UIBlackBars : MonoBehaviour
{
    [SerializeField] private Image[] blackBars;

    [System.Serializable]
    public struct OnBlackBarsHUDMovements
    {
        [field: SerializeField] public RectTransform ObjectToMove { get; private set; }
        [field: SerializeField] public Vector2 MovementAmount { get; private set; }
        [field: SerializeField] public float MovementDuration { get; private set; }
    }
    [SerializeField] private OnBlackBarsHUDMovements[] blackBarsHUDMovements;

    public void SetBlackBars(bool appear, float time = 1f)
    {
        foreach (var item in blackBars)
        {
            LeanTween.value(item.fillAmount, appear ? 1 : 0, time).setIgnoreTimeScale(true).setOnUpdate(
            (float val) =>
            {
                item.fillAmount = val;
            });
        }
        foreach (var item in blackBarsHUDMovements)
        {
            Vector2 targetPos = (Vector2)item.ObjectToMove.localPosition + (appear ? item.MovementAmount : -item.MovementAmount);
            item.ObjectToMove.LeanMoveLocal(targetPos, item.MovementDuration).setIgnoreTimeScale(true);
        }
    }
}

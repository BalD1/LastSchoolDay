using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScorePanel : MonoBehaviour
{
    [field: SerializeField] public Image playerImage;

    [field: SerializeField] public TextMeshProUGUI killsCount;
    [field: SerializeField] public TextMeshProUGUI deathsCount;

    private PlayersScorePanelsController.S_PlayerImages playerImages;

    public delegate void D_AnimationEnded();
    public D_AnimationEnded D_animationEnded;

    public void Setup(PlayersScorePanelsController.S_PlayerImages _playerImages, int _killsCount, int _deathsCount)
    {
        playerImages = _playerImages;

        playerImage.sprite = playerImages.happyImage;

        killsCount.text = "x " + _killsCount;
        deathsCount.text = "x " + _deathsCount;

        D_animationEnded?.Invoke();
    }
}

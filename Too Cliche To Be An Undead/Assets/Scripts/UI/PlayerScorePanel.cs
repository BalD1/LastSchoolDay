using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScorePanel : MonoBehaviour
{
    [field: SerializeField] public Image playerImage;

    [field: SerializeField] public TextMeshProUGUI killsCount;
    [field: SerializeField] public TextMeshProUGUI dealtDamagesCount;
    [field: SerializeField] public TextMeshProUGUI takenDamagesCount;

    private PlayersScorePanelsController.S_PlayerImages playerImages;

    public delegate void D_AnimationEnded();
    public D_AnimationEnded D_animationEnded;

    private PlayerCharacter relatedPlayer;

    public void Setup(PlayersScorePanelsController.S_PlayerImages _playerImages, PlayerCharacter _relatedPlayer)
    {
        relatedPlayer = _relatedPlayer;
        playerImages = _playerImages;

        playerImage.sprite = playerImages.happyImage;

        killsCount.text = "x " + _relatedPlayer.KillsCount;
        dealtDamagesCount.text = "x " + _relatedPlayer.DamagesDealt;
        takenDamagesCount.text = "x " + _relatedPlayer.DamagesTaken;

        D_animationEnded?.Invoke();
    }
}

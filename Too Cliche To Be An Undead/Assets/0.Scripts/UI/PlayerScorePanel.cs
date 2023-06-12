using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScorePanel : MonoBehaviour
{
    [field: SerializeField] public Image haloImage { get; private set; }
    [field: SerializeField] public Image playerImage;

    [field: SerializeField] public TextMeshProUGUI killsCount;
    [field: SerializeField] public TextMeshProUGUI dealtDamagesCount;
    [field: SerializeField] public TextMeshProUGUI takenDamagesCount;
    [field: SerializeField] public TextMeshProUGUI totalScore;

    [field: SerializeField] public TextMeshProUGUI scoreAdditionPop;

    private PlayersScorePanelsController.S_PlayerImages playerImages;

    private PlayersScorePanelsController controller;

    public delegate void D_AnimationEnded();
    public D_AnimationEnded D_animationEnded;

    private Queue<LTDescr> animQueue;

    private int finalScore;
    public int FinalScore { get => finalScore; }

    public void Setup(PlayersScorePanelsController.S_PlayerImages _playerImages, PlayerCharacter _relatedPlayer, PlayersScorePanelsController _controller)
    {
        animQueue = new Queue<LTDescr>();

        controller = _controller;

        playerImages = _playerImages;

        playerImage.sprite = playerImages.neutralImage;

        PlayerEndStatsManager.PlayerEndStats playerEndStats = PlayerEndStatsManager.Instance.GetPlayerEndStats(_relatedPlayer);

        animQueue.Enqueue(AnimateValue(playerEndStats.KillsCount, controller.SingleKillValue, 1, killsCount));
        animQueue.Enqueue(AnimateValue(playerEndStats.DamagesDealt, controller.SingleDamageDealtValue, 1, dealtDamagesCount));
        animQueue.Enqueue(AnimateValue(playerEndStats.DamagesTaken, controller.SingleDamageTakenValue, 1, takenDamagesCount));
        
    }

    public void SetImageToSad()
    {
        playerImage.sprite = playerImages.sadImage;
    }

    public void SetImageToHappy()
    {
        playerImage.sprite = playerImages.happyImage;
        haloImage.LeanAlpha(1, .2f).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            haloImage.LeanAlpha(.75f, .5f).setLoopPingPong(-1).setIgnoreTimeScale(true);
        });
    }

    public void BeginAnim()
    {
        animQueue.Dequeue().resume();
    }

    private LTDescr AnimateValue(int maxVal, int scoreMultiplier, float time, TextMeshProUGUI tmp)
    {
        LTDescr tween;

        int currentVal = 0;
        tween = LeanTween.value(this.gameObject, 0, maxVal, time).setOnUpdate((float val) =>
        {
            currentVal = (int)val;
            tmp.text = "x " + currentVal;
        }).pause();

        tween.setIgnoreTimeScale(true);
        tween.setEaseInOutQuart();

        tween.setOnComplete(() =>
        {
            scoreAdditionPop.rectTransform.position = tmp.rectTransform.position;

            int scoreValue = maxVal * scoreMultiplier;
            finalScore += scoreValue;
            scoreAdditionPop.text = scoreValue.ToString();

            LeanTween.value(scoreAdditionPop.gameObject, scoreAdditionPop.rectTransform.position, totalScore.rectTransform.position, 1f).setEaseInOutBack().setOnUpdate((Vector3 pos) =>
            {
                scoreAdditionPop.rectTransform.position = pos;
            }).setIgnoreTimeScale(true)
            .setOnComplete(() =>
            {
                totalScore.text = finalScore.ToString();
                scoreAdditionPop.text = "";
                if (animQueue.Count > 0) animQueue.Dequeue().resume();
                else D_animationEnded?.Invoke();
            }).setIgnoreTimeScale(true);
        }).setIgnoreTimeScale(true);

        return tween;
    }
}

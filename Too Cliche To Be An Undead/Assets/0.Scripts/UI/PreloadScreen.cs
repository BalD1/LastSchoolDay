using UnityEngine;
using UnityEngine.UI;

public class PreloadScreen : UIScreenBase
{
    [SerializeField] private PlayerPanelsManager panelsManager;

    [SerializeField] private ButtonArgs_Scene sceneArgs;

    [SerializeField] private Image mainScreenImage;
    [SerializeField] private RawImage videoPlayerImage;

    [SerializeField] private float fadeDuration = .5f;

    [SerializeField] private RectTransform playTutoButton;
    [SerializeField] private RectTransform skipTutoButton;

    protected override void OnScreenUp()
    {
        base.OnScreenUp();
        GameManager.Instance.AllowQuitLobby = false;
        AttachInputsToTweens();
    }

    protected override void OnScreenDown()
    {
        base.OnScreenDown();
        DetachInputs();
    }

    private void AttachInputsToTweens()
    {
        PlayerInputsEvents.OnValidateButton += PlayTutorial;
        PlayerInputsEvents.OnFourthAction += SkipTutorial;
    }
    private void DetachInputs()
    {
        PlayerInputsEvents.OnValidateButton -= PlayTutorial;
        PlayerInputsEvents.OnFourthAction -= SkipTutorial;
    }

    public void PlayTutorial(int idx)
    {
        AnimateButton(playTutoButton, false);
    }

    public void SkipTutorial(int idx)
    {
        AnimateButton(skipTutoButton, true);
    }

    private void AnimateButton(RectTransform rectTransform, bool launchSkipTuto)
    {
        mainScreenImage.SetAlpha(0);
        videoPlayerImage.SetAlpha(0);
        LeanTween.scale(rectTransform, Vector3.one * 1.2f, .25f)
            .setOnComplete(() =>
            {
                group.LeanAlpha(0, .5f).setIgnoreTimeScale(true);
                LeanTween.scale(rectTransform, Vector3.zero, .5f).setIgnoreTimeScale(true)
                .setOnComplete(() =>
                {
                    Launch(launchSkipTuto);
                });
            }).setIgnoreTimeScale(true);
    }

    private void Launch(bool skipTuto)
    {

        DataKeeper.Instance.alreadyPlayedTuto = skipTuto;
        DataKeeper.Instance.skipTuto = skipTuto;

        DetachInputs();

        if (sceneArgs == null)
        {
            sceneArgs = new ButtonArgs_Scene();
            sceneArgs.args = GameManager.E_ScenesNames.MainScene;
        }

        GameManager.ChangeScene(sceneArgs.args);

        this.gameObject.SetActive(false);
    }
}
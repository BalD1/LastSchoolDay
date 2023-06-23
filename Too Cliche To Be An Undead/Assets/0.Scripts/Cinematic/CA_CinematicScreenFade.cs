using UnityEngine;

[System.Serializable]
public class CA_CinematicScreenFade : CA_CinematicAction
{
    [SerializeField] private bool fadeIn;
    [SerializeField] private float time;

    public CA_CinematicScreenFade(bool _fadeIn, float _time)
    {
        Setup(_fadeIn, _time);
    }

    public void Setup(bool _fadeIn, float _time)
    {
        fadeIn = _fadeIn;
        time = _time;
    }
    public override void Execute()
    {
        if (UIManager.Instance == null)
        {
            this.Log("UIManager Instance was not set. Skipping Cinematic Action");
            this.ActionEnded(this);
            return;
        }

        UIManager.Instance.FadeScreen(!fadeIn, ActionEnded, time);
    }

    private void ActionEnded() => ActionEnded(this);
}

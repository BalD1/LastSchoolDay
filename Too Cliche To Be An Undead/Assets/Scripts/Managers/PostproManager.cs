using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostproManager : MonoBehaviour
{
    private static PostproManager instance;
    public static PostproManager Instance
    {
        get => instance;
    }

    [field: SerializeField] public Volume MainVolume { get; private set; }

    [InspectorButton(nameof(SearchForVolume))] public bool SetVolume;

    [field: SerializeField] public Vignette MainVolume_vignette;

    [field: SerializeField] public Color vignette_hurtColor;
    [field: SerializeField] public Color vignette_baseColor;

    private LTDescr vignetteTween;

    private DepthOfField DOF;
    private float DOF_baseFocalLength = 18;


    private void Awake()
    {
        instance = this;

        GameManager.Instance.D_onPlayerIsSetup += SubscribeToPlayerEvents;

        if (MainVolume == null) SearchForVolume();

        DepthOfField tmp_DOF;
        if (MainVolume.profile.TryGet<DepthOfField>(out tmp_DOF) == false)
            MissingComponentError("DepthOfField");
        else
        {
            DOF = tmp_DOF;
            DOF_baseFocalLength = DOF.focalLength.value;
            DOF.focalLength.value = 0;
            DOF.active = true;
        }

        Vignette tmp_Vignette;
        if (MainVolume.profile.TryGet<Vignette>(out tmp_Vignette) == false)
            MissingComponentError("Vignette");
        else
            MainVolume_vignette = tmp_Vignette;
    }

    private void SubscribeToPlayerEvents(int playerIdx)
    {
        PlayerCharacter player = GameManager.Instance.playersByName[playerIdx].playerScript;

        player.D_onTakeDamagesFromEntity += (bool c, Entity e) => SetVignetteHurtColor();
    }

    private void SearchForVolume()
    {
        MainVolume = Camera.main.GetComponent<Volume>();
    }

    public void SetVignetteHurtColor()
    {
        if (vignetteTween != null) LeanTween.cancel(vignetteTween.uniqueId);

        MainVolume_vignette.color.Override(vignette_hurtColor);

        vignetteTween = LeanTween.value(this.gameObject, MainVolume_vignette.color.value, vignette_baseColor, .25f).setOnUpdate((Color val) =>
        {
            MainVolume_vignette.color.Override(val);
        });
    }

    public void SetBlurState(bool _active)
    {
        if (DOF == null) return;

        LeanTween.value(DOF.focalLength.value, _active ? DOF_baseFocalLength : 0, .1f).setOnUpdate(
            (float val) =>
            {
                DOF.focalLength.value = val;
            }).setIgnoreTimeScale(true);
    }

    private void MissingComponentError(string component)
    {
#if UNITY_EDITOR
        Debug.LogError("Missing " + component + " in MainVolume.", MainVolume);
#endif
    }
}

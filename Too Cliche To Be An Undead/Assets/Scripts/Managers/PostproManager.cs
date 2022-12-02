using System.Collections;
using System.Collections.Generic;
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

    private DepthOfField DOF;
    private float DOF_baseFocalLength = 18;


    private void Awake()
    {
        instance = this;

        if (MainVolume == null) SearchForVolume();

        if (MainVolume.profile.TryGet<DepthOfField>(out DOF) == false)
            MissingComponentError("DepthOfField");
        else
        {
            DOF_baseFocalLength = DOF.focalLength.value;
            DOF.focalLength.value = 0;
            DOF.active = true;
        }
    }

    private void SearchForVolume()
    {
        MainVolume = Camera.main.GetComponent<Volume>();
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

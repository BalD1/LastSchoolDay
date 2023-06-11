using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpineColorModifier : MonoBehaviour
{
#if UNITY_EDITOR
    [InspectorButton(nameof(EDITOR_SwitchToModifiedColor), ButtonWidth = 200)]
    [SerializeField] private bool switchToModified;

    [InspectorButton(nameof(EDITOR_SwitchToBaseColor), ButtonWidth = 200)]
    [SerializeField] private bool switchToBase;

    [InspectorButton(nameof(SetBaseColorToCurrent), ButtonWidth = 200)]
    [SerializeField] private bool setBaseColorToCurrent;

    [SerializeField] private bool editor_TestTargetColor;
    [SerializeField] private bool editor_TestBaseColor;
#endif

    [field: SerializeField] public Color TargetColor { get; private set; }
    [field: SerializeField] public Color BaseColor { get; private set; }

    [field: SerializeField] public SkeletonAnimation skeletonAnimation { get; private set; }
    private Skeleton SelfSkeleton;

    private void Reset()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
        SelfSkeleton = skeletonAnimation.Skeleton;
        BaseColor = SelfSkeleton.GetColor();
    }

    private void Awake()
    {
        SelfSkeleton = skeletonAnimation.Skeleton;
#if !UNITY_EDITOR
        if (SelfSkeleton == null)
        {
            Destroy(this);
            return;
        } 
#endif
        
        SetBaseColorToCurrent();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (SelfSkeleton == null)
        {
            SetupSkeleton();
            if (SelfSkeleton == null) return;
        }
        if (editor_TestTargetColor) SelfSkeleton.SetColor(TargetColor);
        if (editor_TestBaseColor) SelfSkeleton.SetColor(BaseColor);
    } 
#endif

    private void SetupSkeleton()
    { 
        SelfSkeleton = skeletonAnimation.Skeleton;
    }

    private void EDITOR_SwitchToModifiedColor()
    {
        SelfSkeleton.SetColor(TargetColor); 
    }

    private void EDITOR_SwitchToBaseColor()
    {
        SelfSkeleton.SetColor(BaseColor);
    }

    public void SwitchToModifiedColor(float time = -1)
    {
        if (time < 0) SelfSkeleton.SetColor(TargetColor);
        else LeanTween.value(this.gameObject, SelfSkeleton.GetColor(), TargetColor, time).setOnUpdate(
        (Color val) =>
        {
            SelfSkeleton.SetColor(val);
        });
    }

    public void SwitchToBaseColor(float time = -1)
    {
        if (time < 0) SelfSkeleton.SetColor(BaseColor);
        else LeanTween.value(this.gameObject, SelfSkeleton.GetColor(), BaseColor, time).setOnUpdate(
        (Color val) =>
        {
            SelfSkeleton.SetColor(val);
        });
    }   

    public void SetBaseColorToCurrent()
    {
        BaseColor = SelfSkeleton.GetColor();
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        SelfSkeleton = skeletonAnimation.Skeleton;
    } 
#endif
}

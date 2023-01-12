#if UNITY_2018_3 || UNITY_2019 || UNITY_2018_3_OR_NEWER
#define NEW_PREFAB_SYSTEM
#endif

using System;
using UnityEngine;
using Spine.Unity;
using Spine;

    /// <summary>Sets a GameObject's transform to match a bone on a Spine skeleton.</summary>
#if NEW_PREFAB_SYSTEM
[ExecuteAlways]
#else
[ExecuteInEditMode]
#endif
[AddComponentMenu("Spine/BoneFollower")]
[HelpURL("http://esotericsoftware.com/spine-unity#BoneFollower")]
public class CustomBoneFollow : MonoBehaviour
{

    #region Inspector
    public SkeletonRenderer skeletonRenderer;
    public SkeletonRenderer SkeletonRenderer
    {
        get { return skeletonRenderer; }
        set
        {
            skeletonRenderer = value;
            Initialize();
        }
    }

    /// <summary>If a bone isn't set in code, boneName is used to find the bone at the beginning. For runtime switching by name, use SetBoneByName. You can also set the BoneFollower.bone field directly.</summary>
    [SpineBone(dataField: "skeletonRenderer")]
    public string boneName;

    public bool useOffset;
    public Vector3 offset;
    public bool forceZOffset;

    public bool followXPosition = true;
    public bool followYPosition = true;
    public bool followZPosition = true;
    public bool followBoneRotation = true;


    [Tooltip("Follows the skeleton's flip state by controlling this Transform's local scale.")]
    public bool followSkeletonFlip = true;

    [Tooltip("Follows the target bone's local scale. BoneFollower cannot inherit world/skewed scale because of UnityEngine.Transform property limitations.")]
    public bool followLocalScale = false;

    public enum AxisOrientation
    {
        XAxis = 1,
        YAxis
    }
    [Tooltip("Applies when 'Follow Skeleton Flip' is disabled but 'Follow Bone Rotation' is enabled."
        + " When flipping the skeleton by scaling its Transform, this follower's rotation is adjusted"
        + " instead of its scale to follow the bone orientation. When one of the axes is flipped, "
        + " only one axis can be followed, either the X or the Y axis, which is selected here.")]
    public AxisOrientation maintainedAxisOrientation = AxisOrientation.XAxis;

    [UnityEngine.Serialization.FormerlySerializedAs("resetOnAwake")]
    public bool initializeOnAwake = true;
    #endregion

    [NonSerialized] public bool valid;
    [NonSerialized] public Bone bone;

    Transform skeletonTransform;
    bool skeletonTransformIsParent;

    /// <summary>
    /// Sets the target bone by its bone name. Returns false if no bone was found. To set the bone by reference, use BoneFollower.bone directly.</summary>
    public bool SetBone(string name)
    {
        bone = skeletonRenderer.skeleton.FindBone(name);
        if (bone == null)
        {
            Debug.LogError("Bone not found: " + name, this);
            return false;
        }
        boneName = name;
        return true;
    }

    public void Awake()
    {
        if (initializeOnAwake) Initialize();
    }

    public void HandleRebuildRenderer(SkeletonRenderer skeletonRenderer)
    {
        Initialize();
    }

    public void Initialize()
    {
        bone = null;
        valid = skeletonRenderer != null && skeletonRenderer.valid;
        if (!valid) return;

        skeletonTransform = skeletonRenderer.transform;
        skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
        skeletonRenderer.OnRebuild += HandleRebuildRenderer;
        skeletonTransformIsParent = Transform.ReferenceEquals(skeletonTransform, transform.parent);

        if (!string.IsNullOrEmpty(boneName))
            bone = skeletonRenderer.skeleton.FindBone(boneName);

        if (useOffset && offset == Vector3.zero) offset = this.transform.localPosition;

#if UNITY_EDITOR
        if (Application.isEditor)
            LateUpdate();
#endif
    }

    void OnDestroy()
    {
        if (skeletonRenderer != null)
            skeletonRenderer.OnRebuild -= HandleRebuildRenderer;
    }

    public void LateUpdate()
    {
        if (!valid)
        {
            Initialize();
            return;
        }

#if UNITY_EDITOR
        if (!Application.isPlaying)
            skeletonTransformIsParent = Transform.ReferenceEquals(skeletonTransform, transform.parent);
#endif

        if (bone == null)
        {
            if (string.IsNullOrEmpty(boneName)) return;
            bone = skeletonRenderer.skeleton.FindBone(boneName);
            if (!SetBone(boneName)) return;
        }

        Transform thisTransform = this.transform;
         float additionalFlipScale = 1;
        if (skeletonTransformIsParent)
        {
            // Recommended setup: Use local transform properties if Spine GameObject is the immediate parent
            thisTransform.localPosition = new Vector3(followXPosition ? bone.WorldX : thisTransform.localPosition.x,
                                                    followYPosition ? bone.WorldY : thisTransform.localPosition.y,
                                                    followZPosition ? 0f : thisTransform.localPosition.z);
            if (followBoneRotation)
            {
                float halfRotation = Mathf.Atan2(bone.C, bone.A) * 0.5f;
                if (followLocalScale && bone.ScaleX < 0) // Negate rotation from negative scaleX. Don't use negative determinant. local scaleY doesn't factor into used rotation.
                    halfRotation += Mathf.PI * 0.5f;

                var q = default(Quaternion);
                q.z = Mathf.Sin(halfRotation);
                q.w = Mathf.Cos(halfRotation);
                thisTransform.localRotation = q;
            }
        }
        else
        {
            // For special cases: Use transform world properties if transform relationship is complicated
            Vector3 targetWorldPosition = skeletonTransform.TransformPoint(new Vector3(bone.WorldX, bone.WorldY, 0f));
                
            if (!followXPosition) targetWorldPosition.x = thisTransform.position.x;
            if (!followYPosition) targetWorldPosition.y = thisTransform.position.y;
            if (!followZPosition) targetWorldPosition.z = thisTransform.position.z;

            Vector3 skeletonLossyScale = skeletonTransform.lossyScale;
            Transform transformParent = thisTransform.parent;
            Vector3 parentLossyScale = transformParent != null ? transformParent.lossyScale : Vector3.one;
            if (followBoneRotation)
            {
                float boneWorldRotation = bone.WorldRotationX;

                if ((skeletonLossyScale.x * skeletonLossyScale.y) < 0)
                    boneWorldRotation = -boneWorldRotation;

                if (followSkeletonFlip || maintainedAxisOrientation == AxisOrientation.XAxis)
                {
                    if ((skeletonLossyScale.x * parentLossyScale.x < 0))
                        boneWorldRotation += 180f;
                }
                else
                {
                    if ((skeletonLossyScale.y * parentLossyScale.y < 0))
                        boneWorldRotation += 180f;
                }

                Vector3 worldRotation = skeletonTransform.rotation.eulerAngles;
                if (followLocalScale && bone.ScaleX < 0) boneWorldRotation += 180f;
                thisTransform.SetPositionAndRotation(targetWorldPosition, Quaternion.Euler(worldRotation.x, worldRotation.y, worldRotation.z + boneWorldRotation));
            }
            else
            {
                if (forceZOffset) targetWorldPosition.z = .1f;

                thisTransform.position = targetWorldPosition + (useOffset ? offset : Vector3.zero);
            }

            additionalFlipScale = Mathf.Sign(skeletonLossyScale.x * parentLossyScale.x
                                            * skeletonLossyScale.y * parentLossyScale.y);
        }

        Vector3 localScale = this.transform.localScale;
        if (followLocalScale) localScale = new Vector3(bone.ScaleX, bone.ScaleY, 1f);
        if (followSkeletonFlip) localScale.y *= Mathf.Sign(bone.Skeleton.ScaleX * bone.Skeleton.ScaleY) * additionalFlipScale;

        thisTransform.localScale = localScale;
    }
}

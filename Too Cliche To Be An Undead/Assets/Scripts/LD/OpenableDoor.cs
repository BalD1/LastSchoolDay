using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenableDoor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private bool isClosed = false;

    [SerializeField] private bool isVertical = true;

    [SerializeField] private Vector2 baseHeight;

    private void Start()
    {
        if (isClosed) return;

        Vector2 doorSize = spriteRenderer.size;
        doorSize.y = 0;
        spriteRenderer.size = doorSize;
    }

    public void Close(bool ignoreTimeScale = false)
    {
        if (isClosed) return;

        SetTweenDependingOnOrientation(false, ignoreTimeScale);

        isClosed = true;
    }

    public void Open(bool ignoreTimeScale = false)
    {
        if (!isClosed) return;

        SetTweenDependingOnOrientation(true, ignoreTimeScale);

        isClosed = false;
    }

    /// <summary>
    /// Opens or closes the door. Depending on the orientation (<see cref="isVertical"/>)."
    /// </summary>
    /// <param name="open"></param>
    private void SetTweenDependingOnOrientation(bool open, bool ignoreTimeScale)
    {
        float goal = 0;

        // sets the goal depending on the orientation
        if (!open) goal = isVertical ? baseHeight.y : baseHeight.x;

        LeanTween.value(from: isVertical ? spriteRenderer.size.y : spriteRenderer.size.x,
                        to: goal, 
                        time: .5f)
        .setOnUpdate((float val) =>
        {
            Vector2 size = spriteRenderer.size;

            if (isVertical) size.y = val;
            else size.x = val;

            spriteRenderer.size = size;
        }).setIgnoreTimeScale(ignoreTimeScale);
    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        spriteRenderer ??= this.GetComponent<SpriteRenderer>();
        baseHeight = spriteRenderer.size; 
#endif
    }
}

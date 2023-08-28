using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [InspectorButton("PlayAnim", ButtonWidth = 200)]
    [SerializeField] private bool playAnim;
    [SerializeField] private string animName;

    private void PlayAnim()
    {
        animator.Play(animName);
    }
}

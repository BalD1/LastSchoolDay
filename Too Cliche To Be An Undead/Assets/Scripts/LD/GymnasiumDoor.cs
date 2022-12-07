using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymnasiumDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator animator;

    [SerializeField] private GameObject keycardHolderPF;

    [SerializeField] [ReadOnly] private List<GameObject> currentInteractors = new List<GameObject>();

    [SerializeField] private float keycardsRadius = 2;

    private GameObject[] keycardsHolders;
    private Vector2[] holderPosition;

    [SerializeField] private LeanTweenType inType = LeanTweenType.easeInSine;
    [SerializeField] private LeanTweenType outType = LeanTweenType.easeOutSine;

    [SerializeField] private float tweenTime = .5f;

    private bool canBeInteracted = false;

    private bool tweeningIn = false;
    private bool tweeningOut = false;

    private bool isTweeningKeycard = false;

    private bool needsToTweenOut = false;

    private int keycardsOfferedToDoor = 0;
    private int youngestActiveHolderIdx = 0;

    private void Start()
    {
        GameManager.Instance._onRunStarted += InstantiateKeycardHolders;
    }

    private void InstantiateKeycardHolders()
    {
        int neededCards = GameManager.NeededCards;
        keycardsHolders = new GameObject[neededCards];
        holderPosition = new Vector2[neededCards];

        neededCards++;

        for (int i = 1; i < neededCards; i++)
        {
            GameObject gO = Instantiate(keycardHolderPF, this.transform);

            var angle = i * Mathf.PI * keycardsRadius * .5f / neededCards;
            var AngleOffset = Mathf.Atan2(angle, angle) * Mathf.Deg2Rad;
            angle += AngleOffset;
            var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * keycardsRadius;

            gO.transform.localScale = Vector3.zero;

            holderPosition[i - 1] = pos + this.transform.position;

            keycardsHolders[i - 1] = gO;
        }

        canBeInteracted = true;
    }

    public bool CanBeInteractedWith()
    {
        return canBeInteracted;
    }

    public void EnteredInRange(GameObject interactor)
    {
        currentInteractors.Add(interactor);
        if (currentInteractors.Count > 1 || tweeningIn) return;

        needsToTweenOut = false;

        LeanTween.cancel(this.gameObject);
        EaseInNext(0);
    }

    private void EaseInNext(int i)
    {
        if (i >= keycardsHolders.Length)
        {
            tweeningIn = false;

            return;
        }

        if (tweeningOut) return;

        tweeningIn = true;
        LeanTween.move(keycardsHolders[i], holderPosition[i], tweenTime).setEase(inType);
        LeanTween.scale(keycardsHolders[i], Vector3.one, tweenTime).setOnComplete(
        () =>
        {
            if (currentInteractors.Count <= 0 && !tweeningIn) EaseOutNext(0);
        });
        LeanTween.delayedCall(.1f,
        () =>
        {
            EaseInNext(i + 1);
        });

    }

    public void ExitedRange(GameObject interactor)
    {
        currentInteractors.Remove(interactor);
        if (currentInteractors.Count > 0 || tweeningOut) return;

        if (isTweeningKeycard) needsToTweenOut = true;

        LeanTween.cancel(this.gameObject);
        EaseOutNext(0);
    }

    private void EaseOutNext(int i)
    {
        if (isTweeningKeycard) return;

        if (i >= keycardsHolders.Length)
        {
            tweeningOut = false;

            return;
        }

        if (tweeningIn) return;

        tweeningOut = true;
        LeanTween.move(keycardsHolders[i], this.transform.position, tweenTime).setEase(outType);
        LeanTween.scale(keycardsHolders[i], Vector3.zero, tweenTime).setOnComplete(
        () =>
        {
            if (currentInteractors.Count > 0 && !tweeningOut) EaseInNext(0);
        });
        LeanTween.delayedCall(.1f,
        () =>
        {
            EaseOutNext(i + 1);
        });

    }

    public void Interact(GameObject interactor)
    {
        int keysToOffer = GameManager.AcquiredCards - keycardsOfferedToDoor;

        if (keysToOffer <= 0) return;

        isTweeningKeycard = true;

        keycardsOfferedToDoor += keysToOffer;

        TweenSingleHolder(keysToOffer);
    }

    private void TweenSingleHolder(int count)
    {
        if (count <= 0)
        {
            TryOpen();
            isTweeningKeycard = false;

            if (needsToTweenOut)
            {
                EaseOutNext(0);
                needsToTweenOut = false;
            }
            return;
        }

        int reversedIdx = keycardsHolders.Length - youngestActiveHolderIdx - 1;

        GameObject target = keycardsHolders[reversedIdx];
        target.LeanScale(Vector3.one * 1.2f, .2f).setOnComplete(() =>
        {
            LeanTween.color(target.transform.GetChild(0).gameObject, Color.white, .2f);
            target.LeanScale(Vector3.one, .2f).setOnComplete(() =>
            {
                youngestActiveHolderIdx++;
                TweenSingleHolder(count - 1);
            });
        });
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void TryOpen()
    {
        if (keycardsOfferedToDoor >= GameManager.NeededCards)
        {
            canBeInteracted = false;
            animator.SetTrigger("Open");

            foreach (var item in keycardsHolders)
            {
                item.SetActive(false);
            }
        }
    }
}

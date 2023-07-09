using Spine.Unity;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviourEventsHandler, IInteractable
{
    [SerializeField] private ShopScreen shopUIPF;

    [SerializeField] private SkeletonAnimation skeleton;
    [SerializeField] private GameObject outline;

    [field: SerializeField] public CircleCollider2D InteractionTrigger { get; private set; }

    [SerializeField] private AudioSource source;
    [field: SerializeField] public SCRPT_ShopAudio ShopAudioData { get; private set; }

    [SerializeField] [SpineAnimation] private string idle, open;

    [SerializeField] private float animationDelay = .35f;

    private List<GameObject> currentInteractors = new List<GameObject>();

    protected override void EventsSubscriber()
    {
        ShopScreenEvents.OnCloseUI += OnShopClose;
    }

    protected override void EventsUnSubscriber()
    {
        ShopScreenEvents.OnCloseUI -= OnShopClose;
    }

    protected override void Awake()
    {
        shopUIPF.Create(UIManager.GetMainCanvas.transform).Setup(this);
        base.Awake();
        this.SetCurrentShop();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        this.UnsetCurrentShop();
    }

    public void EnteredInRange(GameObject interactor)
    {
        outline.SetActive(true);
        currentInteractors.Add(interactor);
    }

    public void ExitedRange(GameObject interactor)
    {
        currentInteractors.Remove(interactor);
        if (currentInteractors.Count > 0) return;
        outline.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        OpenShop();
        outline.SetActive(false);
    }

    public float GetDistanceFrom(Transform target) => Vector2.Distance(this.transform.position, target.position);

    public void OpenShop()
    {
        if (skeleton.AnimationName != idle) return;

        this.OpenedShop();
        skeleton.AnimationState.SetAnimation(0, open, true);
        PlayAudio(ShopAudioData.openShopAudio);
        LeanTween.delayedCall(animationDelay, OnAnimEnded);
    }
    private void OnAnimEnded()
    {
        ShopEvents.ShopAnimEnded(this);
    }

    private void OnShopClose()
    {
        PlayAudio(ShopAudioData.closeShopAudio);
        this.ClosedShop();
        LeanTween.delayedCall(animationDelay, DelayedIdle);
    }

    private void DelayedIdle()
    {
        skeleton.AnimationState.SetAnimation(0, idle, true);

        if (currentInteractors.Count > 0) outline.SetActive(true);
    }

    public bool CanBeInteractedWith()
    {
        return true;
    }

    public void PlayAudio(SCRPT_EntityAudio.S_AudioClips audioData)
    {
        if (audioData.clip == null) return;

        source.pitch = Random.Range(1 - audioData.pitchRange, 1 + audioData.pitchRange);
        source.PlayOneShot(audioData.clip);
    }
}

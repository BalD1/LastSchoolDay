using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityWatcher : MonoBehaviour
{
    [SerializeField] private PlayerCharacter owner;

    [SerializeField] private SpriteRenderer markerSprite;
    [SerializeField] private Transform markerTransform;

    [SerializeField] private float maxDistance;
    [SerializeField] private float scaleMultiplier;

    [SerializeField] private SerializedDictionary<GameManager.E_CharactersNames, Sprite> charactersMarkerSprite;

    [field: SerializeField, ReadOnly] public bool IsVisible { get; private set; } = true;
    private bool checkVisibility;

    private Camera targetCamera;

    private void Awake()
    {
        targetCamera = Camera.main;
        if (owner == null) return;
        markerSprite.sprite = charactersMarkerSprite[owner.GetCharacterName()];
    }

    public void Setup(PlayerCharacter _owner)
    {
        this.owner = _owner;
        markerSprite.sprite = charactersMarkerSprite[_owner.GetCharacterName()];
    }

    private void Update()
    {
        if (!checkVisibility) return;
        if (CameraManager.ST_InstanceExists() && CameraManager.Instance.CinematicMode) return;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(targetCamera);
        IsVisible = GeometryUtility.TestPlanesAABB(planes, owner.BodyTrigger.bounds);

        markerSprite.enabled = !IsVisible;
        if (IsVisible) return;

        Vector2 minScreenBounds = targetCamera.ScreenToWorldPoint(new Vector3(0, 0));
        Vector2 maxScreenBounds = targetCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));

        markerTransform.position = new Vector2(Mathf.Clamp(markerTransform.position.x, minScreenBounds.x + .5f, maxScreenBounds.x - .5f),
                                               Mathf.Clamp(markerTransform.position.y, minScreenBounds.y + .5f, maxScreenBounds.y - .5f));

        float dist = Vector2.Distance(markerTransform.position, markerTransform.position);
        float markerScale = Mathf.Clamp01(1 - (dist / maxDistance));

        Vector2 v = markerTransform.transform.localScale;
        v.x = markerScale * scaleMultiplier;
        v.y = markerScale * scaleMultiplier;
        markerTransform.localScale = v;

        if (dist > maxDistance)
        {
            Vector2 newPos = IGPlayersManager.Instance.PlayersList[0].transform.position;
            IGPlayersManager.Instance.TeleportPlayer(owner.PlayerIndex, newPos);
        }
    }
}

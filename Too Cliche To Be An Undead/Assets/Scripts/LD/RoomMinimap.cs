using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMinimap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer minimapSprite_room;
    [SerializeField] private SpriteRenderer minimapSprite_entry;

    [SerializeField] private ElementSpawner roomKeycardSpawner;

    private bool hasKeycard;

    private void Awake()
    {
        roomKeycardSpawner.D_spawnedKeycard += OnRoomSpawnedCard;
    }

    private void OnRoomSpawnedCard(Keycard key)
    {
        hasKeycard = true;
        key.D_onPickup += ValidateRoom;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasKeycard) return;

        if (collision.GetComponent<PlayerCharacter>() == null) return;

        ValidateRoom();
    }

    private void ValidateRoom()
    {
        minimapSprite_room.LeanAlpha(1, .5f);
        minimapSprite_entry.LeanAlpha(1, .5f);
        Destroy(this, 1f);
    }
}

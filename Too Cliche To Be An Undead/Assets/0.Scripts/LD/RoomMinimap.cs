using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMinimap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer minimapSprite_room;
    [SerializeField] private SpriteRenderer minimapSprite_entry;

    [SerializeField] private ElementSpawner roomKeycardSpawner;

    private Keycard key;

    [SerializeField] private SCRPT_RoomMinimap roomMinimap;

    private bool hasKeycard;

    private void Awake()
    {
        roomKeycardSpawner.D_spawnedKeycard += OnRoomSpawnedCard;

        minimapSprite_room.color = roomMinimap.UndiscoveredColor;
        minimapSprite_entry.color = roomMinimap.UndiscoveredColor;
    }

    private void OnRoomSpawnedCard(Keycard _key)
    {
        key = _key;
        hasKeycard = true;
        key.onPickup += ValidateRoom;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasKeycard)
        {
            key.SetMinimapState(true);
            return;
        }

        if (collision.GetComponent<PlayerCharacter>() == null) return;

        ValidateRoom();
    }

    private void ValidateRoom()
    {
        minimapSprite_room.LeanColor(roomMinimap.DiscoveredColor, roomMinimap.DiscoveryColorBlendSpeed);
        minimapSprite_entry.LeanColor(roomMinimap.DiscoveredColor, roomMinimap.DiscoveryColorBlendSpeed);
        Destroy(this, 1f);
    }
}

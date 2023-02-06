using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AreaTransitorManager : MonoBehaviour
{
    private static AreaTransitorManager instance;
    public static AreaTransitorManager Instance
    {
        get
        {
            return instance;
        }
    }

    private static int playersInCorridorCount;
    public static int PlayersInCorridorCount
    {
        get => playersInCorridorCount;
        set
        {
            if (playersInCorridorCount <= 0 && value > 0) instance?.SetCorridorHiddenState(false);
            
            playersInCorridorCount = value;

            if (playersInCorridorCount <= 0) instance?.SetCorridorHiddenState(true);
        }
    }

    [SerializeField] private Tilemap corridorHidder;

    public const float fadeTime = .15f;

    [field: SerializeField] public Color hiddenColor { get; private set; }
    [field: SerializeField] public Color transparentColor { get; private set; }

    private int corridorTweenID;

    private void Awake()
    {
        instance = this;
    }

    public void SetCorridorHiddenState(bool hidden)
    {
        LeanTween.cancel(corridorTweenID);

        corridorTweenID = LeanTween.value(corridorHidder.gameObject, corridorHidder.color, hidden ? hiddenColor : transparentColor, fadeTime).setOnUpdate(
        (Color val) =>
        {
            corridorHidder.color = val;
        }).id;
    }
}

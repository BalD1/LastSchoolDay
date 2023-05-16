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

    [field: SerializeField] public int playersInCorridorCount { get; private set; }

    [SerializeField] private Tilemap corridorHidder;
    public Tilemap CorridorHidder { get => corridorHidder; }

    public const float fadeTime = .15f;

    [field: SerializeField] public Color hiddenColor { get; private set; }
    [field: SerializeField] public Color transparentColor { get; private set; }

    private int corridorTweenID;

    private void Awake()
    {
        instance = this;
    }

    public void PlayerEnteredCorridor()
    {
        playersInCorridorCount++;
    }
    public void PlayerExitedCorridor()
    {
        playersInCorridorCount--;
    }

    public void AskForCorridorAlphaChange(float newAlpha)
    {
        if (playersInCorridorCount > 0) newAlpha = 0;

        Color corridorColor = CorridorHidder.color;
        corridorColor.a = newAlpha;
        CorridorHidder.color = corridorColor;
    }

    public void ForceHideCorridor()
    {
        Color corridorColor = CorridorHidder.color;
        corridorColor.a = 1;
        CorridorHidder.color = corridorColor;
    }
}
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
            playersInCorridorCount = value;
        }
    }

#if UNITY_EDITOR
    [SerializeField]
    [ReadOnly] private int EDITOR_playersInCorridor; 
#endif

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

    private void Update()
    {
#if UNITY_EDITOR
        EDITOR_playersInCorridor = playersInCorridorCount;
#endif
    }

    public void AskForCorridorAlphaChange(float newAlpha)
    {
        if (playersInCorridorCount > 0) newAlpha = 0;

        Color corridorColor = CorridorHidder.color;
        corridorColor.a = newAlpha;
        CorridorHidder.color = corridorColor;
    }
}

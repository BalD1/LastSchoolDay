using System.Collections.Generic;
using UnityEngine;

public class IGPlayersManager : Singleton<IGPlayersManager>
{
    [SerializeField] private PlayerCharacter playerPF;

    [field: SerializeField] public List<PlayerCharacter> PlayersList { get; private set; } = new List<PlayerCharacter>();

    private void Start()
    {
        foreach (var item in PlayerInputsManager.Instance.PlayerInputsList)
        {
            //playerPF?.Create();
        }
    }
}

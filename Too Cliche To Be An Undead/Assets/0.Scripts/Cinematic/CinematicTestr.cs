using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTestr : MonoBehaviour
{
    [SerializeField] private List<Vector2> posTests = new List<Vector2>();
    [SerializeField] private List<Vector2> posTests2 = new List<Vector2>();

    private Cinematic c;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CinematicPlayersMove cpm = new CinematicPlayersMove();
            cpm.Setup(IGPlayersManager.Instance.PlayersList, posTests);

            CinematicPlayersMove cpm2 = new CinematicPlayersMove();
            cpm2.Setup(IGPlayersManager.Instance.PlayersList, posTests2);

            c = new Cinematic(cpm, cpm2);
            c.StartCinematic();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTestr : MonoBehaviour
{
    [SerializeField] private List<Vector2> posTests = new List<Vector2>();
    [SerializeField] private List<Vector2> posTests2 = new List<Vector2>();

    [SerializeField] private SCRPT_SingleDialogue dialogue;

    private Cinematic c;

    private void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            CA_CinematicPlayersMove cpm = new CA_CinematicPlayersMove(posTests, true);
            CA_CinematicPlayersMove cpm2 = new CA_CinematicPlayersMove(posTests2, true);

            CA_CinematicDialoguePlayer cdp = new CA_CinematicDialoguePlayer(dialogue);

            c = new Cinematic(cpm, cdp, cpm2);
            c.SetPlayers(IGPlayersManager.Instance.PlayersList);
            c.StartCinematic();
        }
    }
}

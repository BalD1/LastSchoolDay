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
            CinematicPlayersMove cpm = new CinematicPlayersMove(posTests, true);
            CinematicPlayersMove cpm2 = new CinematicPlayersMove(posTests2, true);

            CinematicDialoguePlayer cdp = new CinematicDialoguePlayer(dialogue);

            c = new Cinematic(cpm, cdp, cpm2);
            c.SetPlayers(IGPlayersManager.Instance.PlayersList);
            c.StartCinematic();
        }
    }
}

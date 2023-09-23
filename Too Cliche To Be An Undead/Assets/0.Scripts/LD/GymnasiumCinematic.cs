using System;
using System.Collections;
using UnityEngine;

public class GymnasiumCinematic : MonoBehaviour
{
    [SerializeField] private SCRPT_SingleDialogue entryDialogue;
    [SerializeField] private SCRPT_SingleDialogue bossHeardDialogue;
    [SerializeField] private SCRPT_SingleDialogue bossSpawnDialogue;
    [SerializeField] private EndCinematic afterBossDeathCinematic;
    [SerializeField] private SpineGymnasiumDoor door;

    [SerializeField] private Transform doorTarget;
    [SerializeField] private Transform cameraShowBossTarget;
    [SerializeField] private Transform bossSpawnTarget;

    [SerializeField] private Transform playersTeleportPosition;

    [SerializeField] private BossZombie bossPF;
    private BossZombie createdBoss;

    private Cinematic doorOpenCinematic;
    private Cinematic afterBossFallCinematic;

    private void Start()
    {
        doorOpenCinematic = new Cinematic(
                new CA_CinematicScreenFade(_fadeIn: false, .5f),
                new CA_CinematicChangeMusicState(CA_CinematicChangeMusicState.E_State.Stop, false),
                new CA_CinematicCustomAction(() => this.GymnasiumCinematicStarted()),
                new CA_CinematicScreenFade(_fadeIn: true, .5f),
                new CA_CinematicDialoguePlayer(entryDialogue),
                new CA_CinematicCameraMove(doorTarget.position, 1.5f).SetLeanType(LeanTweenType.easeInOutQuad),
                new CA_CinematicDialoguePlayer(bossHeardDialogue),
                new CA_CinematicCameraMove(cameraShowBossTarget.position, 1.5f).SetLeanType(LeanTweenType.easeInOutQuad),
                new CA_CinematicCustomAction(() =>
                {
                    createdBoss = bossPF?.Create(bossSpawnTarget);
                    createdBoss.Setup(withCinematic: true);
                    createdBoss.OnAppearAnimationEnded += () => afterBossFallCinematic.StartCinematic();
                    afterBossDeathCinematic.SetBoss(createdBoss);
                })
            ).SetPlayers(IGPlayersManager.Instance.PlayersList).AllowChangeCinematicStateAtEnd(false);

        afterBossFallCinematic = new Cinematic( 
                new CA_CinematicDialoguePlayer(bossSpawnDialogue),
                new CA_CinematicScreenFade(_fadeIn: false, .5f),
                new CA_CinematicCameraZoom(-1, 0),
                new CA_CinematicPlayersMove(playersTeleportPosition.position, true, true),
                new CA_CinematicCameraMove(IGPlayersManager.Instance.PlayersList[0].transform.position, _teleport: true),
                new CA_CinematicCustomAction(() => AreaTransitorManager.Instance.ForceHideCorridor()),
                new CA_CinematicScreenFade(_fadeIn: true, .5f)
            ).SetPlayers(IGPlayersManager.Instance.PlayersList).AllowChangeCinematicStateAtStart(false);

    }

    public void Begin()
    {
        afterBossFallCinematic.OnCinematicEnded += door.CloseDoor;
        doorOpenCinematic.StartCinematic();
    }
}

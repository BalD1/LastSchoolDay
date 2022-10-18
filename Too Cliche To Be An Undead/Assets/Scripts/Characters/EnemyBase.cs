using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class EnemyBase : Entity
{
    [SerializeField] private SCRPT_DropTable dropTable;

    [SerializeField] private List<PlayerCharacter> detectedPlayers;

    [SerializeField] private PlayerCharacter currentPlayerTarget;
    [SerializeField] private Transform currentTransformTarget;

    [SerializeField] private float distanceBeforeStop = 1f;
    public float DistanceBeforeStop { get => distanceBeforeStop; }

    public PlayerCharacter CurrentPlayerTarget { get => currentPlayerTarget; }
    public Transform CurrentTransformTarget { get => currentPlayerTarget == null ? currentTransformTarget : currentPlayerTarget.transform; }

    public List<PlayerCharacter> DetectedPlayers { get => detectedPlayers; }
    public Vector2 lastSeenPosition;

    public delegate void D_DetectedPlayer();
    public D_DetectedPlayer D_detectedPlayer;

    public delegate void D_LostPlayer();
    public D_LostPlayer D_lostPlayer;
    
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public void AddDetectedPlayer(PlayerCharacter newDetection)
    {
        if (detectedPlayers.Count == 0) SetTarget(newDetection);

        detectedPlayers.Add(newDetection);
        D_detectedPlayer?.Invoke();
    }

    public void RemoveDetectedPlayer(PlayerCharacter player)
    {
        detectedPlayers.Remove(player);

        if (detectedPlayers.Count > 0) SetTarget(detectedPlayers.Last());
        else ResetTarget();

        lastSeenPosition = player.transform.position;
        D_lostPlayer?.Invoke();
    }

    private void SetTarget(Transform target)
    {
        currentTransformTarget = target;
    }
    private void SetTarget(PlayerCharacter playerTarget)
    {
        currentPlayerTarget = playerTarget;
        currentTransformTarget = playerTarget.transform;
    }
    private void ResetTarget()
    {
        currentPlayerTarget = null;
        currentTransformTarget = null;
    }

    public override void OnDeath(bool forceDeath = false)
    {
        base.OnDeath(forceDeath);

        if (dropTable != null)
        {
            dropTable.DropRandom(this.transform.position);
        }
    }
}

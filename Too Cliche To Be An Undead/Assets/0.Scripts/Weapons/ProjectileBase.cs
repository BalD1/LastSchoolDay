using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D body;

    [SerializeField] private float force;

    [SerializeField] private bool pierceEffect;

    [SerializeField] private int maxPierceCount;
    private int pierceCount;

    [SerializeField] private TrailRenderer trail;

    [SerializeField] private float maxLifetime = 5;

    [SerializeField] private AudioSource source;

    [SerializeField] private SCRPT_EntityAudio.S_AudioClips entityHitAudioData;
    [SerializeField] private SCRPT_EntityAudio.S_AudioClips objectHitAudioData;

    [SerializeField] private GameObject hitFX;

    private static MonoPool<ProjectileBase> pool;

    private float currentLifetime;

    public SCRPT_EntityStats.E_Team Team { get; private set; }

    private Entity owner;

    private float damages;
    private int critChances;

    public static void CheckPool()
    {
        if (pool == null)
            pool = new MonoPool<ProjectileBase>
                (_createAction: () => GameAssets.Instance.BaseProjectilePF.Create(Vector2.zero).GetComponent<ProjectileBase>(),
                _parentContainer: GameManager.Instance.InstantiatedMiscParent);
    }
    public static ProjectileBase GetProjectile(Vector2 position, Quaternion rotation)
    {
        return pool.GetNext(position, rotation);
    }

    private void Update()
    {
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0) Deactivate();
    }

    public void Fire(Vector2 direction, float _damages, int _critChances, Entity _owner)
    {
        currentLifetime = maxLifetime;
        pierceCount = maxPierceCount;

        this.body.velocity = direction * force;
        damages = _damages;
        critChances = _critChances;

        owner = _owner;
        Team = owner.GetStats.Team;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();

        Vector3 selfPos = this.transform.position;
        Vector3 otherPos = collision.transform.position;
        float dist = Vector2.Distance(selfPos, collision.gameObject.transform.position) / 2;
        Vector2 dir = (otherPos - selfPos).normalized;
        hitFX.Create(selfPos + (Vector3)(dist * dir));
        if (damageable == null)
        {
            ProjectileBase proj = collision.GetComponent<ProjectileBase>();
            if (proj != null)
                if (proj.Team.Equals(this.Team)) return;

            CheckDestroySelf();

            return;
        }

        bool isCrit = Random.Range(0, 100) <= critChances;

        if (damageable.OnTakeDamages(damages, owner, isCrit))
        {
            CheckDestroySelf();

            if (collision.GetComponentInParent<NormalZombie>() != null) PlayAudio(entityHitAudioData);
            else PlayAudio(objectHitAudioData);
        }
    }

    public void HitWall(Vector2 wallPosition)
    {
        hitFX.Create(wallPosition).transform.parent = GameManager.Instance.InstantiatedMiscParent;
        PlayAudio(objectHitAudioData);
        Deactivate();
    }

    private void PlayAudio(SCRPT_EntityAudio.S_AudioClips audioData)
    {
        AudioclipPlayer.Create(this.transform.position, audioData);
    }

    private void CheckDestroySelf()
    {
        if (!pierceEffect || pierceCount <= 0)
        {
            Deactivate();
            return;
        }

        pierceCount--;
    }

    private void Deactivate()
    {
        trail.Clear();
        pool.Enqueue(this);
    }
}

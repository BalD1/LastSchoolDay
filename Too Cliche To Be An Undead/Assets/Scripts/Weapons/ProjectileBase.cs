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

    private static Queue<ProjectileBase> ProjectilesPool;

    public static ProjectileBase GetProjectile(Vector2 position, Quaternion rotation)
    {
        if (ProjectilesPool == null) ProjectilesPool = new Queue<ProjectileBase>();

        ProjectileBase proj;

        if (ProjectilesPool.Count > 0)
        {
            proj = ProjectilesPool.Dequeue();

            GameObject projObj = proj.gameObject;
            projObj.SetActive(true);
            projObj.transform.SetPositionAndRotation(position, rotation);

            return proj;
        }

        proj = Instantiate(GameAssets.Instance.BaseProjectilePF, position, rotation).GetComponent<ProjectileBase>();
        proj.transform.SetParent(GameManager.Instance.InstantiatedProjectilesParent, true);
        return proj;
    }

    private float currentLifetime;

    public SCRPT_EntityStats.E_Team Team { get; private set; }

    private Entity owner;

    private float damages;
    private int critChances;

    private void Awake()
    {
        ProjectilesPool = new Queue<ProjectileBase>();
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

        if (damageable == null)
        {
            ProjectileBase proj = collision.GetComponent<ProjectileBase>();
            if (proj != null)
                if (proj.Team.Equals(this.Team)) return;

            if (collision.CompareTag("Wall"))
            {
                PlayAudio(objectHitAudioData);
                Deactivate();
            }
            else CheckDestroySelf();

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
        ProjectilesPool.Enqueue(this);
        this.gameObject.SetActive(false);
    }
}

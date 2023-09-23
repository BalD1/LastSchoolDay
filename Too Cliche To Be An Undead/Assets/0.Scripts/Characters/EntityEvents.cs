using System;

public static class EntityEvents
{
    public class OnEntityDamagesData<T>
    {
        public T DamagedEntity { get; private set; }
        public Entity Damager { get; private set;}
        public float TakenDamages { get; private set;}

        public OnEntityDamagesData(T damagedEntity, Entity damager, float takenDamages)
        {
            DamagedEntity = damagedEntity;
            Damager = damager;
            TakenDamages = takenDamages;
        }

        public void SetDamager(Entity damager)
            => Damager = damager;
        public void SetTakenDamages(float takenDamages)
            => TakenDamages = takenDamages;

        public void SetDamagerAndDamagesAmount(Entity damager, float takenDamages)
        {
            SetDamager(damager);
            SetTakenDamages(takenDamages);
        }
    }

    public static event Action<OnEntityDamagesData<EnemyBase>> OnEnemyTookDamages;
    public static void EnemyTookDamages(this EnemyBase enemy, OnEntityDamagesData<EnemyBase> data)
        => OnEnemyTookDamages?.Invoke(data);

    public static event Action<OnEntityDamagesData<EnemyBase>> OnEnemyDeath;
    public static void EnemyDeath(this EnemyBase enemy, OnEntityDamagesData<EnemyBase> data)
        => OnEnemyDeath?.Invoke(data);

    public static event Action<OnEntityDamagesData<PlayerCharacter>> OnPlayerTookDamages;
    public static void PlayerTookDamages(this PlayerCharacter enemy, OnEntityDamagesData<PlayerCharacter> data)
        => OnPlayerTookDamages?.Invoke(data);

    public static event Action<OnEntityDamagesData<PlayerCharacter>> OnPlayerDeath;
    public static void PlayerDeath(this PlayerCharacter enemy, OnEntityDamagesData<PlayerCharacter> data)
        => OnPlayerDeath?.Invoke(data);
}

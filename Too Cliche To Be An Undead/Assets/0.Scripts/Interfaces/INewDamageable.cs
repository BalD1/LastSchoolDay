
using System;

public interface INewDamageable
{
    public abstract void UpdateMaxHealth(float newMaxHealth, bool healDifference);
    public abstract bool TryInflictDamages(DamagesData damagesData);
    public abstract void InflictDamages(DamagesData damagesData);
    public abstract void Heal(float amount, bool isCrit);
    public abstract void Kill();
    public bool IsAlive();

    public class DamagesData : EventArgs
    {
        public Entity Origin {  get; private set; }
        public SO_BaseStats.E_Team DamagerTeam { get; private set; }
        public E_DamagesType DamagesType { get; private set; }
        public bool IsCrit { get; private set; }
        public float Damages { get; private set; }

        public DamagesData(SO_BaseStats.E_Team damagerTeam, E_DamagesType damagesType, float damages, bool isCrit, Entity origin)
        {
            this.Origin = origin;
            this.DamagerTeam = damagerTeam;
            this.DamagesType = damagesType;
            this.IsCrit = isCrit;
            this.Damages = damages;
        }

        public void SetIsCrit(bool isCrit)
            => IsCrit = isCrit;
    }

    public enum E_DamagesType
    {
        Unknown,
        Blunt,
        Sharp,
        Flames,
    }
}

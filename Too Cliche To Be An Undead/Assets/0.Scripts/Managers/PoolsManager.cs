using UnityEngine;

public class PoolsManager : Singleton<PoolsManager>
{
	protected override void Awake()
	{
		base.Awake();
		BloodParticles.CheckPool();
		ProjectileBase.CheckPool();
		//SpawnersManager.CheckPool();
		TextPopup.CheckPool();
		HealthPopup.CheckPool();
		DashHitParticles.CheckPool();
		BossHitFX.CheckPool();
	}
}
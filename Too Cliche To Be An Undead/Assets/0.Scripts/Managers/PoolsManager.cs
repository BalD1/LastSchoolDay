using UnityEngine;

public class PoolsManager : MonoBehaviour
{
	private static PoolsManager instance;
	public static PoolsManager Instance
	{
		get
		{
			if (instance == null) Debug.LogError("PoolsManager instance could not be found.");

			return instance;
		}
	}
	
	private void Awake()
	{
		instance = this;

		BloodParticles.CheckPool();
		ProjectileBase.CheckPool();
		SpawnersManager.CheckPool();
		TextPopup.CheckPool();
		HealthPopup.CheckPool();
	}
}
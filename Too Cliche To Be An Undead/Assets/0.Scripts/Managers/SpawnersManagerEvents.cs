using System;

public static class SpawnersManagerEvents
{
	public static event Action OnSpawnedKeycards;
	public static void SpawnedKeycards(this SpawnersManager manager)
		=> OnSpawnedKeycards?.Invoke();

	public static event Action<Keycard> OnSpawnedKeycardSingle;
	public static void SpawnedKeycardSingle(this SpawnersManager manager, Keycard spawnedCard)
		=> OnSpawnedKeycardSingle?.Invoke(spawnedCard);

	public static event Action<Keycard> OnPickedupCard;
	public static void PickedupCard(this SpawnersManager manager, Keycard pickedCard)
		=> OnPickedupCard?.Invoke(pickedCard);
}
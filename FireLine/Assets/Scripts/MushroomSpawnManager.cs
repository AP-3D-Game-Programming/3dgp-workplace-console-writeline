using UnityEngine;
using System.Collections.Generic;

public class MushroomSpawnManager : MonoBehaviour
{
	[SerializeField] private GameObject[] mushroomPrefabs;
	[SerializeField] private int numberOfMushrooms = 12;
	[SerializeField] private float spawnRadius = 20f;
	[SerializeField] private Transform spawnCenter;
	[SerializeField] private float spawnHeight = 50f;
	[SerializeField] private LayerMask groundLayer;

	private List<GameObject> spawnedMushrooms = new List<GameObject>();
	private int mushroomsCollected = 0;
	private int mushroomsSpawned = 0;

	void OnEnable()
	{
		// Subscribe to mushroom collection events if needed
	}

	void OnDisable()
	{
		// Unsubscribe from events if needed
	}

	public void SpawnMushrooms()
	{
		Debug.Log("SpawnMushrooms() called!");

		if (mushroomPrefabs == null || mushroomPrefabs.Length == 0)
		{
			Debug.LogError("No mushroom prefabs assigned in Inspector!");
			return;
		}

		ClearMushrooms();
		mushroomsCollected = 0;
		mushroomsSpawned = 0;

		for (int i = 0; i < numberOfMushrooms; i++)
		{
			Vector3 randomPos = GetRandomGroundPosition();

			if (randomPos != Vector3.zero)
			{
				int randomIndex = Random.Range(0, mushroomPrefabs.Length);
				Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

				GameObject mushroom = Instantiate(mushroomPrefabs[randomIndex], randomPos, randomRotation);
				spawnedMushrooms.Add(mushroom);
				mushroomsSpawned++;
			}
		}

		Debug.Log($"Spawned {mushroomsSpawned} mushrooms");
	}

	private Vector3 GetRandomGroundPosition()
	{
		int maxAttempts = 10;

		for (int attempt = 0; attempt < maxAttempts; attempt++)
		{
			Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
			randomOffset.y = 0;

			Vector3 spawnPos = spawnCenter.position + randomOffset;
			spawnPos.y = spawnCenter.position.y + spawnHeight;

			RaycastHit hit;
			if (Physics.Raycast(spawnPos, Vector3.down, out hit, spawnHeight * 2, groundLayer))
			{
				return hit.point;
			}
		}

		Debug.LogWarning("Could not find valid ground position for mushroom");
		return Vector3.zero;
	}

	public void ClearMushrooms()
	{
		foreach (GameObject mushroom in spawnedMushrooms)
		{
			if (mushroom != null)
				Destroy(mushroom);
		}
		spawnedMushrooms.Clear();
	}

	void OnDrawGizmosSelected()
	{
		if (spawnCenter != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
		}
	}
}

using System.Collections.Generic;
using UnityEngine;

public class MushroomSpawnManager : MonoBehaviour
{
	public GameObject[] mushroomPrefabs; // Assign different mushroom variants
	public Transform player;
	public int numberOfMushrooms = 12;
	public Vector2 spawnAreaSize = new Vector2(50f, 50f); // Area around origin

	private List<GameObject> spawnedMushrooms = new List<GameObject>();
	private Terrain terrain;
	private TerrainData terrainData;

	void Start()
	{
		terrain = Terrain.activeTerrain;
		terrainData = terrain.terrainData;

		SpawnMushrooms();
	}

	public void SpawnMushrooms()
	{
		for (int i = 0; i < numberOfMushrooms; i++)
		{
			// Random position within spawn area
			float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
			float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

			// Get terrain height at this position
			Vector3 worldPos = terrain.transform.position + new Vector3(randomX, 0, randomZ);
			float height = terrain.SampleHeight(worldPos);
			Vector3 spawnPosition = new Vector3(worldPos.x, height, worldPos.z);

			// Random mushroom type
			int randomIndex = Random.Range(0, mushroomPrefabs.Length);

			// Spawn with random rotation
			Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
			GameObject mushroom = Instantiate(mushroomPrefabs[randomIndex], spawnPosition, randomRotation);

			spawnedMushrooms.Add(mushroom);
		}
	}
}

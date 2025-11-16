using UnityEngine;
using System.Collections.Generic;

public class TreeSpawner : MonoBehaviour
{
	[SerializeField] private GameObject treePrefab;
	[SerializeField] private int numberOfTrees = 5;
	[SerializeField] private float spawnRadius = 20f;
	[SerializeField] private Transform spawnCenter;
	[SerializeField] private float spawnHeight = 50f;
	[SerializeField] private LayerMask groundLayer;

	private int treesChopped = 0;
	private int treesSpawned = 0;

	void OnEnable()
	{
		ChoppableTree.OnAnyTreeChopped += OnTreeChoppedDown;
	}

	void OnDisable()
	{
		ChoppableTree.OnAnyTreeChopped -= OnTreeChoppedDown;
	}

	// Set tree count dynamically
	public void SetTreeCount(int count)
	{
		numberOfTrees = count;
	}

	public void SpawnTrees()
	{
		treesChopped = 0;
		treesSpawned = 0;

		for (int i = 0; i < numberOfTrees; i++)
		{
			Vector3 randomPos = GetRandomGroundPosition();

			if (randomPos != Vector3.zero)
			{
				GameObject tree = Instantiate(treePrefab, randomPos, Quaternion.identity);
				treesSpawned++;
			}
		}

		Debug.Log($"Spawned {treesSpawned} trees");
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

		Debug.LogWarning("Could not find valid ground position for tree");
		return Vector3.zero;
	}

	private void OnTreeChoppedDown()
	{
		treesChopped++;
		Debug.Log($"Trees chopped: {treesChopped}/{treesSpawned}");

		if (treesChopped >= treesSpawned)
		{
			Debug.Log("All spawned trees have been chopped!");
		}
	}

	void OnDrawGizmosSelected()
	{
		if (spawnCenter != null)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
		}
	}
}

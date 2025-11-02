using UnityEngine;

public class ChoppableTree : MonoBehaviour
{
	public GameObject choppedTreePrefab; // Assign your top + stump prefab here
	private int chopCount = 0;
	private int chopsNeeded = 3; // How many hits to chop it down

	public void TakeChop()
	{
		chopCount++;
		Debug.Log("Chop " + chopCount + " of " + chopsNeeded);

		if (chopCount >= chopsNeeded)
		{
			ChopTree();
		}
	}

	void ChopTree()
	{
		// Get TaskListManager reference
		TaskListManager taskManager = FindObjectOfType<TaskListManager>();

		// Spawn the chopped version
		Vector3 spawnPos = transform.position;
		spawnPos.y += 0.5f;
		Instantiate(choppedTreePrefab, spawnPos, transform.rotation);

		// Update task
		if (taskManager != null)
		{
			taskManager.UpdateWoodProgress(1);
		}

		// Destroy the intact tree
		Destroy(gameObject);
	}


}

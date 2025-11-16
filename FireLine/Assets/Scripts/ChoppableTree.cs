using UnityEngine;
using System;

public class ChoppableTree : MonoBehaviour
{
	public static event Action OnAnyTreeChopped;
	public static int totalWoodChopped = 0; // Track total wood across all trees

	public GameObject choppedTreePrefab;
	private int chopCount = 0;
	private int chopsNeeded = 3;
	private TreesManager treesManager;

	void Start()
    {
        treesManager = GameObject.Find("Terrain").GetComponent<TreesManager>();
    }
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
		// Notify tutorial if active
		TutorialManager tutorial = FindObjectOfType<TutorialManager>();
		if (tutorial != null)
		{
			tutorial.OnTreeChopped();
		}

		// Spawn the chopped version
		Vector3 spawnPos = transform.position;
		spawnPos.y += 0.5f;
		var choppedTree = Instantiate(choppedTreePrefab, spawnPos, transform.rotation);
		int treeIndex = treesManager.trees.FindIndex(t => t.RealTree == gameObject);
		treesManager.trees[treeIndex].RealTree = choppedTree;

		// Increment total wood count
		totalWoodChopped++;

		// Trigger the static event
		OnAnyTreeChopped?.Invoke();

		// Update task progress with TOTAL wood count
		TaskListManager taskManager = FindObjectOfType<TaskListManager>();
		if (taskManager != null)
		{
			taskManager.UpdateWoodProgress(totalWoodChopped);
		}

		// Destroy the intact tree
		Destroy(gameObject);
	}

	// Reset wood count at start of new day
	public static void ResetWoodCount()
	{
		totalWoodChopped = 0;
	}
}

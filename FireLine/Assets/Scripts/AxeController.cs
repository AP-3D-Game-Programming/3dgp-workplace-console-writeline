using UnityEngine;
using System.Collections;

public class AxeController : MonoBehaviour
{
	[SerializeField] private Animator playerAnimator; // Your player's Animator
	[SerializeField] private TaskListManager taskListManager; // For updating wood chop progress
	[SerializeField] private float swingCooldown = 0.8f; // Time between swings

	private float lastSwingTime = 0f;
	private bool isSwinging = false;

	void Update()
	{
		// Check if axe is equipped
		InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
		Tool currentTool = inventoryManager.GetCurrentTool();

		if (currentTool != null && currentTool.toolName == "Axe")
		{
			// Left click to swing axe
			if (Input.GetMouseButtonDown(0) && !isSwinging && Time.time >= lastSwingTime + swingCooldown)
			{
				SwingAxe();
			}
		}
	}

	private void SwingAxe()
	{
		isSwinging = true;
		lastSwingTime = Time.time;

		// Play swing animation
		if (playerAnimator != null)
		{
			playerAnimator.SetTrigger("SwingAxe");
		}

		// Wait for animation to complete, then process the chop
		StartCoroutine(ProcessChop());
	}

	private IEnumerator ProcessChop()
	{
		// Wait for animation to finish (adjust time based on your animation length)
		yield return new WaitForSeconds(0.5f);

		// Add progress to wood chop task
		if (taskListManager != null)
		{
			SubtaskUI[] allTasks = taskListManager.GetComponentsInChildren<SubtaskUI>();

			// Find the "Chop wood" task (first one with progress)
			foreach (SubtaskUI task in allTasks)
			{
				if (task.IsComplete())
					continue;

				// Check if this task has progress (wood chop task)
				task.AddProgress();
				Debug.Log("Wood chopped! Task progress updated.");
				break;
			}
		}

		isSwinging = false;
	}
}

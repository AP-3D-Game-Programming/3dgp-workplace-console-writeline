using UnityEngine;
using TMPro;

public class TaskManager : MonoBehaviour
{
	public TextMeshProUGUI taskText;

	// Turn on the campfire task
	private int woodChopped = 0;
	private int woodNeeded = 3;
	private bool woodPlacedOnFire = false;
	private bool campfireTaskComplete = false;

	// Clean camp task
	private int trashCollected = 0;
	private int trashNeeded = 5;
	private bool campCleanedComplete = false;

	// Patrol towers task
	private int towersPatrolled = 0;
	private int towersNeeded = 3;
	private bool patrolComplete = false;

	void Start()
	{
		UpdateTaskDisplay();
	}

	void UpdateTaskDisplay()
	{
		string tasks = "<size=32><b>Forest Watcher Tasks</b></size>\n\n";

		// Campfire task with sub-tasks
		if (!campfireTaskComplete)
		{
			tasks += "<b><color=#faf8df>Turn on the campfire:</color></b>\n";
			tasks += "  <b>- Chop 3 wood</b> <color=#faf8df>" + woodChopped + "/" + woodNeeded + "</color>\n";

			if (woodChopped >= woodNeeded && !woodPlacedOnFire)
				tasks += "  <b>- Put the wood on the campfire</b>\n";
				tasks += "  <b>- Put the wood on the campfire</b>\n";

			tasks += "\n";
		}

		// Clean camp task
		if (!campCleanedComplete)
		{
			tasks += "<b><color=#faf8df>Clean camp from visitors:</color></b>\n";
			tasks += "  <b>- Collect trash</b> <color=#faf8df>" + trashCollected + "/" + trashNeeded + "</color>\n\n";
		}

		// Patrol towers task
		if (!patrolComplete)
		{
			tasks += "<b><color=#faf8df>Patrol lookout towers:</color></b>\n";
			tasks += "  <b>- Visit towers</b> <color=#faf8df>" + towersPatrolled + "/" + towersNeeded + "</color>\n";
		}

		taskText.text = tasks;
	}



	// Methods to update progress
	public void ChopWood()
	{
		if (woodChopped < woodNeeded)
		{
			woodChopped++;
			UpdateTaskDisplay();
			Debug.Log("Wood chopped: " + woodChopped + "/" + woodNeeded);
		}
	}

	public void PlaceWoodOnFire()
	{
		if (woodChopped >= woodNeeded && !woodPlacedOnFire)
		{
			woodPlacedOnFire = true;
			campfireTaskComplete = true;
			UpdateTaskDisplay();
			Debug.Log("Campfire lit!");
		}
	}

	public void CollectTrash()
	{
		if (trashCollected < trashNeeded)
		{
			trashCollected++;
			UpdateTaskDisplay();

			if (trashCollected >= trashNeeded)
			{
				campCleanedComplete = true;
				Debug.Log("Camp cleaned!");
			}
		}
	}

	public void VisitTower()
	{
		if (towersPatrolled < towersNeeded)
		{
			towersPatrolled++;
			UpdateTaskDisplay();

			if (towersPatrolled >= towersNeeded)
			{
				patrolComplete = true;
				Debug.Log("All towers patrolled!");
			}
		}
	}
}

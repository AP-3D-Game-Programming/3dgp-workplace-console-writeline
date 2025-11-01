using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
	public static DayManager Instance { get; private set; }

	[SerializeField] private int currentDay = 1;
	[SerializeField] private TaskListManager taskListManager; // Reference to your task manager
	private bool hasCompletedDailyTasks = false;
	private bool canSleep = false;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	void Start()
	{
		StartNewDay();
	}

	private void StartNewDay()
	{
		hasCompletedDailyTasks = false;
		canSleep = false;

		Debug.Log("Day " + currentDay + " started!");

		// Clear old tasks
		// TODO: Add a ClearAllTasks() method to TaskListManager

		// Generate and assign today's daily tasks
		GenerateDailyTasks();
	}

	private void GenerateDailyTasks()
	{
		// Define which tasks are mandatory each day
		// You can randomize them or make them fixed per day

		if (currentDay == 1)
		{
			taskListManager.AddObjective("Daily Task: Water the flowers", new List<string>
			{
				"Add water to your bucket",
				"Water the flowers 0/3"
			});
		}
		else if (currentDay == 2)
		{
			taskListManager.AddObjective("Daily Task: Check the watchtower", new List<string>
			{
				"Climb the tower",
				"Look for smoke"
			});
		}
		else
		{
			// Repeat or randomize tasks for later days
			taskListManager.AddObjective("Daily Task: Prepare the campfire", new List<string>
			{
				"Chop 3 wood  0/3",
				"Put the wood on the fire"
			});
		}
	}

	public void CompleteAllDailyTasks()
	{
		hasCompletedDailyTasks = true;
		canSleep = true;

		Debug.Log("All daily tasks completed! You can now sleep.");
	}

	public void ProgressToNextDay()
	{
		if (!canSleep)
		{
			Debug.Log("You haven't completed your daily tasks yet!");
			return;
		}

		currentDay++;
		StartNewDay();
	}

	public int GetCurrentDay()
	{
		return currentDay;
	}

	public bool CanSleep()
	{
		return canSleep;
	}

	public bool HasCompletedDailyTasks()
	{
		return hasCompletedDailyTasks;
	}
}

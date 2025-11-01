using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
	public static DayManager Instance { get; private set; }

	[SerializeField] private int currentDay = 1;
	[SerializeField] private TaskListManager taskListManager;
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

	void Update()
	{
		// Check if day ended (midnight reached)
		if (TimeManager.Instance.HasDayEnded())
		{
			if (!hasCompletedDailyTasks)
			{
				Debug.Log("GAME OVER! You didn't complete your tasks before midnight!");
				// TODO: Show game over screen or fail state
			}
		}
	}

	private void StartNewDay()
	{
		hasCompletedDailyTasks = false;
		canSleep = false;

		Debug.Log("Day " + currentDay + " started!");

		taskListManager.ClearAllTasks();
		GenerateDailyTasks();

		// Reset the day/night cycle
		TimeManager.Instance.ResetDay();
	}

	private void GenerateDailyTasks()
	{
		// Tasks get harder each day
		int taskDifficulty = currentDay;

		if (currentDay == 1)
		{
			taskListManager.AddObjective("Daily: Water the plants", new List<string>
			{
				"Fill your bucket with water",
				"Water the plants"
			});
		}
		else if (currentDay == 2)
		{
			taskListManager.AddObjective("Check the watchtower", new List<string>
			{
				"Climb the tower",
				"Look for smoke"
			});
			taskListManager.AddObjective("Prepare the campfire", new List<string>
			{
				"Chop 5 wood  0/5"
			});
		}
		else
		{
			// Add more tasks as days progress
			taskListManager.AddObjective("Patrol the forest", new List<string>
			{
				"Check north area",
				"Check south area"
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
		if (!canSleep && !TimeManager.Instance.HasDayEnded())
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
}

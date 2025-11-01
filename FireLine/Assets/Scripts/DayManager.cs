using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
	public static DayManager Instance { get; private set; }

	[SerializeField] private int currentDay = 1;
	[SerializeField] private TaskListManager taskListManager;
	[SerializeField] private Light sun; // Add this
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
		if (sun == null)
			sun = FindObjectOfType<Light>();

		StartNewDay();
	}

	void Update()
	{
		if (TimeManager.Instance.HasDayEnded())
		{
			if (!hasCompletedDailyTasks)
			{
				Debug.Log("GAME OVER! You didn't complete your tasks before midnight!");
			}
			if (taskListManager.AreAllTasksComplete() && !hasCompletedDailyTasks)
			{
				CompleteAllDailyTasks();
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

		// Reset the day/night cycle and sun position
		TimeManager.Instance.ResetDay();
		sun.transform.rotation = Quaternion.Euler(0, 0, 0); // Reset sun to sunrise (0 degrees)
	}

	private void GenerateDailyTasks()
	{
		if (currentDay == 1)
		{
			taskListManager.AddObjective("Prepare the campfire", new List<string>
			{
				"Chop 3 wood  0/3",
				"Put the wood on the fire"
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
}

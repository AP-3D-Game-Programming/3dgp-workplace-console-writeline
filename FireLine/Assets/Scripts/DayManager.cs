using UnityEngine;
using System.Collections.Generic;

public class DayManager : MonoBehaviour
{
	public static DayManager Instance { get; private set; }

	[SerializeField] private int currentDay = 1;
	[SerializeField] public TaskListManager taskListManager;
	[SerializeField] private Light sun;
	[SerializeField] private TreeSpawner treeSpawner; // Add reference to TreeSpawner
	[SerializeField] private MushroomSpawnManager mushroomSpawner;
	private bool hasCompletedDailyTasks = false;
	private bool canSleep = false;

	private TutorialManager tutorialManager;

	[Header("Progression Settings")]
	[SerializeField] private int startingTreeCount = 5; // Trees required on Day 2
	[SerializeField] private int treeIncreasePerDay = 3; // How many more trees each day


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

		// Find the TutorialManager in the scene
		tutorialManager = FindObjectOfType<TutorialManager>();

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

		// Reset wood count for new day
		ChoppableTree.ResetWoodCount();

		taskListManager.ClearAllTasks();
		GenerateDailyTasks();

		// Reset the day/night cycle and sun position
		TimeManager.Instance.ResetDay();
		sun.transform.rotation = Quaternion.Euler(0, 0, 0);
	}



	private void GenerateDailyTasks()
	{
		if (currentDay == 1)
		{
			if (tutorialManager != null)
			{
				// Start interactive tutorial for Day 1
				tutorialManager.StartTutorial();
			}
			else
			{
				// Fallback normal objectives if tutorial missing
				taskListManager.AddObjective("Prepare the campfire", new List<string>
				{
					"Chop 1 wood  0/1",
				});
			}
		}
		else if (currentDay == 2)
		{
			taskListManager.AddObjective("Gather wood for winter", new List<string>
			{
				"Chop 5 trees  0/5"
			});

			// Spawn 5 trees for the player to chop
			if (treeSpawner != null)
			{
				treeSpawner.SpawnTrees();
				Debug.Log("Spawned trees for Day 2 wood gathering task.");
			}
		}
		else if (currentDay == 3)
		{
			taskListManager.AddObjective("Mushroom soup", new List<string>
			{
				"Pick up 10 mushrooms 0/10"
			});
		}
		else
		{
			// When no tasks are there: Infinite scaling tree chopping
			int treesToChop = CalculateTreesForDay(currentDay);

			taskListManager.AddObjective("Gather wood for winter", new List<string>
		{
			$"Chop {treesToChop} trees  0/{treesToChop}"
		});

			// Spawn the required number of trees
			if (treeSpawner != null)
			{
				treeSpawner.SetTreeCount(treesToChop);
				treeSpawner.SpawnTrees();
				Debug.Log($"Day {currentDay}: Spawned {treesToChop} trees.");
			}
		}

	}

	private int CalculateTreesForDay(int day)
	{
		// Day 2 = 5 trees, Day 3 = 8 trees, Day 4 = 11 trees, etc.
		return startingTreeCount + ((day - 2) * treeIncreasePerDay);
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

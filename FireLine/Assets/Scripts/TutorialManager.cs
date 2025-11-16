using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public GameObject tutorialTree;
	public GameObject bed;
	private Outline tutorialTreeOutline;
	private bool tutorialCompleted = false;

	private int step = 0;

	public TaskListManager taskListManager;
	public InventoryManager inventoryManager;

	private List<SubtaskUI> moveSubtasks = new List<SubtaskUI>();
	private List<SubtaskUI> axeSubtasks = new List<SubtaskUI>();
	private List<SubtaskUI> sleepSubtasks = new List<SubtaskUI>();

	void Start()
	{
		// Only show tutorial on Day 1
		if (DayManager.Instance.GetCurrentDay() == 1)
		{
			ShowStep(0);
		}

		if (tutorialTree != null)
			tutorialTreeOutline = tutorialTree.GetComponent<Outline>();

		if (tutorialTreeOutline != null)
			tutorialTreeOutline.enabled = false;

		Outline bedOutline = bed.GetComponent<Outline>();
		if (bedOutline == null)
		{
			Debug.LogError("Bed does NOT have an Outline component!");
		}
		else
		{
			Debug.Log("Bed outline found and disabled");
			bedOutline.enabled = false;
		}
	}

	public void StartTutorial()
	{
		// Only start tutorial on Day 1 and if not completed
		if (DayManager.Instance.GetCurrentDay() == 1 && !tutorialCompleted)
		{
			gameObject.SetActive(true);
			ShowStep(0);
		}
	}

	private void Update()
	{
		// Skip all tutorial logic if not Day 1 or tutorial is completed
		if (DayManager.Instance.GetCurrentDay() != 1 || tutorialCompleted)
			return;

		if (step == 0)
		{
			if (Input.GetKeyDown(KeyCode.W))
				moveSubtasks[0].MarkComplete();
			if (Input.GetKeyDown(KeyCode.A))
				moveSubtasks[1].MarkComplete();
			if (Input.GetKeyDown(KeyCode.S))
				moveSubtasks[2].MarkComplete();
			if (Input.GetKeyDown(KeyCode.D))
				moveSubtasks[3].MarkComplete();
			if (Input.GetKeyDown(KeyCode.Space))
				moveSubtasks[4].MarkComplete();

			if (moveSubtasks.TrueForAll(st => st.IsComplete()))
			{
				taskListManager.ClearAllTasks();
				ShowStep(1);
			}
		}
		else if (step == 1)
		{
			if (inventoryManager.GetCurrentTool() != null && inventoryManager.GetCurrentTool().toolName == "Axe")
			{
				axeSubtasks[0].MarkComplete();
			}

			if (axeSubtasks.TrueForAll(st => st.IsComplete()))
			{
				taskListManager.ClearAllTasks();
				ShowStep(2);
			}
		}
		else if (step == 2)
		{
			Outline bedOutline = bed.GetComponent<Outline>();
			if (bedOutline != null && !bedOutline.enabled)
				bedOutline.enabled = true;
		}
	}

	void ShowStep(int stepIndex)
	{
		step = stepIndex;
		switch (step)
		{
			case 0:
				taskListManager.AddObjective("Move using WASD keys.", new List<string>());
				moveSubtasks.Clear();
				moveSubtasks.Add(taskListManager.AddSubtask("Press W to move forward"));
				moveSubtasks.Add(taskListManager.AddSubtask("Press A to move left"));
				moveSubtasks.Add(taskListManager.AddSubtask("Press S to move backward"));
				moveSubtasks.Add(taskListManager.AddSubtask("Press D to move right"));
				moveSubtasks.Add(taskListManager.AddSubtask("Press Space to jump"));
				break;

			case 1:
				taskListManager.AddObjective("Find a choppable tree", new List<string>());
				axeSubtasks.Add(taskListManager.AddSubtask("Equip your axe using 1, 2, 3 or 4"));
				HighlightTree(true);
				axeSubtasks.Add(taskListManager.AddSubtask("Find a tree to chop"));
				break;

			case 2:
				taskListManager.AddObjective("Go to sleep", new List<string>());
				sleepSubtasks.Add(taskListManager.AddSubtask("After you completed your tasks, go to the tower and sleep"));
				break;
		}
	}

	public void OnAxePickedUp()
	{
		ShowStep(1);
	}

	public void HighlightTree(bool highlight)
	{
		if (tutorialTreeOutline != null)
			tutorialTreeOutline.enabled = highlight;
	}

	public void OnTreeChopped()
	{
		if (axeSubtasks.Count > 1)
		{
			axeSubtasks[1].MarkComplete();
		}
	}

	public bool IsTutorialAtSleepStep()
	{
		return DayManager.Instance.GetCurrentDay() == 1 && step == 2 && !tutorialCompleted;
	}

	public void OnPlayerReachedBed()
	{
		// Only execute on Day 1, step 2, and if not already completed
		if (DayManager.Instance.GetCurrentDay() == 1 && step == 2 && sleepSubtasks.Count > 0 && !tutorialCompleted)
		{
			sleepSubtasks[0].MarkComplete();
			DayManager.Instance.CompleteAllDailyTasks();
			tutorialCompleted = true;

			// Disable bed outline after tutorial
			Outline bedOutline = bed.GetComponent<Outline>();
			if (bedOutline != null)
				bedOutline.enabled = false;
		}
	}
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
	public GameObject tutorialTree;        // Assign the "TutorialTree" GameObject in Inspector
	public GameObject bed;
	private Outline tutorialTreeOutline;

	private int step = 0;

	public TaskListManager taskListManager;
	public InventoryManager inventoryManager;

	private List<SubtaskUI> moveSubtasks = new List<SubtaskUI>();
	private List<SubtaskUI> axeSubtasks = new List<SubtaskUI>();
	private List<SubtaskUI> sleepSubtasks = new List<SubtaskUI>();

	void Start()
	{
		ShowStep(0);
		if (tutorialTree != null)
			tutorialTreeOutline = tutorialTree.GetComponent<Outline>();

		if (tutorialTreeOutline != null)
			tutorialTreeOutline.enabled = false;

		// DEBUG: Check if bed has outline
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
		// Show tutorial UI and steps here
		gameObject.SetActive(true);
		ShowStep(0); // Start from step 0
	}

	private void Update()
	{
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

			// If all move subtasks complete, progress to next step
			if (moveSubtasks.TrueForAll(st => st.IsComplete()))
			{
				taskListManager.ClearAllTasks();
				ShowStep(1);
			}
		}
		if (step == 1)
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
			// Wait for axe pickup and tree chop events to progress
		}
		if (step == 2)
		{
			Outline bedOutline = bed.GetComponent<Outline>();
			if (bedOutline != null)
				bedOutline.enabled = true;
			else
				Debug.LogError("Cannot enable bed outline - component not found!");
		}

	}




	void ShowStep(int stepIndex)
	{
		step = stepIndex;
		switch (step)
		{
			case 0:
				taskListManager.AddObjective("Move using WASD keys.", new List<string>());

				// Manually create each subtask UI so you can track them
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

	// Call this from your axe pickup code
	public void OnAxePickedUp()
	{
		ShowStep(1);
	}

	public void HighlightTree(bool highlight)
	{
		if (tutorialTreeOutline != null)
			tutorialTreeOutline.enabled = highlight;
	}


	// Call this when the tree is chopped
	public void OnTreeChopped()
	{
		if (axeSubtasks.Count > 1) // Assuming second subtask is chop tree
		{
			axeSubtasks[1].MarkComplete();
		}
		
	}

	public void EnableSleepForTutorial()
	{
    // Mark all tutorial tasks as complete so player can sleep
    DayManager.Instance.CompleteAllDailyTasks();
	}

	public bool IsTutorialAtSleepStep()
	{
		return step == 2;
	}



}

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TaskListManager : MonoBehaviour
{
	[SerializeField] private GameObject taskContainerPrefab; // Your TaskContainer prefab
	[SerializeField] private Transform objectiveListParent; // Where TaskContainers spawn
	[SerializeField] private GameObject subtaskItemPrefab; // SubtaskItem prefab

	private List<GameObject> activeObjectives = new List<GameObject>();

	void Start()
	{
		// Add multiple objectives
		AddObjective("Prepare the campfire", new List<string> { "Chop 3 wood  0/3", "Put the wood on the fire" });
		AddObjective("Check the watchtower", new List<string> { "Climb the tower", "Look for smoke" });
	}

	/// <summary>
	///		Add a new objective with subtasks to the task list.
	///		Some subtasks can have progress tracking (e.g., "Chop 3 wood 0/3").
	/// </summary>
	/// <param name="title"></param>
	/// <param name="subtasks"></param>
	/// 
	/// USAGE: Get all SubtaskUI components and update the first one
	///	SubtaskUI[] allTasks = taskListManager.GetComponentsInChildren<SubtaskUI>();
	///	allTasks[0].AddProgress(); // Updates "Chop 3 wood 0/3" to "Chop 3 wood 1/3"
	public void AddObjective(string title, List<string> subtasks)
	{
		GameObject taskContainer = Instantiate(taskContainerPrefab, objectiveListParent, false);

		TextMeshProUGUI titleText = taskContainer.transform.Find("TaskTitle").GetComponent<TextMeshProUGUI>();
		Transform subtaskList = taskContainer.transform.Find("SubtaskList");

		titleText.text = title.ToUpper();

		SubtaskUI[] existingSubtasks = subtaskList.GetComponentsInChildren<SubtaskUI>();
		if (existingSubtasks.Length > 0)
		{
			ParseAndSetup(existingSubtasks[0], subtasks[0]);
		}

		for (int i = 1; i < subtasks.Count; i++)
		{
			GameObject subtask = Instantiate(subtaskItemPrefab, subtaskList, false);
			ParseAndSetup(subtask.GetComponent<SubtaskUI>(), subtasks[i]);
		}

		activeObjectives.Add(taskContainer);
	}

	private void ParseAndSetup(SubtaskUI subtaskUI, string taskText)
	{
		// Check if text contains progress (e.g., "Chop 3 wood 0/3")
		if (taskText.Contains("/"))
		{
			string[] parts = taskText.Split('/');
			string baseText = taskText.Substring(0, taskText.LastIndexOf(' ')); // "Chop 3 wood 0"
			baseText = baseText.Substring(0, baseText.LastIndexOf(' ')); // "Chop 3 wood"

			int current = int.Parse(parts[0].Split(' ').Last()); // Get 0 from "Chop 3 wood 0"
			int total = int.Parse(parts[1]); // Get 3

			subtaskUI.SetupWithProgress(baseText, current, total);
		}
		else
		{
			subtaskUI.Setup(taskText);
		}
	}



	public void RemoveObjective(int index)
	{
		if (index < activeObjectives.Count)
		{
			Destroy(activeObjectives[index]);
			activeObjectives.RemoveAt(index);
		}
	}

	// Marks a specific subtask as complete based on its index in the overall list
	// Use: gameManager.GetComponent<TaskListManager>().CompleteTask(0); // Completes first task

	public void CompleteTask(int taskIndex)
	{
		// Find all subtasks and complete the one at taskIndex
		SubtaskUI[] allSubtasks = objectiveListParent.GetComponentsInChildren<SubtaskUI>();
		if (taskIndex < allSubtasks.Length)
		{
			allSubtasks[taskIndex].MarkComplete();
		}
	}

}

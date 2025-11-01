using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TaskListManager : MonoBehaviour
{
	[SerializeField] private GameObject taskContainerPrefab;
	[SerializeField] private Transform objectiveListParent;
	[SerializeField] private GameObject subtaskItemPrefab;

	private List<GameObject> activeObjectives = new List<GameObject>();

	void Start()
	{
		AddObjective("Prepare the campfire", new List<string> { "Chop 3 wood  0/3", "Put the wood on the fire" });
		AddObjective("Check the watchtower", new List<string> { "Climb the tower", "Look for smoke" });
	}

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
		if (taskText.Contains("/"))
		{
			string[] parts = taskText.Split('/');
			string baseText = taskText.Substring(0, taskText.LastIndexOf(' '));
			baseText = baseText.Substring(0, baseText.LastIndexOf(' '));

			int current = int.Parse(parts[0].Split(' ').Last());
			int total = int.Parse(parts[1]);

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

	public void CompleteTask(int taskIndex)
	{
		SubtaskUI[] allSubtasks = objectiveListParent.GetComponentsInChildren<SubtaskUI>();
		if (taskIndex < allSubtasks.Length)
		{
			allSubtasks[taskIndex].MarkComplete();
		}
	}

	// Check if ALL subtasks are complete
	public bool AreAllTasksComplete()
	{
		SubtaskUI[] allSubtasks = objectiveListParent.GetComponentsInChildren<SubtaskUI>();

		// If no tasks exist, return false
		if (allSubtasks.Length == 0)
			return false;

		// All tasks must be complete
		return allSubtasks.All(task => task.IsComplete());
	}

	public void ClearAllTasks()
	{
		foreach (GameObject obj in activeObjectives)
		{
			Destroy(obj);
		}
		activeObjectives.Clear();
		Debug.Log("All tasks cleared");
	}
}

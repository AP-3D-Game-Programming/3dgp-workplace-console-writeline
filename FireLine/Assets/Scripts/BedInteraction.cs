using UnityEngine;
using System.Collections;

public class BedInteraction : MonoBehaviour
{
	[SerializeField] private float interactionRange = 2f;
	[SerializeField] private Canvas interactionPromptUI;
	private bool isPlayerNearby = false;
	private SphereCollider triggerCollider;
	private TaskListManager taskListManager;

	void Start()
	{
		interactionPromptUI.gameObject.SetActive(false);

		triggerCollider = GetComponent<SphereCollider>();
		if(!DayManager.Instance.taskListManager.AreAllTasksComplete())
		{
			interactionPromptUI.gameObject.SetActive(false);
			if (triggerCollider == null)
			{
				triggerCollider = gameObject.AddComponent<SphereCollider>();
			}
		}
		

		triggerCollider.radius = interactionRange;
		triggerCollider.isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		// Check if tasks are complete OR tutorial is at sleep step
		bool canShowPrompt = DayManager.Instance.taskListManager.AreAllTasksComplete();

		// If tutorial exists and is at sleep step, also allow prompt
		TutorialManager tutorial = FindFirstObjectByType<TutorialManager>();
		if (tutorial != null && tutorial.IsTutorialAtSleepStep())
		{
			canShowPrompt = true;
		}

		if (canShowPrompt)
		{
			if (other.CompareTag("Player"))
			{
				isPlayerNearby = true;
				interactionPromptUI.gameObject.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNearby = false;
			interactionPromptUI.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		// Same logic - allow sleep if tasks complete OR tutorial at sleep step
		bool canSleep = DayManager.Instance.taskListManager.AreAllTasksComplete();

		TutorialManager tutorial = FindFirstObjectByType<TutorialManager>();
		if (tutorial != null && tutorial.IsTutorialAtSleepStep())
		{
			canSleep = true;
		}

		if (canSleep)
		{
			if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
			{
				Sleep();
			}
		}
	}

	private void Sleep()
	{
		// Remove the AreAllTasksComplete check since we already checked above
		DayManager.Instance.CompleteAllDailyTasks();

		Debug.Log("Player is sleeping!");
		interactionPromptUI.gameObject.SetActive(false);
		isPlayerNearby = false;

		StartCoroutine(SleepSequence());
	}

	private IEnumerator SleepSequence()
	{
		// Fade to black, generate new day tasks while black, fade back
		yield return StartCoroutine(FadeManager.Instance.FadeToBlackAndBack(() =>
		{
			DayManager.Instance.ProgressToNextDay();
		}));
	}
}

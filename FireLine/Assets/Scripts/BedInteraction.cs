using UnityEngine;
using System.Collections;

public class BedInteraction : MonoBehaviour
{
	[SerializeField] private float interactionRange = 2f;
	[SerializeField] private Canvas interactionPromptUI;
	private bool isPlayerNearby = false;
	private SphereCollider triggerCollider;

	void Start()
	{
		interactionPromptUI.gameObject.SetActive(false);

		triggerCollider = GetComponent<SphereCollider>();
		if (triggerCollider == null)
		{
			triggerCollider = gameObject.AddComponent<SphereCollider>();
		}

		triggerCollider.radius = interactionRange;
		triggerCollider.isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNearby = true;
			interactionPromptUI.gameObject.SetActive(true);
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
		if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
		{
			Sleep();
		}
	}

	private void Sleep()
	{
		// Check if all tasks are actually complete
		if (!DayManager.Instance.taskListManager.AreAllTasksComplete())
		{
			Debug.Log("You need to finish your daily tasks before sleeping!");
			return;
		}

		// If tasks are complete, mark them as done
		DayManager.Instance.CompleteAllDailyTasks();

		Debug.Log("Player is sleeping!");
		interactionPromptUI.gameObject.SetActive(false);
		isPlayerNearby = false;

		// Start the fade animation and sleep sequence
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

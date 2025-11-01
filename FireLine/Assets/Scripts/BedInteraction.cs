using UnityEngine;

public class BedInteraction : MonoBehaviour
{
	[SerializeField] private float interactionRange = 2f; // How close player needs to be
	[SerializeField] private Canvas interactionPromptUI; // Your UI canvas with the prompt
	private bool isPlayerNearby = false;
	private SphereCollider triggerCollider;

	void Start()
	{
		// Disable the prompt UI on start
		interactionPromptUI.gameObject.SetActive(false);

		// Get or create a sphere collider and set its radius based on interactionRange
		triggerCollider = GetComponent<SphereCollider>();
		if (triggerCollider == null)
		{
			triggerCollider = gameObject.AddComponent<SphereCollider>();
		}

		// Set the collider radius to match the interaction range
		triggerCollider.radius = interactionRange;
		triggerCollider.isTrigger = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNearby = true;
			interactionPromptUI.gameObject.SetActive(true); // Show "Press E to sleep"
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNearby = false;
			interactionPromptUI.gameObject.SetActive(false); // Hide prompt
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
		if (!DayManager.Instance.CanSleep())
		{
			Debug.Log("You need to finish your daily tasks before midnight!");
			return;
		}

		Debug.Log("Player is sleeping!");
		interactionPromptUI.gameObject.SetActive(false);
		isPlayerNearby = false;

		DayManager.Instance.ProgressToNextDay();
	}


}

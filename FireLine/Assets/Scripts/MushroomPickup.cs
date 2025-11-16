using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
	[SerializeField] private float interactionRange = 2f;
	private GameObject interactionPromptUI;
	private bool isPlayerNearby = false;
	private SphereCollider triggerCollider;

	void Start()
	{
		Canvas canvas = GetComponentInChildren<Canvas>();
		if (canvas != null)
		{
			interactionPromptUI = canvas.gameObject;
			interactionPromptUI.SetActive(false);
			Debug.Log("Canvas found and hidden!"); // Debug
		}
		else
		{
			Debug.LogError("No Canvas found as child!"); // Debug
		}

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
			if (interactionPromptUI != null)
			{
				interactionPromptUI.SetActive(true); // Remove .gameObject - it's already a GameObject
				Debug.Log("Showing canvas!"); // Debug
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isPlayerNearby = false;
			if (interactionPromptUI != null)
			{
				interactionPromptUI.SetActive(false); // Remove .gameObject
			}
		}
	}

	void Update()
	{
		if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
		{
			CollectMushroom();
		}
	}

	private void CollectMushroom()
	{
		// Update task progress
		if (TaskListManager.Instance != null)
		{
			TaskListManager.Instance.UpdateMushroomProgress();
		}

		Debug.Log("Mushroom collected!");

		// Hide UI before destroying
		if (interactionPromptUI != null)
		{
			interactionPromptUI.SetActive(false); // Remove .gameObject
		}

		Destroy(gameObject);
	}
}

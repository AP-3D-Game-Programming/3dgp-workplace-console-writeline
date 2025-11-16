using UnityEngine;
using TMPro;

public class MushroomPickup : MonoBehaviour
{
	public static int mushroomsCollected = 0;
	public static int mushroomsNeeded = 10;

	[SerializeField] private float interactionDistance = 2f;
	[SerializeField] private Canvas pickupPromptUI; // Canvas: "Press E to collect" text
	private bool playerInRange = false;
	private GameObject player;
	private SphereCollider triggerCollider;

	void Start()
	{
		if (pickupPromptUI != null)
		{
			pickupPromptUI.gameObject.SetActive(false);
		}

		triggerCollider = GetComponent<SphereCollider>();
		if (triggerCollider == null)
		{
			triggerCollider = gameObject.AddComponent<SphereCollider>();
		}

		triggerCollider.radius = interactionDistance;
		triggerCollider.isTrigger = true;
	}

	void Update()
	{
		// Check for E key press
		if (playerInRange && Input.GetKeyDown(KeyCode.E))
		{
			CollectMushroom();
		}
	}

	void CollectMushroom()
	{
		// Update the task progress through TaskListManager
		if (TaskListManager.Instance != null)
		{
			TaskListManager.Instance.UpdateMushroomProgress();
		}

		Debug.Log("Mushroom collected!");

		// Destroy this mushroom
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			player = other.gameObject;
			playerInRange = true;

			if (pickupPromptUI != null)
			{
				pickupPromptUI.gameObject.SetActive(true);
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerInRange = false;
			player = null;

			if (pickupPromptUI != null)
			{
				pickupPromptUI.gameObject.SetActive(false);
			}
		}
	}
}

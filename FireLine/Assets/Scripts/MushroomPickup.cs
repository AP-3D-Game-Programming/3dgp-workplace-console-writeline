using UnityEngine;

public class MushroomPickup : MonoBehaviour
{
	private bool playerNearby = false;
	public float pickupRange = 2f;
	private Transform player;

	void Update()
	{
		if (playerNearby && Input.GetKeyDown(KeyCode.E))
		{
			CollectMushroom();
		}
	}

	void CollectMushroom()
	{
		TaskListManager.Instance?.UpdateMushroomProgress();
		Debug.Log("Mushroom collected!");
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerNearby = true;
			player = other.transform;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerNearby = false;
			player = null;
		}
	}
}

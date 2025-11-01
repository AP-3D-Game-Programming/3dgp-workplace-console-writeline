using UnityEngine;
using System.Collections;

public class AxeController : MonoBehaviour
{
	[SerializeField] private Transform axeModel; // Drag the axe model here
	[SerializeField] private TaskListManager taskListManager;
	[SerializeField] private float swingCooldown = 0.8f;

	private float lastSwingTime = 0f;
	private InventoryManager inventoryManager;
	private bool isSwinging = false;

	void Start()
	{
		inventoryManager = FindObjectOfType<InventoryManager>();
	}

	void Update()
	{
		if (inventoryManager == null) return;

		Tool currentTool = inventoryManager.GetCurrentTool();

		if (currentTool != null && currentTool.toolName == "Axe")
		{
			if (Input.GetMouseButtonDown(0) && !isSwinging && Time.time >= lastSwingTime + swingCooldown)
			{
				StartCoroutine(SwingAxe());
			}
		}
	}

	private IEnumerator SwingAxe()
	{
		isSwinging = true;
		lastSwingTime = Time.time;

		float elapsed = 0f;
		float swingDuration = 0.5f; // Match your 30-frame animation

		Vector3 startRotation = new Vector3(-37.224f, -180f, 90f);
		Vector3 swingRotation = new Vector3(15f, -180f, 90f);
		Vector3 endRotation = new Vector3(-37.224f, -180f, 90f);

		// Swing forward (0 to 10 frames)
		while (elapsed < swingDuration / 3)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / (swingDuration / 3);
			axeModel.localEulerAngles = Vector3.Lerp(startRotation, swingRotation, t);
			yield return null;
		}

		// Swing down (10 to 20 frames)
		elapsed = 0f;
		while (elapsed < swingDuration / 3)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / (swingDuration / 3);
			axeModel.localEulerAngles = Vector3.Lerp(swingRotation, new Vector3(-42.52f, -180f, 90f), t);
			yield return null;
		}

		// Back to start
		elapsed = 0f;
		while (elapsed < swingDuration / 3)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / (swingDuration / 3);
			axeModel.localEulerAngles = Vector3.Lerp(new Vector3(-42.52f, -180f, 90f), endRotation, t);
			yield return null;
		}

		isSwinging = false;
	}

}

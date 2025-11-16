using UnityEngine;

public class ChopManager : MonoBehaviour
{
	[SerializeField] private Camera firstPersonCamera;
	[SerializeField] private Camera thirdPersonCamera;
	[SerializeField] private Transform player;
	public float rayDistance = 100f;

	[SerializeField] private InventoryManager inventoryManager;

	private float lastChopTime = 0f;
	[SerializeField] private float chopCooldown = 1f; // 1 second cooldown

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Left click
		{
			// Check if cooldown has passed
			if (Time.time - lastChopTime < chopCooldown)
				return;

			lastChopTime = Time.time;

			Ray ray;

			if (firstPersonCamera.enabled)
			{
				ray = firstPersonCamera.ScreenPointToRay(Input.mousePosition);
			}
			else
			{
				ray = new Ray(player.position, player.forward);
			}

			if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
			{
				ChoppableTree tree = hit.collider.GetComponent<ChoppableTree>();
				if (inventoryManager == null) return;

				Tool currentTool = inventoryManager.GetCurrentTool();

				if (currentTool != null && currentTool.toolName == "Axe")
				{
					if (tree != null)
					{
						tree.TakeChop();
					}
				}
				
			}
		}
	}
}

using UnityEngine;

public class Chopper : MonoBehaviour
{
	public float rayDistance = 100f;

	void Update()
	{
		if (Input.GetMouseButtonDown(0)) // Left click
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
			{
				ChoppableTree tree = hit.collider.GetComponent<ChoppableTree>();
				if (tree != null)
				{
					tree.TakeChop();
				}
			}
		}
	}
}

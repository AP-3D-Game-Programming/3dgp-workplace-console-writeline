using UnityEngine;

public class TreeTop : MonoBehaviour
{
	[SerializeField] private float destroyAfterInSeconds = 10f;
	void Start()
	{
		// Destroy this object after 10 seconds
		Destroy(gameObject, destroyAfterInSeconds);
	}
}

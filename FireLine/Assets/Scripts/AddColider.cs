using UnityEngine;

public class Add : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	// Attach this script to your lookout tower parent object
	// Then right-click the script component in Inspector and select "Add Colliders"
	[ContextMenu("Add Colliders")]
	void AddCollidersToChildren()
	{
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter mf in meshFilters)
		{
			if (mf.GetComponent<MeshCollider>() == null)
			{
				mf.gameObject.AddComponent<MeshCollider>();
			}
		}
	}

}

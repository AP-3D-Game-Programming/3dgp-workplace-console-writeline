using System;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Collections;
using NUnit.Framework;


public class Bucket : MonoBehaviour
{
    private bool isRotatingForward;
    private float rotateThisFrame;
    private float rotation = 30f;
    private bool isRotatingBackward;
    private float roateSpeed = 3f;
	private float tiltHoldTime = 0f; // Timer for how long to hold the tilt
	private float tiltMaxHoldTime = 1f; // 3 seconds

	// Makes it public without letting other scripts able to use this varaible.
	[SerializeField] private Transform waterSplash;

	public GameObject sphereColliderPrefab;

	private InventoryManager inventoryManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
	{
		isRotatingForward = false;
		rotateThisFrame = 0;
		isRotatingBackward = false;
		tiltHoldTime = 0f;

		inventoryManager = UnityEngine.Object.FindFirstObjectByType<InventoryManager>();

		if (waterSplash == null)
			Debug.Log("Water splash not assigned in Inspector!");
	}


	// Update is called once per frame
	void Update()
	{

		if (!IsBucketEquipped())
			return;

		if (Input.GetMouseButton(0))
		{
			if (rotateThisFrame == 0)
			{
				isRotatingForward = true;
				// Look for water_splash in the bucket model child
				if (waterSplash != null)
				{
					waterSplash.gameObject.SetActive(true);
				}

				Instantiate(sphereColliderPrefab, transform);
			}
		}

		if (isRotatingForward)
		{
			transform.Rotate(Vector3.right * roateSpeed);
			rotateThisFrame += roateSpeed;
			if (rotateThisFrame >= rotation)
			{
				isRotatingForward = false;
				tiltHoldTime = 0f; // Start timer
			}
		}

		// Wait 3 seconds, then tilt back
		if (!isRotatingForward && !isRotatingBackward && rotateThisFrame >= rotation)
		{
			tiltHoldTime += Time.deltaTime;
			if (tiltHoldTime >= tiltMaxHoldTime)
			{
				isRotatingBackward = true;
			}
		}

		if (isRotatingBackward)
		{
			transform.Rotate(Vector3.left * roateSpeed);
			rotateThisFrame -= roateSpeed;
			if (rotateThisFrame <= 0)
            {
				isRotatingBackward = false;
				waterSplash.gameObject.SetActive(false);
            }
		}
	}
	private bool IsBucketEquipped()
	{
		if (inventoryManager == null) return false;
		Tool equipped = inventoryManager.GetCurrentTool();
		return equipped != null && equipped.toolName == "Bucket";
	}

}

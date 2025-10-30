using System.Diagnostics;
using NUnit.Framework;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Bucket : MonoBehaviour
{
    private bool isRotatingForward;
    private float rotateThisFrame;
    private float rotation = 30f;
    private bool isRotatingBackward;
    private float roateSpeed = 3f;
	private float tiltHoldTime = 0f; // Timer for how long to hold the tilt
	private float tiltMaxHoldTime = 1f; // 3 seconds

	public GameObject sphereColliderPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		isRotatingForward = false;
		rotateThisFrame = 0;
		isRotatingBackward = false;
	}

	// Update is called once per frame
	void Update()
    {

		if (Input.GetMouseButton(0))
		{
			if (rotateThisFrame == 0)
			{
				isRotatingForward = true;
				transform.Find("water_splash").gameObject.SetActive(false);
				transform.Find("water_splash").gameObject.SetActive(true);
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
				isRotatingBackward = false;
		}
	}
}

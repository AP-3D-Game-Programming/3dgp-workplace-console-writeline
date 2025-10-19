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
            else if (rotateThisFrame == rotation)
                isRotatingBackward = true;
        }

        if (isRotatingForward)
        {
            transform.Rotate(Vector3.right * roateSpeed);
            rotateThisFrame += roateSpeed;
            if (rotateThisFrame >= rotation)
                isRotatingForward = false;
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

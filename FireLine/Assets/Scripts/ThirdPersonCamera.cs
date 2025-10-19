using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -5);
    public float rotateSpeed = 150f; // muis gevoeligheid
    public float smoothSpeed = 10f;
    public float distance = 5f;

    private float rotX = 15f; // hoogte van de camera
    private float rotY = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // verberg cursor tijdens spel
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        // Muisinput
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -40f, 60f); // kantelbeperkingen

        // Rotatie berekenen
        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -distance) + Vector3.up * offset.y;

        // Smooth bewegen
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1.5f - transform.position);
    }
}

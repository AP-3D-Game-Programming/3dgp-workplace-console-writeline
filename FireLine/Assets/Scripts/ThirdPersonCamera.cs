using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 3, -5);
    public float rotateSpeed = 150f;
    public float smoothSpeed = 10f;
    public float distance = 5f;

    private float rotX = 15f;
    private float rotY = 0f;
   
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        rotY += mouseX;
        rotX -= mouseY;
        rotX = Mathf.Clamp(rotX, -40f, 60f);

        Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, 0, -distance) + Vector3.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1.5f - transform.position);
    }
}

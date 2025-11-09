using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerHaitam : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 100f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    public Camera firstPersonCam;
    public Transform cameraHolder;

    private Transform cameraTransform;
    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isSprinting = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        
        firstPersonCam.enabled = true;
        cameraTransform = firstPersonCam.transform;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleAnimations();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void HandleMovement()
    {
        bool wasGrounded = isGrounded;
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
        else if (!isGrounded && wasGrounded && velocity.y <= 0)
            velocity.y = -1f;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = (transform.forward * vertical + transform.right * horizontal).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    void HandleAnimations()
    {
        bool isJumpingNow = !isGrounded;
        bool isSprintingNow = Input.GetKey(KeyCode.LeftShift) && !isJumpingNow && anim.GetFloat("Speed") > 0.1f;

        anim.SetBool("isJumping", !isGrounded && velocity.y > 0.1f);
        anim.SetBool("isSprinting", isSprintingNow);

        if (!isJumpingNow)
        {
            float inputMagnitude = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
            anim.SetFloat("Speed", inputMagnitude, 0.1f, Time.deltaTime);
        }
    }
}


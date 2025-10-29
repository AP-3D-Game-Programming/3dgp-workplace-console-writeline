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

    [Header("Cameras")]
    public Camera firstPersonCam;
    public Camera thirdPersonCam;
    public Transform cameraHolder;

    private Transform cameraTransform;
    private CharacterController controller;
    private Animator anim;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isFirstPerson = true;
    private bool isSprinting = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;

        firstPersonCam.enabled = true;
        thirdPersonCam.enabled = false;
        cameraTransform = firstPersonCam.transform;
    }

    void Update()
    {
        HandleCameraSwitch();
        HandleMouseLook();
        HandleMovement();
        HandleAnimations();
    }

    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;
            firstPersonCam.enabled = isFirstPerson;
            thirdPersonCam.enabled = !isFirstPerson;
            cameraTransform = isFirstPerson ? firstPersonCam.transform : thirdPersonCam.transform;

            
            if (isFirstPerson)
            {
                yRotation = transform.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            }
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        
        Quaternion targetRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        cameraHolder.rotation = Quaternion.Slerp(cameraHolder.rotation, targetRotation, Time.deltaTime * 10f);

        
        if (isFirstPerson)
        {
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraHolder.forward, Vector3.up).normalized;
        Vector3 cameraRight = Vector3.ProjectOnPlane(cameraHolder.right, Vector3.up).normalized;

        Vector3 moveDir = (cameraForward * vertical + cameraRight * horizontal).normalized;

        if (moveDir.magnitude > 0.1f)
        {
            
            if (!isFirstPerson)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

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

        anim.SetBool("isJumping", isJumpingNow);
        anim.SetBool("isSprinting", isSprintingNow);

        if (!isJumpingNow)
        {
            float inputMagnitude = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude;
            anim.SetFloat("Speed", inputMagnitude, 0.1f, Time.deltaTime);
        }
    }
}


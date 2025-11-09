using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BLINK
{
    public class AnimationDemo : MonoBehaviour
    {
        public Animator animator;

        [Header("Movement Settings")]
        public float walkSpeed = 1.5f;
        public float runSpeed = 3f;
        public float turnSpeed = 5f;

        [Header("Animation Settings")]
        public float walkAnimationSpeedMultiplier = 1f;
        public float runAnimationSpeedMultiplier = 1f;

        [Header("Debug")]
        public bool showDebugInfo = true;

        [Header("Movement Bounds")]
        public MovementBounds movementBounds;

        [Header("Obstacle Avoidance")]
        public float obstacleAvoidanceDistance = 1f;
        public float avoidanceStrength = 90f; // graden bij normale obstakels

        private Vector3 moveDirection = Vector3.zero;
        private float currentSpeed = 0f;
        private Quaternion targetRotation;
        private string currentAction = "";

        void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            targetRotation = transform.rotation;

            animator.enabled = true;
            animator.speed = 1f;

            ForceCompleteReset();
            animator.SetBool("Idle", true);
            currentAction = "Idle";

            StartCoroutine(BehaviorLoop());
        }

        void Update()
        {
            // Obstakels en borders vermijden
            AvoidObstaclesAndBorders();

            // Rotatie
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            // Beweging
            if (currentSpeed > 0f)
            {
                moveDirection = transform.forward;
                Vector3 newPosition = transform.position + moveDirection * currentSpeed * Time.deltaTime;

                if (movementBounds != null)
                    newPosition = movementBounds.GetClampedPosition(newPosition);

                transform.position = newPosition;
            }

            // Animatiesnelheid
            if (currentAction == "WalkForward")
                animator.speed = walkAnimationSpeedMultiplier;
            else if (currentAction == "Run Forward")
                animator.speed = runAnimationSpeedMultiplier;
            else
                animator.speed = 1f;

            // Debug
            if (showDebugInfo && Time.frameCount % 120 == 0)
            {
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                string clipName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "None";
                Debug.Log($"<color=yellow>Action: {currentAction} | Clip: {clipName} | Speed: {currentSpeed:F2}</color>");
            }
        }

        void AvoidObstaclesAndBorders()
        {
            if (currentSpeed <= 0f) return;

            // Obstakel detectie (bomen, objecten)
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, obstacleAvoidanceDistance))
            {
                if (hit.collider != null && (movementBounds == null || hit.collider != movementBounds.boundsCollider))
                {
                    float angle = Random.value > 0.5f ? avoidanceStrength : -avoidanceStrength;
                    targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
                    if (showDebugInfo)
                        Debug.Log($"<color=red>Obstacle detected! Rotating {angle} degrees</color>");
                }
            }

            // Border detectie
            if (movementBounds != null)
            {
                Vector3 nextPos = transform.position + transform.forward * obstacleAvoidanceDistance;
                Vector3 clampedPos = movementBounds.GetClampedPosition(nextPos);

                if (Vector3.Distance(nextPos, clampedPos) > 0.01f)
                {
                    // Direct 180 graden draaien
                    targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                    if (showDebugInfo)
                        Debug.Log("<color=orange>Border reached! Rotating 180 degrees</color>");
                }
            }
        }

        void ForceCompleteReset()
        {
            animator.SetBool("Idle", false);
            animator.SetBool("WalkForward", false);
            animator.SetBool("Run Forward", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Eat", false);
            animator.SetBool("Buff", false);

            animator.Rebind();
            animator.Update(0f);
            animator.Update(0f);
        }

        IEnumerator BehaviorLoop()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                int choice = Random.Range(0, 12); // verhoogd bereik om Buff toe te voegen

                if (choice <= 4) // Walk (≈41%)
                {
                    SetNewRandomDirection();
                    yield return new WaitForSeconds(0.5f);
                    yield return StartCoroutine(SetActionCoroutine("WalkForward", walkSpeed));
                    yield return new WaitForSeconds(Random.Range(4f, 7f));
                }
                else if (choice <= 7) // Run (≈25%)
                {
                    SetNewRandomDirection();
                    yield return new WaitForSeconds(0.5f);
                    yield return StartCoroutine(SetActionCoroutine("Run Forward", runSpeed));
                    yield return new WaitForSeconds(Random.Range(3f, 5f));
                }
                else if (choice == 8) // Idle
                {
                    yield return StartCoroutine(SetActionCoroutine("Idle", 0f));
                    yield return new WaitForSeconds(Random.Range(2f, 4f));
                }
                else if (choice == 9) // Eat
                {
                    yield return StartCoroutine(SetActionCoroutine("Eat", 0f));
                    yield return new WaitForSeconds(3f);
                }
                else if (choice == 10) // Jump
                {
                    yield return StartCoroutine(SetActionCoroutine("Jump", 0f));
                    yield return new WaitForSeconds(2f);
                }
                else // Buff (choice == 11)
                {
                    yield return StartCoroutine(SetActionCoroutine("Buff", 0f));
                    yield return new WaitForSeconds(3f); // duur Buff animatie
                }

                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator SetActionCoroutine(string action, float speed)
        {
            if (showDebugInfo)
                Debug.Log($"<color=cyan>>>> SWITCHING TO: {action}</color>");

            ForceCompleteReset();
            yield return null;

            currentAction = action;
            currentSpeed = speed;
            animator.SetBool(action, true);

            yield return null;
            animator.Update(Time.deltaTime);

            if (showDebugInfo)
                Debug.Log($"<color=green>>>> ACTIVATED: {action}</color>");
        }

        void SetNewRandomDirection()
        {
            float randomY = Random.Range(0f, 360f);
            targetRotation = Quaternion.Euler(0, randomY, 0);
        }
    }
}


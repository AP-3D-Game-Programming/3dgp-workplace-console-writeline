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
        public float turnSpeed = 12f;

        [Header("Animation Settings")]
        public float walkAnimationSpeedMultiplier = 1f;
        public float runAnimationSpeedMultiplier = 1f;

        [Header("Debug")]
        public bool showDebugInfo = true;

        [Header("Movement Bounds")]
        public MovementBounds movementBounds;

        [Header("Obstacle Avoidance")]
        public float obstacleAvoidanceDistance = 1f;
        public float avoidanceStrength = 90f;

        [Header("Border Avoidance")]
        public float borderLookAheadDistance = 3f;
        public float borderAvoidanceDistance = 2f;
        public float borderTurnSpeed = 60f;

        [Header("Player Detection")]
        public Transform player;
        public float visionDistance = 10f;
        public float visionAngle = 120f;
        public float attackRange = 1.5f;
        public float attackCooldown = 0.1f;

        [Header("Knockback Settings")]
        public float knockbackDistance = 3f;
        public float knockbackDuration = 0.2f;

        private Vector3 moveDirection = Vector3.zero;
        private float currentSpeed = 0f;
        private Quaternion targetRotation;
        private string currentAction = "";
        private bool isAvoidingBorder = false;
        private float borderAvoidanceTimer = 0f;
        private float lastBorderAvoidanceTime = 0f;
        private bool isInCombat = false;
        private Vector3 startPosition;

        [Header("Dangerous Surface Detection")]
        public GameObject dangerousSurface; // sleep hier je object in Inspector
        public float groundCheckDistance = 2f; // afstand van raycast naar beneden

        void Start()
        {
            startPosition = transform.position; // startpositie opslaan

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
            // --------------------------
            // CHECK DANGEROUS SURFACE
            // --------------------------
            CheckDangerousSurface();

            bool playerVisible = IsPlayerVisible();

            if (borderAvoidanceTimer > 0f)
                borderAvoidanceTimer -= Time.deltaTime;

            if (!playerVisible && borderAvoidanceTimer <= 0f)
                CheckAndAvoidBorder();
            else if (playerVisible)
                isAvoidingBorder = false;

            if (borderAvoidanceTimer <= 0f)
                AvoidObstacles();

            float rotSpeed = isAvoidingBorder ? borderTurnSpeed : turnSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);

            float angleToTarget = Quaternion.Angle(transform.rotation, targetRotation);
            bool isStillTurning = angleToTarget > 15f;

            if (currentSpeed > 0f && !(isAvoidingBorder && isStillTurning))
            {
                moveDirection = transform.forward;
                Vector3 newPosition = transform.position + moveDirection * currentSpeed * Time.deltaTime;

                if (movementBounds != null)
                {
                    Vector3 clampedPos = movementBounds.GetClampedPosition(newPosition);
                    float distanceToBoundary = Vector3.Distance(newPosition, clampedPos);

                    if (distanceToBoundary > 0.01f)
                    {
                        if (!isAvoidingBorder)
                        {
                            targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                            isAvoidingBorder = true;
                            borderAvoidanceTimer = 2f;
                            lastBorderAvoidanceTime = Time.time;

                            if (showDebugInfo)
                                Debug.Log("<color=red>EMERGENCY: At border! Turning 180°!</color>");
                        }
                    }
                    else
                        transform.position = newPosition;
                }
                else
                    transform.position = newPosition;
            }

            if (isAvoidingBorder && angleToTarget < 5f)
            {
                isAvoidingBorder = false;
                if (showDebugInfo)
                    Debug.Log("<color=green>Turn complete! Resuming normal behavior.</color>");
            }

            if (currentAction == "WalkForward")
                animator.speed = walkAnimationSpeedMultiplier;
            else if (currentAction == "Run Forward")
                animator.speed = runAnimationSpeedMultiplier;
            else
                animator.speed = 1f;

            if (showDebugInfo && Time.frameCount % 120 == 0)
            {
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                string clipName = clipInfo.Length > 0 ? clipInfo[0].clip.name : "None";
                Debug.Log($"<color=yellow>Action: {currentAction} | Clip: {clipName} | Speed: {currentSpeed:F2} | InCombat: {isInCombat}</color>");
            }

            if (playerVisible && !isInCombat)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer > attackRange)
                {
                    SetNewDirectionTowards(player.position);
                    if (currentAction != "Run Forward")
                        StartCoroutine(SetActionCoroutine("Run Forward", runSpeed));
                }
                else
                {
                    StartCoroutine(AttackPlayer());
                }
            }

            StickToGround();
        }

        void CheckDangerousSurface()
        {
            if (dangerousSurface == null) return;

            if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, groundCheckDistance))
            {
                if (hit.collider.gameObject == dangerousSurface)
                {
                    if (showDebugInfo)
                        Debug.LogWarning($"<color=red>Bear touched dangerous surface '{hit.collider.name}'! Respawning...</color>");
                    RespawnToStart();
                }
            }
        }

        void RespawnToStart()
        {
            transform.position = startPosition;
            targetRotation = transform.rotation;

            currentSpeed = 0f;
            isInCombat = false;
            isAvoidingBorder = false;

            StopAllCoroutines();
            ForceCompleteReset();
            animator.SetBool("Idle", true);
            currentAction = "Idle";

            StartCoroutine(BehaviorLoop());
        }

        // -----------------------
        // REST VAN JE ORIGINELE CODE
        // -----------------------
        void CheckAndAvoidBorder()
        {
            if (movementBounds == null || currentSpeed <= 0f) return;

            Vector3 currentClamped = movementBounds.GetClampedPosition(transform.position);
            if (Vector3.Distance(transform.position, currentClamped) > 0.1f)
            {
                transform.position = currentClamped;
                targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                isAvoidingBorder = true;
                borderAvoidanceTimer = 2f;
                Debug.LogWarning("<color=red>Bear was outside bounds! Teleported back in.</color>");
                return;
            }

            Vector3 lookAheadPos = transform.position + transform.forward * borderLookAheadDistance;
            Vector3 clampedLookAhead = movementBounds.GetClampedPosition(lookAheadPos);
            float distanceToBorder = Vector3.Distance(lookAheadPos, clampedLookAhead);

            Vector3 leftDir = Quaternion.Euler(0, -30, 0) * transform.forward;
            Vector3 leftLookAhead = transform.position + leftDir * borderLookAheadDistance;
            Vector3 clampedLeftLookAhead = movementBounds.GetClampedPosition(leftLookAhead);
            float leftDistanceToBorder = Vector3.Distance(leftLookAhead, clampedLeftLookAhead);

            Vector3 rightDir = Quaternion.Euler(0, 30, 0) * transform.forward;
            Vector3 rightLookAhead = transform.position + rightDir * borderLookAheadDistance;
            Vector3 clampedRightLookAhead = movementBounds.GetClampedPosition(rightLookAhead);
            float rightDistanceToBorder = Vector3.Distance(rightLookAhead, clampedRightLookAhead);

            if (distanceToBorder > 0.1f || leftDistanceToBorder > 0.1f || rightDistanceToBorder > 0.1f)
            {
                float closestDistance = Mathf.Min(distanceToBorder, leftDistanceToBorder, rightDistanceToBorder);

                if (closestDistance < borderAvoidanceDistance && !isAvoidingBorder)
                {
                    isAvoidingBorder = true;
                    borderAvoidanceTimer = 2f;

                    float turnAngle = leftDistanceToBorder < rightDistanceToBorder ? 120f : -120f;
                    targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + turnAngle, 0);

                    if (showDebugInfo)
                        Debug.Log($"<color=orange>Border detected ahead! Distance: {closestDistance:F2}m. Turning {turnAngle}°</color>");
                }
            }
        }

        void AvoidObstacles()
        {
            if (currentSpeed <= 0f || isAvoidingBorder) return;

            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, obstacleAvoidanceDistance))
            {
                bool isMovementBound = movementBounds != null && hit.collider == movementBounds.boundsCollider;
                bool isPlayer = player != null && hit.collider.transform == player;

                if (!isMovementBound && !isPlayer)
                {
                    float angle = Random.value > 0.5f ? avoidanceStrength : -avoidanceStrength;
                    targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);
                    borderAvoidanceTimer = 1f;

                    if (showDebugInfo)
                        Debug.Log($"<color=red>Obstacle '{hit.collider.name}' detected! Rotating {angle}°</color>");
                }
            }
        }

        void ForceCompleteReset()
        {
            animator.SetBool("Idle", false);
            animator.SetBool("Run Forward", false);
            animator.SetBool("WalkForward", false);
            animator.SetBool("Eat", false);
            animator.SetBool("Jump", false);
            animator.SetBool("Buff", false);
            animator.SetBool("Attack1", false);

            animator.Rebind();
            animator.Update(0f);
            animator.Update(0f);
        }

        IEnumerator BehaviorLoop()
        {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                if (!IsPlayerVisible() && !isInCombat)
                {
                    int choice = Random.Range(0, 10);

                    if (choice <= 4)
                    {
                        SetNewRandomDirection();
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(SetActionCoroutine("WalkForward", walkSpeed));
                        yield return new WaitForSeconds(Random.Range(4f, 7f));
                    }
                    else if (choice <= 7)
                    {
                        SetNewRandomDirection();
                        yield return new WaitForSeconds(0.5f);
                        yield return StartCoroutine(SetActionCoroutine("Run Forward", runSpeed));
                        yield return new WaitForSeconds(Random.Range(3f, 5f));
                    }
                    else if (choice == 8)
                    {
                        yield return StartCoroutine(SetActionCoroutine("Idle", 0f));
                        yield return new WaitForSeconds(Random.Range(2f, 4f));
                    }
                    else
                    {
                        string action = Random.value > 0.5f ? "Eat" : "Jump";
                        yield return StartCoroutine(SetActionCoroutine(action, 0f));
                        yield return new WaitForSeconds(action == "Eat" ? 3f : 2f);
                    }

                    yield return new WaitForSeconds(0.3f);
                }
                else
                    yield return null;
            }
        }

        IEnumerator SetActionCoroutine(string action, float speed)
        {
            ForceCompleteReset();
            yield return null;

            currentAction = action;
            currentSpeed = speed;
            animator.SetBool(action, true);

            yield return null;
            animator.Update(Time.deltaTime);
        }

        void SetNewRandomDirection()
        {
            float randomY = Random.Range(0f, 360f);
            targetRotation = Quaternion.Euler(0, randomY, 0);
        }

        void SetNewDirectionTowards(Vector3 position)
        {
            Vector3 dir = (position - transform.position).normalized;
            dir.y = 0;
            if (dir != Vector3.zero)
                targetRotation = Quaternion.LookRotation(dir);
        }

        IEnumerator AttackPlayer()
        {
            isInCombat = true;

            if (showDebugInfo)
                Debug.Log("<color=red>>>> ENTERING COMBAT MODE <<<</color>");

            while (IsPlayerVisible())
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (distanceToPlayer <= attackRange)
                {
                    currentSpeed = 0f;
                    SetNewDirectionTowards(player.position);

                    ForceCompleteReset();
                    currentAction = "Attack1";
                    animator.SetBool("Attack1", true);

                    if (showDebugInfo)
                        Debug.Log("<color=red>>>> ATTACKING! <<<</color>");

                    yield return new WaitForSeconds(0.1f);

                    player.GetComponent<GameOverSimple>()?.TakeHit();
                    player.GetComponent<SimpleKnockBack>()?.ApplyKnockback(transform.position);
                    player.GetComponent<PlayerDamageIndicator>()?.ShowDamage();

                    yield return new WaitForSeconds(attackCooldown);
                }
                else
                {
                    SetNewDirectionTowards(player.position);
                    currentAction = "Run Forward";
                    currentSpeed = runSpeed;
                    animator.SetBool("Run Forward", true);
                    yield return new WaitForSeconds(0.1f);
                }
            }

            isInCombat = false;
            currentSpeed = 0f;

            ForceCompleteReset();
            currentAction = "Idle";
            animator.SetBool("Idle", true);

            if (showDebugInfo)
                Debug.Log("<color=green>>>> EXITING COMBAT MODE <<<</color>");
        }

        void StickToGround()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
            {
                transform.position = new Vector3(
                    transform.position.x,
                    hit.point.y,
                    transform.position.z
                );
            }
        }

        void OnDrawGizmos()
        {
            if (movementBounds == null || !Application.isPlaying) return;

            Vector3 origin = transform.position + Vector3.up * 0.5f;

            Gizmos.color = isAvoidingBorder ? Color.red : Color.green;
            Gizmos.DrawLine(origin, origin + transform.forward * borderLookAheadDistance);

            Vector3 leftDir = Quaternion.Euler(0, -30, 0) * transform.forward;
            Gizmos.DrawLine(origin, origin + leftDir * borderLookAheadDistance);

            Vector3 rightDir = Quaternion.Euler(0, 30, 0) * transform.forward;
            Gizmos.DrawLine(origin, origin + rightDir * borderLookAheadDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, borderAvoidanceDistance);

            if (player != null)
            {
                Gizmos.color = isInCombat ? Color.red : Color.cyan;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }
        }

        bool IsPlayerVisible()
        {
            if (player == null) return false;
            Vector3 dir = player.position - transform.position;
            if (dir.magnitude > visionDistance) return false;
            float angle = Vector3.Angle(transform.forward, dir);
            return angle <= visionAngle * 0.5f;
        }
    }
}


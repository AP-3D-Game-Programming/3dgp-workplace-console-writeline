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
        public float turnSpeed = 12f; // hogere waarde voor scherpere bochten

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

        [Header("Player Detection")]
        public Transform player;
        public float visionDistance = 10f;
        public float visionAngle = 120f;
        public float attackRange = 1.5f;
        public float attackCooldown = 2f;

        [Header("Knockback Settings")]
        public float knockbackDistance = 3f;
        public float knockbackDuration = 0.2f;

        private Vector3 moveDirection = Vector3.zero;
        private float currentSpeed = 0f;
        private Quaternion targetRotation;
        private string currentAction = "";
        private float lastAttackTime = -999f;

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
            bool playerVisible = IsPlayerVisible();

            AvoidObstacles();

            // Scherpe draaiing
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

            // Movement
            if (currentSpeed > 0f)
            {
                moveDirection = transform.forward;
                Vector3 newPosition = transform.position + moveDirection * currentSpeed * Time.deltaTime;

                if (movementBounds != null)
                {
                    Vector3 clampedPos = movementBounds.GetClampedPosition(newPosition);

                    // Border check
                    if (Vector3.Distance(newPosition, clampedPos) > 0.01f)
                    {
                        targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 180f, 0);
                        newPosition = transform.position;
                    }
                }

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

            // Player follow / attack
            if (playerVisible)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                // Draai naar speler
                SetNewDirectionTowards(player.position);

                if (distanceToPlayer > attackRange)
                {
                    // Achtervolging
                    if (currentAction != "Run Forward")
                        StartCoroutine(SetActionCoroutine("Run Forward", runSpeed));
                }
                else
                {
                    // Aanval
                    if (Time.time - lastAttackTime > attackCooldown)
                    {
                        lastAttackTime = Time.time;
                        StartCoroutine(AttackPlayer());
                    }
                }
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

        void AvoidObstacles()
        {
            if (currentSpeed <= 0f) return;

            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, obstacleAvoidanceDistance))
            {
                if (hit.collider != null && (movementBounds == null || hit.collider != movementBounds.GetComponent<Collider>()))
                {
                    float angle = Random.value > 0.5f ? avoidanceStrength : -avoidanceStrength;
                    targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + angle, 0);

                    if (showDebugInfo)
                        Debug.Log($"<color=red>Obstacle detected! Rotating {angle} degrees</color>");
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
                if (!IsPlayerVisible())
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
            targetRotation = Quaternion.LookRotation(dir);
        }

        IEnumerator AttackPlayer()
        {
            // Draai onmiddellijk naar speler
            SetNewDirectionTowards(player.position);

            // Speel attack animatie terwijl beer kan bewegen
            StartCoroutine(SetActionCoroutine("Attack1", runSpeed));

            // Knockback toepassen
            SimpleKnockBack knockback = player.GetComponent<SimpleKnockBack>();
            if (knockback != null)
                knockback.ApplyKnockback(transform.position);

            // Korte pauze zodat animatie zichtbaar is
            yield return new WaitForSeconds(0.2f);

            // Direct terug naar achtervolging en aanval indien speler nog in vizier
            while (IsPlayerVisible() && Vector3.Distance(transform.position, player.position) <= attackRange)
            {
                // Blijf speler raken
                SetNewDirectionTowards(player.position);
                yield return StartCoroutine(SetActionCoroutine("Attack1", runSpeed));
                knockback?.ApplyKnockback(transform.position);
                yield return new WaitForSeconds(attackCooldown);
            }

            // Als speler buiten bereik: direct achtervolgen
            if (IsPlayerVisible() && Vector3.Distance(transform.position, player.position) > attackRange)
            {
                SetNewDirectionTowards(player.position);
                StartCoroutine(SetActionCoroutine("Run Forward", runSpeed));
            }

            PlayerDamageIndicator dmg = player.GetComponent<PlayerDamageIndicator>();
            if (dmg != null) dmg.ShowDamage();
        }
    }
}


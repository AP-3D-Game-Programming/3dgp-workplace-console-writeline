using UnityEngine;

public class BearVision : MonoBehaviour
{
    public Transform player;
    public float viewDistance = 15f;
    public float viewAngle = 80f;
    public LayerMask visionMask;

    public bool PlayerVisible { get; private set; }

    void Update()
    {
        if (player == null)
        {
            PlayerVisible = false;
            return;
        }

        Vector3 dir = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > viewDistance)
        {
            PlayerVisible = false;
            return;
        }

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f)
        {
            PlayerVisible = false;
            return;
        }

        // Check line of sight
        if (Physics.Raycast(transform.position + Vector3.up * 1f, dir, out RaycastHit hit, viewDistance, visionMask))
        {
            if (hit.collider.transform == player)
            {
                PlayerVisible = true;
                return;
            }
        }

        PlayerVisible = false;
    }
}

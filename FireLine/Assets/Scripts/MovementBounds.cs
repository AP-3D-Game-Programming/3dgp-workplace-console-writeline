using UnityEngine;

public class MovementBounds : MonoBehaviour
{
    [HideInInspector]
    public Collider boundsCollider;

    void Awake()
    {
        boundsCollider = GetComponent<Collider>();
        if (boundsCollider == null)
        {
            Debug.LogError("MovementBounds requires a Collider component!");
        }
    }

    public Vector3 GetClampedPosition(Vector3 position)
    {
        if (boundsCollider == null) return position;
        return boundsCollider.ClosestPoint(position);
    }

    void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.25f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}

using UnityEngine;

public class WorldHUDIndicator : MonoBehaviour
{
    public Transform target;       // Object dat je volgt
    public Camera mainCamera;      // Spelercamera
    public float edgeBuffer = 50f; // Buffer van de rand

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2f;
        Vector3 fromCenterToTarget = screenPos - screenCenter;

        // Als object achter camera is, draai de vector om
        if (screenPos.z < 0)
            fromCenterToTarget *= -1;

        // Normale richting behouden
        fromCenterToTarget.Normalize();

        // Plaats indicator op rand van scherm
        float maxX = screenCenter.x - edgeBuffer;
        float maxY = screenCenter.y - edgeBuffer;

        Vector3 finalPos = screenCenter + fromCenterToTarget * Mathf.Min(maxX, maxY);

        rectTransform.position = finalPos;
    }
}


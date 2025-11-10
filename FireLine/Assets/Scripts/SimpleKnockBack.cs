using System.Collections;
using UnityEngine;

public class SimpleKnockBack : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float knockbackDistance = 3f;
    public float knockbackDuration = 0.2f;

    private CharacterController characterController;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("SimpleKnockback requires a CharacterController!");
        }
    }

    public void ApplyKnockback(Vector3 sourcePosition)
    {
        StartCoroutine(KnockbackCoroutine(sourcePosition));
    }

    private IEnumerator KnockbackCoroutine(Vector3 sourcePosition)
    {
        if (characterController == null) yield break;

        Vector3 direction = (transform.position - sourcePosition).normalized;
        direction.y = 0;

        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + direction * knockbackDistance;

        while (elapsed < knockbackDuration)
        {
            float t = elapsed / knockbackDuration;
            Vector3 move = Vector3.Lerp(startPos, targetPos, t) - transform.position;
            characterController.Move(move);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Vector3 finalMove = targetPos - transform.position;
        characterController.Move(finalMove);
    }
}

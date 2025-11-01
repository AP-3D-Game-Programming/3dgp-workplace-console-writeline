using ithappy.Animals_FREE;
using System.Collections;
using UnityEngine;

public class CreatureAI : MonoBehaviour
{
    [Header("Wandelgebied")]
    public Vector3 centerPosition;   // Centrum van het gebied
    public float walkRadius = 10f;   // Hoe ver het dier maximaal mag lopen

    [Header("Bewegingsgedrag")]
    public float minWaitTime = 2f;
    public float maxWaitTime = 6f;
    public bool canRun = false;

    private CreatureMover mover;
    private Vector3 currentTarget;

    private void Start()
    {
        mover = GetComponent<CreatureMover>();

        if (centerPosition == Vector3.zero)
            centerPosition = transform.position; // startpositie als centrum

        StartCoroutine(WanderRoutine());
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Kies een willekeurig punt binnen de straal
            Vector2 randomCircle = Random.insideUnitCircle * walkRadius;
            currentTarget = centerPosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

            // Beweeg naar het doel
            float moveDuration = Random.Range(3f, 6f);
            float timer = 0f;

            // Willekeurig lopen of rennen
            bool isRunning = canRun && Random.value > 0.7f;

            while (timer < moveDuration)
            {
                timer += Time.deltaTime;

                // Richting naar doel
                Vector3 dir = (currentTarget - transform.position);
                dir.y = 0f;

                // Stop als we dicht genoeg zijn
                if (dir.magnitude < 0.5f) break;

                dir.Normalize();
                Vector2 axis = new Vector2(0f, 1f); // vooruitbeweging

                mover.SetInput(axis, transform.position + dir, isRunning, false);

                yield return null;
            }

            // Stop even
            mover.SetInput(Vector2.zero, transform.position, false, false);
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        }
    }
}
